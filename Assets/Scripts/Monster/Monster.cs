using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

// 행동 목록
public enum ActionList
{
    Wandering, // 방황 중
    SkillCasting1, // 스킬1 캐스팅 중
    SkillCasting2, // 스킬2 캐스팅 중
    OnDamaging, // 피격 중
}

public class Monster : MonoBehaviour
{
    public GameObject hpBar; // 체력바
    public GameObject hpBarPrefab; // 체력바 프리팹
    public GameObject goldTxt; // 골드 텍스트
    public GameObject canvas; // 캔버스

    public List<Dictionary<string, object>> monsterData; // 몬스터 데이터 !!고칠 코드
    public AudioClip deathSound; // 사망시 재생 소리
    public AudioClip hitSound; // 피격시 재생 소리

    protected Animator animator;
    protected AudioSource audioPlayer;
    protected Rigidbody2D rigidbody2d;
    protected CapsuleCollider2D capsuleCollider2D;

    public int id; // 몬스터 Id
    protected MonsterStat stat; // 몬스터 스텟
    protected Player player; // 플레이어

    protected float distance; // 플레이어와의 거리
    protected Vector2 direction; // 플레이어 방향
    protected Vector2 randomDirection; // 랜덤 방향
    protected float attackCoolTime = 0.5f; // 공격 쿨타임
    protected float lastAttackTime; // 마지막 공격 시점
    protected float randomDirectionCoolTime = 3f; // 랜덤 경로 업데이트 쿨타임
    protected float lastRandomDirectionUpdate; // 마지막 랜덤 방향 업데이트 시점
    protected float knockBackForce;
    protected Vector2 knockBackDirection;

    public bool isDead; // 사망 여부
    protected bool actionChanged; // 행동 변경 여부
    protected bool actionFinished;
    private ActionList action; // 현재 행동
    protected Coroutine currentActionCoroutine; // 현재 행동 코루틴

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

    public event Action onDie; // 사망 시 발동 이벤트
    public event Action onRevive; // 부활 시 발동 이벤트

    protected virtual void Awake()
    {
        // 컴포넌트 초기화
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        audioPlayer = GetComponent<AudioSource>();
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        
        // 몬스터 HP바 생성
        hpBarPrefab = Resources.Load<GameObject>("Prefabs/UI/MonsterHp");
        goldTxt = Resources.Load<GameObject>("Prefabs/UI/CoinTxt");
        canvas = GameObject.FindGameObjectWithTag("HPCanvas");
    }

    protected void Start()
    {
        stat = new MonsterStat(monsterData, id); // !! 고칠 코드
        Generate(); // 몬스터 생성
    }

    // 몬스터 활성화
    protected virtual void Generate()
    {
        capsuleCollider2D.enabled = true;
        isDead = false;
        actionChanged = true;
        actionFinished = true;
        Action = ActionList.Wandering;

        hpBar = Instantiate(hpBarPrefab, canvas.transform); // 수정중
        hpBar.GetComponent<MonsterHPbar>().CreateHPbar(stat,this);
        StartCoroutine(UpdatePath());
    }

    // 경로 갱신
    protected IEnumerator UpdatePath()
    {
        while (!isDead)
        {
            if (player != null && !player.dead)
            {
                distance = Vector2.Distance(player.transform.position, transform.position);
                direction = (player.transform.position - transform.position).normalized;
            }
            else
            {
                player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
                distance = Vector2.Distance(player.transform.position, transform.position);
                direction = (player.transform.position - transform.position).normalized;
                StartCoroutine(CheckAction());
            }

            animator.SetBool("HasTarget", true);
            yield return new WaitForSeconds(0.05f);
        }
    }

    protected IEnumerator CheckAction()
    {
        while(!isDead)
        {
            //Debug.Log(Action + " "+ currentActionCoroutine);

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
    public void OnDamage(float damage, float _knockBackForce, Vector2 _knockBackDirection)
    {
        Debug.Log($"id {this.name} got damage {damage}");
        stat.OnDamage(damage);


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

            //audioPlayer.PlayOneShot(hitSound);
        }

        //Debug.Log("Monster Health: " + stat.health);

    }

    // 사망 시 실행
    public virtual void Die()
    {
        capsuleCollider2D.enabled = false;
        isDead = true;
        player = null;

        DropGold();
        
        onDie();
        animator.SetTrigger("Die");
        //audioPlayer.PlayOneShot(deathSound);
    }

    // 소지금에 골드 추가
    public void DropGold()
    {
        GameManager.Instance.Player.Inventory.UpdateGold(stat.gold);
        //UI 골드 추가
        if (stat.gold != 0)
        {
            GameObject temp = Instantiate(goldTxt, canvas.transform);
            temp.transform.position = GameManager.Instance.Player.transform.position + Vector3.up * 0.5f;
            temp.GetComponent<TextMeshProUGUI>().text = $"+{stat.gold}G";
        }
    }

    // 부활 시 실행
    public void Revive()
    {  
        stat.Revive();
        Generate();

        onRevive();
        animator.SetTrigger("Revive");
    }

    // 시야 방향 갱신
    protected void UpdateEyes()
    {
        if (rigidbody2d.velocity.x > 0)
        {
            transform.localScale = new Vector3(-stat.scale, stat.scale, 1);
        }
        else if (rigidbody2d.velocity.x < 0)
        {
            transform.localScale = new Vector3(stat.scale, stat.scale, 1);
        }
    }

    protected void OnCollisionStay2D(Collision2D other)
    {
        Player attackTarget = other.gameObject.GetComponent<Player>();

        // 충돌한 게임 오브젝트가 추적 대상이라면 공격
        if (attackTarget != player)
        {
            randomDirection = new Vector2(UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f));
            lastRandomDirectionUpdate = Time.time;
        }
        else if (Time.time >= lastAttackTime + attackCoolTime)
        {
            lastAttackTime = Time.time;
            attackTarget.OnDamage(stat.damage, 5f, (attackTarget.transform.position - transform.position).normalized);
            animator.SetTrigger("Attack_Normal");
        }
    }
}