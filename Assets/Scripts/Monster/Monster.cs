using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

// 행동 목록
public enum ActionList
{
    Wandering, // 방황 중
    SkillCasting1, // 스킬1 캐스팅 중
    SkillCasting2, // 스킬2 캐스팅 중
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

    public List<Dictionary<string, object>> monsterData; // 몬스터 데이터 !!고칠 코드
    // 0 공격 혹은 부활 시 재생소리(일반좀비는 null) 1 피격시 재생 소리 2 사망시 재생 소리
    public AudioClip[] Sound;

    protected Animator animator;
    protected AudioSource audioPlayer;
    protected Rigidbody2D rigidbody2d;
    protected CapsuleCollider2D capsuleCollider2D;

    public int id; // 몬스터 Id
    protected MonsterStat stat; // 몬스터 스텟
    protected Player player; // 플레이어
    protected MonsterSpawner spawner; // 부모 스포너 객체

    protected float distance; // 플레이어와의 거리
    protected Vector2 direction; // 플레이어 방향
    protected Vector2 randomDirection; // 랜덤 방향
    protected float attackCoolTime = 0.5f; // 공격 쿨타임
    protected float lastAttackTime; // 마지막 공격 시점
    protected float randomDirectionCoolTime = 3f; // 랜덤 경로 업데이트 쿨타임
    protected float lastRandomDirectionUpdate; // 마지막 랜덤 방향 업데이트 시점
    protected float knockBackForce; // 넉백 힘
    protected Vector2 knockBackDirection; // 넉백 방향
    protected MonsterType Monstertype;

    public bool isDead; // 사망 여부
    protected bool actionChanged; // 행동 변경 여부
    protected bool actionFinished; // 행동 종료 여부
    protected Coroutine currentActionCoroutine; // 현재 행동 코루틴
    public bool isInvulnerable; // 무적 여부
    public bool onDotdmg; // 도트 데미지 받고있는 여부
    private ActionList action; // 현재 행동
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

    // 몬스터 초기화
    protected virtual void Init()
    {
        stat = new MonsterStat(monsterData, id); // !! 고칠 코드
        Generate(); // 몬스터 생성
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        spawner = transform.GetComponentInParent<MonsterSpawner>();
        Sound = new AudioClip[3];
    }

    // 몬스터 활성화
    protected virtual void Generate()
    {
        capsuleCollider2D.enabled = true;
        isDead = false;
        actionChanged = true;
        actionFinished = true;
        isInvulnerable = false;
        Action = ActionList.Wandering;
        
        canvas = Instantiate(canvas, transform.position, Quaternion.identity);
        canvas.transform.SetParent(transform);
        canvas.transform.localPosition = new Vector3(0, 1f, 0);
        canvas.transform.localScale = new Vector3(1, 1, 1);
        hpBar = Instantiate(hpBarPrefab, transform.position, Quaternion.identity);
        hpBar.transform.SetParent(canvas.transform);
        hpBar.transform.localPosition = new Vector3(0, 0, 0);
        hpBar.transform.localScale = new Vector3(0.01f, 0.01f,0);
        hpBar.SetActive(false);

        Sound = new AudioClip[3];

        StartCoroutine(UpdatePath());
    }

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
            else
            {
                //player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
                //distance = Vector2.Distance(player.transform.position, transform.position);
                //direction = (player.transform.position - transform.position).normalized;
                //StartCoroutine(CheckAction());
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
                    case ActionList.SkillCasting1:
                        currentActionCoroutine = StartCoroutine(SkillCasting1());
                        break;
                    case ActionList.SkillCasting2:
                        currentActionCoroutine = StartCoroutine(SkillCasting2());
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
                    Action = ActionList.Wandering;
                }
            }

            yield return new WaitForSeconds(0.05f);
        }

        StopCoroutine(currentActionCoroutine);
        currentActionCoroutine = StartCoroutine(Dying());
    }

    // 추적 수행
    protected virtual IEnumerator Wandering()
    {
        stat.ChangeSpeed(1);
        lastRandomDirectionUpdate = Time.time;
        randomDirection = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1.0f, 1.0f)).normalized;

        float randomDirectionCoolTime = UnityEngine.Random.Range(1f, 3f);
        while (!isDead && Action == ActionList.Wandering)
        {
            if (distance < stat.sight)
            {
                Action = ActionList.SkillCasting1;
            }
            else if (Time.time >= lastRandomDirectionUpdate + randomDirectionCoolTime)
            {
                randomDirection = new Vector2(UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f));
                lastRandomDirectionUpdate = Time.time;
            }

            rigidbody2d.velocity = randomDirection * stat.speed;
            UpdateEyes();
            yield return new WaitForSeconds(0.05f);
        }

        actionFinished = true;
    }

    // 스킬1 수행
    protected virtual IEnumerator SkillCasting1() 
    {
        stat.ChangeSpeed(2);

        while (!isDead && Action == ActionList.SkillCasting1 && distance < stat.sight)
        {
            rigidbody2d.velocity = direction * stat.speed;
            UpdateEyes();
            yield return new WaitForSeconds(0.05f);
        }

        actionFinished = true;
    }

    // 스킬2 수행
    protected virtual IEnumerator SkillCasting2()
    {
        while (!isDead && Action == ActionList.SkillCasting2)
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
            rigidbody2d.AddForce(-knockBackDirection * knockBackForce * 12f, ForceMode2D.Force);
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

    // 피격 시 실행
    public void OnDamage(float damage, float _knockBackForce, Vector2 _knockBackDirection, WaitForSeconds invulnerabletime = null)
    {
        stat.OnDamage(damage);

        
        hpBar.GetComponent<Slider>().value = stat.health / stat.maxHealth;
        if (stat.health<stat.maxHealth)
        {
            hpBar.SetActive(true);
        }

        if(invulnerabletime != null) StartCoroutine(SetInvulnerable(invulnerabletime));


        if (stat.health <= 0)
        {
            Die();
        }
        else
        {
            if (true) // !! 스킬시전 중 스턴가능 여부 추가
            {
                if (Action == ActionList.OnDamaging)
                {
                    StopCoroutine(currentActionCoroutine);
                }

                knockBackForce = _knockBackForce;
                knockBackDirection = _knockBackDirection;
                Action = ActionList.OnDamaging;
            }

            SoundPlay(Sound[1]);
        }
    }

    // 사망 시 실행
    public virtual void Die()
    {
        capsuleCollider2D.enabled = false;
        isDead = true;

        spawner.monsters.Remove(this);
        spawner.deadMonsters.Add(this);
        spawner.CheckRemainEnemy();
        DropGold();

        
        hpBar.SetActive(false);
        
        onDie();

        animator.SetTrigger("Die");
        SoundPlay(Sound[2]);
    }

    // 소지금에 골드 추가
    public void DropGold()
    {
        GameManager.Instance.Player.Inventory.UpdateGold(stat.gold);

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

    // 부활 시 실행
    public virtual void Revive()
    {
        stat.Revive();
        Generate();

        animator.SetTrigger("Revive");
        SoundPlay(Sound[0]);
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
            hpBar.transform.localScale = new Vector3(0.01f, 0.01f, 1);
            transform.localScale = new Vector3(stat.scale, stat.scale, 1);
        }
    }

    protected void OnCollisionStay2D(Collision2D other)
    {
        Player attackTarget = null;
        try
        {
            attackTarget = other.gameObject.GetComponent<Player>();
        }
        catch (NullReferenceException)
        {
            return; 
        }

        // 충돌한 게임 오브젝트가 추적 대상이라면 공격
        if (attackTarget != player)
        {
            randomDirection = new Vector2(UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f));
            lastRandomDirectionUpdate = Time.time;
        }
        else if (Time.time >= lastAttackTime + attackCoolTime && attackTarget != null)
        {
            lastAttackTime = Time.time;
            attackTarget.OnDamage(stat.damage, 5f, (attackTarget.transform.position - transform.position).normalized);
            animator.SetTrigger("Attack_Normal");
        }
    }

    protected void SoundPlay(AudioClip clip)
    {
        if (audioPlayer.isPlaying)
            audioPlayer.Stop();

        audioPlayer.clip = clip;
        audioPlayer.volume = SoundManager.Instance.effectvolume * SoundManager.Instance.totalvolume;
        audioPlayer.Play();
    }

    private IEnumerator SetInvulnerable(WaitForSeconds timer)
    {
        isInvulnerable = true;
        yield return timer;
        isInvulnerable = false;
    }

    public void SetDotDmg(float prob, float dmg, float delay, float duration, string effectname = "") // 도트데미지 set, prob: 걸릴 확률 (0<prob<1)
    {
        if (onDotdmg) StopCoroutine("DoDotDmg");
        if (UnityEngine.Random.Range(0.0f, 1.0f) <= prob) StartCoroutine(DoDotDmg(dmg, delay, GameManager.Instance.Setwfs((int)(delay * 100)), duration));
        onDotdmg = true;
    }

    private IEnumerator DoDotDmg(float dmg, float delayf, WaitForSeconds delay, float duration) // 도트데미지 적용
    {
        if (isDead) { onDotdmg = false; yield break; } // 다음 도트데미지 받기 전에 플레이어의 공격으로 죽었을 수도 있음.
        OnDamage(dmg, 0f, Vector2.zero);
        duration -= delayf;
        if (duration < 0f || isDead) { onDotdmg = false; yield break; }  // 5초 지속이며 1초마다 데미지 받는 상황일 시 정확히 5초 지난 시점에도 데미지를 받도록 함. 즉 총 5회의 데미지
        yield return delay;
        StartCoroutine(DoDotDmg(dmg, delayf, delay, duration));
    }
}