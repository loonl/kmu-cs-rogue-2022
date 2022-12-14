using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;
using Slider = UnityEngine.UI.Slider;

// 행동 목록
public enum ActionList
{
    Wandering, // 방랑 중
    Chasing, // 추적 중
    SkillCasting1, // 스킬1 캐스팅 중
    OnDamaging, // 피격 중
}

//몬스터 종류
public enum MonsterType{
    Zombie,
    RushZombie,
    RevivalZombie,
    FastZombie
}

public class Monster : MonoBehaviour
{
    public GameObject hpBar; // 체력바
    public GameObject hpBarPrefab; // 체력바 프리팹
    public GameObject goldTxt; // 골드 텍스트
    public GameObject canvas; // 캔버스
    private GameObject doteffect; // 도트대미지 이펙트

    public List<Dictionary<string, object>> monsterData; // 몬스터 데이터 !!고칠 코드

    public Animator animator;
    protected AudioSource audioPlayer;
    protected Rigidbody2D rigidbody2d;
    protected CapsuleCollider2D capsuleCollider2D;

    public int id; // 몬스터 Id

    public MonsterStat stat; // 몬스터 스텟
    public List<AudioClip> sound; // 0 공격 혹은 부활 시 재생소리(일반좀비는 null) 1 피격시 재생 소리 2 사망시 재생 소리
    protected Player player; // 플레이어
    protected MonsterSpawner spawner; // 부모 스포너 객체
    
    public bool isDead; // 사망 여부
    protected bool targetOn; // 타깃 여부
    protected bool actionChanged; // 행동 변경 여부
    protected bool actionFinished; // 행동 종료 여부
    protected Coroutine currentActionCoroutine; // 현재 행동 코루틴
    public bool isInvulnerable; // 무적 여부
    public bool onDotdmg; // 도트 데미지 받고있는 여부
    private ActionList action; // 현재 행동
    
    protected float distance = 100; // 플레이어와의 거리
    protected Vector2 direction; // 플레이어 방향
    protected Vector2 randomDirection; // 랜덤 방향
    protected float attackCoolTime = 0.5f; // 공격 쿨타임
    protected float lastAttackTime; // 마지막 공격 시점
    protected float randomDirectionCoolTime = 3f; // 랜덤 경로 업데이트 쿨타임
    protected float lastRandomDirectionUpdate; // 마지막 랜덤 방향 업데이트 시점
    protected float knockBackForce; // 넉백 힘
    protected Vector2 knockBackDirection; // 넉백 방향
    protected MonsterType Monstertype;


    // action 프로퍼티
    public ActionList Action
    {
        get
        {
            return action;
        }
        protected set 
        {
            actionChanged = true;
            action = value;
        }
    }
    // --------------------------------------
    // 기본 메소드
    // --------------------------------------
    protected virtual void Awake()
    {
        // 컴포넌트 초기화
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        audioPlayer = GetComponent<AudioSource>();
        if (audioPlayer == null)
            audioPlayer = gameObject.AddComponent<AudioSource>();
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        
        // 몬스터 HP바 생성
        hpBarPrefab = Resources.Load<GameObject>("Prefabs/UI/MonsterHp");
        goldTxt = Resources.Load<GameObject>("Prefabs/UI/CoinTxt");
        canvas = Resources.Load<GameObject>("Prefabs/UI/MonsterHPCanvas");
    }

    protected void Start()
    {
        Init(); // 몬스터 초기화
    }
    
    // 몬스터 초기화
    protected virtual void Init()
    {
        stat = new MonsterStat(monsterData, id); // !! 고칠 코드
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        spawner = transform.GetComponentInParent<MonsterSpawner>();
        
        gameObject.GetComponentInChildren<SortingGroup>().sortingOrder = 2;
        
        Generate(); // 몬스터 생성
    }



    // 몬스터 활성화
    public virtual void Generate()
    {
        capsuleCollider2D.enabled = true;
        isDead = false;
        targetOn = false;
        actionChanged = true;
        actionFinished = true;
        isInvulnerable = false;
        Action = ActionList.Wandering;

        if (hpBar == null)
        {
            canvas = Instantiate(canvas, transform.position, Quaternion.identity);
            canvas.transform.SetParent(transform);
            canvas.transform.localPosition = new Vector3(0, 1f, 0);
            canvas.transform.localScale = new Vector3(1, 1, 1);

            hpBar = Instantiate(hpBarPrefab, transform.position, Quaternion.identity);
            hpBar.transform.SetParent(canvas.transform);
            hpBar.transform.localPosition = new Vector3(0, 0, 0);
            hpBar.transform.localScale = new Vector3(0.01f, 0.01f, 0);
        }

        hpBar.SetActive(false);
        
        stat.healthToMax();

        StartCoroutine(UpdatePath());
    }

    // --------------------------------------
    // 행동 코루틴
    // --------------------------------------
    
    // 경로 갱신
    protected IEnumerator UpdatePath()
    {
        StartCoroutine(CheckAction());

        while (!isDead)
        {
            if (player != null)
            {
                distance = Vector2.Distance(player.transform.position, transform.position);
                direction = (player.transform.position - transform.position).normalized;
            }

            animator.SetBool("HasTarget", true);
            
            yield return new WaitForSeconds(0.05f);
        }
    }

    protected IEnumerator CheckAction()
    {
        while(!isDead)
        {
            if (actionChanged)
            {
                actionChanged = false;
                actionFinished = false;
                rigidbody2d.velocity = Vector2.zero;

                switch (Action)
                {
                    case ActionList.Wandering:
                        currentActionCoroutine = StartCoroutine(Wandering());
                        break;
                    case ActionList.Chasing:
                        currentActionCoroutine = StartCoroutine(Chasing());
                        break;
                    case ActionList.SkillCasting1:
                        currentActionCoroutine = StartCoroutine(SkillCasting1());
                        break;
                    case ActionList.OnDamaging:
                        currentActionCoroutine = StartCoroutine(OnDamaging());
                        break;
                }
            }
            else
            {
                if (actionFinished)
                {
                    Action = ActionList.Chasing;
                }
            }

            yield return new WaitForSeconds(0.05f);
        }

        if(currentActionCoroutine != null) StopCoroutine(currentActionCoroutine);
        currentActionCoroutine = StartCoroutine(Dying());
    }

    // 방랑 수행
    protected virtual IEnumerator Wandering()
    {
        stat.ChangeSpeed(1);
        lastRandomDirectionUpdate = Time.time;
        randomDirection = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1.0f, 1.0f)).normalized;
        randomDirectionCoolTime = UnityEngine.Random.Range(2f, 3f);
        
        while (!isDead && Action == ActionList.Wandering)
        {

            rigidbody2d.velocity = randomDirection * stat.speed;
            UpdateEyes();

            if (distance < stat.sight || targetOn)
            {
                stat.ChangeSpeed(2);
                targetOn = true;
                Action = ActionList.Chasing;
            }
            else if (Time.time >= lastRandomDirectionUpdate + randomDirectionCoolTime)
            {
                randomDirection = new Vector2(UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f));
                randomDirectionCoolTime = UnityEngine.Random.Range(2f, 3f);
                lastRandomDirectionUpdate = Time.time;
            }
            
            yield return new WaitForSeconds(0.05f);
        }

        actionFinished = true;
    }

    // 추적 수행
    protected virtual IEnumerator Chasing() 
    {
        while (!isDead && Action == ActionList.Chasing)
        {
            rigidbody2d.velocity = direction * stat.speed;
            UpdateEyes();
            
            yield return new WaitForSeconds(0.05f);
        }
    }

    // 스킬1 수행
    protected virtual IEnumerator SkillCasting1()
    {
        while (!isDead && Action == ActionList.SkillCasting1)
        {

            yield return new WaitForSeconds(0.05f);
        }
    }

    // 피격 수행
    protected virtual IEnumerator OnDamaging()
    {
        rigidbody2d.AddForce(knockBackDirection * knockBackForce, ForceMode2D.Impulse);
        Vector2 startWay = rigidbody2d.velocity;

        while (!isDead && Action == ActionList.OnDamaging
            && (startWay.x > 0 && rigidbody2d.velocity.x > 0) || (startWay.x < 0 && rigidbody2d.velocity.x < 0)
            && (startWay.y > 0 && rigidbody2d.velocity.y > 0) || (startWay.y < 0 && rigidbody2d.velocity.y < 0))
        {
            rigidbody2d.AddForce(-knockBackDirection * (knockBackForce * 12f), ForceMode2D.Force);

            yield return new WaitForSeconds(0.05f);
        }

        actionFinished = true;
    }

    // 시체 상태 수행
    protected virtual IEnumerator Dying()
    {
        while (isDead)
        {
            rigidbody2d.velocity = Vector2.zero;
            
            yield return new WaitForSeconds(0.05f);
        }
    }

    // -------------------------------------
    // 실행 메소드
    // -------------------------------------
    
    // 피격 시 실행
    public virtual void OnDamage(float damage, float _knockBackForce, Vector2 _knockBackDirection = default(Vector2), WaitForSeconds invulnerabletime = null)
    {   
        if(_knockBackForce != 0 && _knockBackDirection == default(Vector2))
        {
            _knockBackDirection = (gameObject.transform.position - player.transform.position).normalized;
        }
        stat.OnDamage(damage);
        targetOn = true;
        
        hpBar.GetComponent<Slider>().value = stat.health / stat.maxHealth;
        if (stat.health<stat.maxHealth)
        {
            hpBar.SetActive(true);
        }

        if(invulnerabletime != null) 
            StartCoroutine(SetInvulnerable(invulnerabletime));
        
        if (stat.health <= 0)
        {
            Die();
        }
        else
        {
            if (Action != ActionList.SkillCasting1)
            {
                if (Action == ActionList.OnDamaging && currentActionCoroutine != null)
                {
                    StopCoroutine(currentActionCoroutine);
                }

                knockBackForce = _knockBackForce;
                knockBackDirection = _knockBackDirection;
                Action = ActionList.OnDamaging;
            }
            
            SoundPlay(Random.Range(2, 5));
        }
    }

    // 사망 시 실행
    public virtual void Die()
    {
        capsuleCollider2D.enabled = false;
        isDead = true;

        DropGold();
        spawner.aliveMonsters.Remove(this);
        spawner.deadMonsters.Add(this);
        spawner.CheckRemainEnemy();

        hpBar.SetActive(false);

        animator.SetTrigger("Die");
        SoundPlay(Random.Range(5, 7));
    }

    // 소지금에 골드 추가
    public void DropGold()
    {
        GameManager.Instance.Player.Inventory.UpdateGold(stat.gold);
        
        // score 업데이트
        GameManager.Instance.score += stat.gold * 100;

        //UI 골드 추가
        if (stat.gold != 0)
        {
            GameObject temp = Instantiate(goldTxt, transform.position, Quaternion.identity);
            temp.transform.SetParent(canvas.transform);
            if(this.transform.localScale.x<0)
                temp.transform.localScale = new Vector3(-0.01f, 0.01f, 1);
            else
                temp.transform.localScale = new Vector3(0.01f, 0.01f, 1);
            temp.GetComponent<TextMeshProUGUI>().text = $"+{stat.gold}G";
        }
    }

    // 시야 방향 갱신
    protected void UpdateEyes()
    {
        if (rigidbody2d.velocity.x > 0)
        {
            transform.localScale = new Vector3(-stat.scale, stat.scale, 1);
            hpBar.transform.localScale = new Vector3(-0.01f, 0.01f, 1);
        }
        else if (rigidbody2d.velocity.x < 0)
        {
            transform.localScale = new Vector3(stat.scale, stat.scale, 1);
            hpBar.transform.localScale = new Vector3(0.01f, 0.01f, 1);
        }
    }

    protected virtual void OnCollisionStay2D(Collision2D other)
    {
        // 충돌한 게임 오브젝트가 추적 대상이라면 공격
        if (!other.gameObject.CompareTag("Player"))
        {
            randomDirection = new Vector2(UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f));
            lastRandomDirectionUpdate = Time.time;
        }
        else if (Time.time >= lastAttackTime + attackCoolTime)
        {
            lastAttackTime = Time.time;
            player.OnDamage(stat.damage, 5f, (other.transform.position - transform.position).normalized);
            
            animator.SetTrigger("Attack_Normal");
            SoundPlay(Random.Range(0, 2));
        }
    }

    public void SoundPlay(int soundNum)
    {
        if (audioPlayer.isPlaying)
            audioPlayer.Stop();

        audioPlayer.clip = sound[soundNum];
        audioPlayer.volume = SoundManager.Instance.effectvolume * SoundManager.Instance.totalvolume;
        audioPlayer.Play();
    }

    protected IEnumerator SetInvulnerable(WaitForSeconds timer)
    {
        isInvulnerable = true;
        yield return timer;
        isInvulnerable = false;
    }

    public void SetDotDmg(float prob, float dmg, float delay, float duration, string effectname = "") // 도트데미지 set, prob: 걸릴 확률 (0<prob<1)
    {
        if (UnityEngine.Random.Range(0.0f, 1.0f) > prob) return; // 도트 대미지 걸기 실패
        //{ StopCoroutine("DoDotDmg"); }
        if (!onDotdmg)
        {
            doteffect = Instantiate(Resources.Load($"Prefabs/Effect/{effectname}")) as GameObject; // 도트 이펙트 세팅
            doteffect.transform.position = transform.position;
            doteffect.transform.SetParent(transform);
            doteffect.GetComponent<Animator>().SetTrigger(effectname);
            if (isDead) Destroy(doteffect);
        }
        if (!isDead)
        {
            StartCoroutine(DoDotDmg(dmg, delay, GameManager.Instance.Setwfs((int)(delay * 100)), duration));
            onDotdmg = true;
        }
    }

    protected IEnumerator DoDotDmg(float dmg, float delayf, WaitForSeconds delay, float duration) // 도트데미지 적용
    {
        float starttime = Time.time;
        while(Time.time < starttime + duration)
        {
            Debug.Log("dot");
            if (isDead && onDotdmg) // 다음 도트데미지 받기 전에 플레이어의 공격으로 죽었을 수도 있음.
            {
                onDotdmg = false;
                doteffect.GetComponent<Animator>().SetBool("dotdmgEnd", true);
                yield break;
            }
            OnDamage(dmg, 0f, Vector2.zero);
            if (isDead && onDotdmg)
            {
                onDotdmg = false;
                doteffect.GetComponent<Animator>().SetBool("dotdmgEnd", true);
                yield break;
            }  // 5초 지속이며 1초마다 데미지 받는 상황일 시 정확히 5초 지난 시점에도 데미지를 받도록 함. 즉 총 5회의 데미지
            yield return delay;
        }
        onDotdmg = false;
        doteffect.GetComponent<Animator>().SetBool("dotdmgEnd", true);
    }
}