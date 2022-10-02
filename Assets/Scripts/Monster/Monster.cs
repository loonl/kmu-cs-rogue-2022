using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    protected Animator animator;
    protected AudioSource audioPlayer;
    protected Rigidbody2D rigidbody2d;
    protected CapsuleCollider2D capsuleCollider2d;
    protected CircleCollider2D circleCollider2d;

    public List<Dictionary<string, object>> monsterData; // 몬스터 데이터 !!고칠 코드
    public AudioClip deathSound; // 사망시 재생 소리
    public AudioClip hitSound; // 피격시 재생 소리

    public int id; // 몬스터 Id
    protected MonsterStat stat; // 몬스터 스텟
    protected Coroutine onDamageCoroutine; // 현재 onDamage 코루틴
    protected Player player; // 플레이어
    protected float distance; // 플레이어와의 거리
    protected Vector2 direction; // 플레이어 방향
    protected Vector2 randomDirection; // 랜덤 방향

    protected float sight = 2f; //시야 범위
    protected float attackCoolTime = 0.5f; // 공격 쿨타임
    protected float lastAttackTime; // 마지막 공격 시점
    protected float randomDirectionCoolTime = 3f; // 랜덤 경로 업데이트 쿨타임
    protected float lastRandomDirectionUpdate;

    public event System.Action onDie; // 사망 시 발동 이벤트
    public event System.Action onRevive; // 부활 시 발동 이벤트

    public bool isDead; // 사망 여부
    public Action action { get; protected set; } // 현재 행동
    // 행동 목록
    public enum Action
    {
        Standing, // 휴식 중
        //Wandering, // 방황 중
        Tracing, // 추적 중
        SkillCasting, // 스킬 캐스팅 중
        OnDamaging, // 피격 중
    }

    protected void Awake()
    {
        // 컴포넌트 초기화
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        audioPlayer = GetComponent<AudioSource>();
        capsuleCollider2d = GetComponent<CapsuleCollider2D>();
        circleCollider2d = GetComponent<CircleCollider2D>();
    }

    protected void Start()
    {
        stat = new MonsterStat(monsterData, id); // !! 고칠 코드
        Generate(); // 몬스터 생성
    }

    // 몬스터 활성화
    protected virtual void Generate()
    {
        isDead = false;
        capsuleCollider2d.enabled = true;
        circleCollider2d.enabled = true;
        StartCoroutine(UpdatePath());
    }

    // 경로 갱신
    protected IEnumerator UpdatePath()
    {
        StartCoroutine(Standing());

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
                action = Action.Tracing;
                StartCoroutine(Tracing());
            }

            animator.SetBool("HasTarget", true);
            yield return new WaitForSeconds(0.05f);
        }
    }

    // 시야 방향 갱신
    protected void UpdateEyes(float x)
    {
        if (x > 0)
        {
            transform.localScale = new Vector3(-stat.scale, stat.scale, 1);
        }
        else
        {
            transform.localScale = new Vector3(stat.scale, stat.scale, 1);
        }
    }

    // 휴식 수행
    protected IEnumerator Standing()
    {
        while (!isDead && action == Action.Standing)
        {
            rigidbody2d.velocity = Vector2.zero;

            yield return new WaitForSeconds(0.05f);
        }
    }

    // 추적 수행
    protected virtual IEnumerator Tracing()
    {      
        lastRandomDirectionUpdate = Time.time;
        randomDirection = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1.0f, 1.0f));

        float randomDirectionCoolTime = UnityEngine.Random.Range(1f, 3f);
        while (player != null && !player.dead && !isDead && action == Action.Tracing)
        {
            if (distance >= sight)
            {
                if (Time.time >= lastRandomDirectionUpdate + randomDirectionCoolTime)
                {
                    randomDirection = new Vector2(UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f));
                    lastRandomDirectionUpdate = Time.time;
                }
                stat.ChangeSpeed(1);
                rigidbody2d.velocity = randomDirection.normalized * stat.speed;
                UpdateEyes(randomDirection.x);
            }
            else
            {
                stat.ChangeSpeed(2);
                rigidbody2d.velocity = direction * stat.speed;
                UpdateEyes(direction.x);
            }

            yield return new WaitForSeconds(0.05f);
        }
    }

    // 피격 수행
    protected virtual IEnumerator OnDamaging(float knockBackForce, Vector2 diff)
    {
        rigidbody2d.velocity = Vector2.zero;
        rigidbody2d.AddForce(diff * knockBackForce, ForceMode2D.Impulse);
        Vector2 startWay = rigidbody2d.velocity;

        while ((startWay.x > 0 && rigidbody2d.velocity.x > 0) || (startWay.x < 0 && rigidbody2d.velocity.x < 0)
            && (startWay.y > 0 && rigidbody2d.velocity.y > 0) || (startWay.y < 0 && rigidbody2d.velocity.y < 0))
        {
            rigidbody2d.AddForce(-diff * knockBackForce * 7f, ForceMode2D.Force);
            yield return new WaitForSeconds(0.05f);
        }

        rigidbody2d.velocity = Vector2.zero;
        action = Action.Tracing;
        StartCoroutine(Tracing());
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
    public void OnDamage(float damage, float knockBackForce, Vector2 diff)
    {

        stat.OnDamage(damage);

        if (stat.health <= 0)
        {
            Die();
        }
        else
        {
            //audioPlayer.PlayOneShot(hitSound);
        }

        if (!isDead && action != Action.SkillCasting)
        {
            action = Action.OnDamaging;
            if (onDamageCoroutine != null)
            {
                StopCoroutine(onDamageCoroutine);
            }
            onDamageCoroutine = StartCoroutine(OnDamaging(knockBackForce, diff));
        }

        Debug.Log("Monster Health: " + stat.health);
    }

    // 사망 시 실행
    public virtual void Die()
    {
        capsuleCollider2d.enabled = false;
        circleCollider2d.enabled = false;
        DropGold();

        isDead = true;
        StartCoroutine(Dying());

        onDie();
        animator.SetTrigger("Die");
        //audioPlayer.PlayOneShot(deathSound);
    }

    // 소지금에 골드 추가
    public void DropGold()
    {
        GameManager.Instance.Player.Inventory.UpdateGold(stat.gold);
    }

    // 부활 시 실행
    public void Revive()
    {  
        stat.Revive();
        Generate();

        onRevive();
        animator.SetTrigger("Revive");
    }

    protected void OnCollisionStay2D(Collision2D other)
    {
        Player attackTarget = other.gameObject.GetComponent<Player>();

        // 충돌한 게임 오브젝트가 추적 대상이라면 공격
        if (attackTarget != player)
        {
            Player attackTarget = other.gameObject.GetComponent<Player>();

            if (attackTarget == player && !player.isInvincible)
            {
                lastAttackTime = Time.time;
                attackTarget.OnDamage(damage, 5f, 
                    (other.gameObject.transform.position - transform.position).normalized);
                animator.SetTrigger("Attack_Normal");
            }
            randomDirection = new Vector2(UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f));
            lastRandomDirectionUpdate = Time.time;
        }
    }
}