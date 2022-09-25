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

    public List<Dictionary<string, object>> monsterData; // 몬스터 데이터
    public AudioClip deathSound; // 사망시 재생 소리
    public AudioClip hitSound; // 피격시 재생 소리

    public int idNumber; // 아이디 넘버
    protected float scale; // 크기
    protected int gold; //드랍 골드
    protected float maxHealth; // 최대 체력
    protected float health; // 현재 체력
    protected float corpseHealth; // 시체 체력
    protected float damage; // 공격력
    protected float speed; // 이동 속도
    protected float attackCoolTime = 0.5f; // 공격 쿨타임
    protected float lastAttackTime; // 마지막 공격 시점
    //protected float timeForDamaging = 0.75f;
    //protected float lastDamagedTime;


    // 행동 목록
    public enum Action
    {
        Standing, // 휴식 중
        Moving, // 이동 중
        SkillCasting, // 스킬 캐스팅 중
        OnDamaging // 피격 중
    }
    protected Action action; // 현재 상태
    protected Coroutine onDamageCoroutine = null;
    public bool dead = false; // 사망 상태
    protected Player player; // 추적 대상
    protected Vector2 direction; // 경로 방향

    public event System.Action onDie; // 사망 시 발동 이벤트
    public event System.Action onRevive; // 부활 시 발동 이벤트
    // 추적 대상의 존재 여부
    protected bool hasTarget
    {
        get
        {
            if (player != null && !player.dead)
            {
                return true;
            }

            return false;
        }
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
        SetUp(); // 몬스터 초기화
        Generate(); // 몬스터 생성
    }

    // csv파일을 이용하여 몬스터 초기화
    protected void SetUp()
    {
        scale = float.Parse(monsterData[idNumber]["Scale"].ToString());
        gold = int.Parse(monsterData[idNumber]["Gold"].ToString());
        maxHealth = float.Parse(monsterData[idNumber]["MaxHealth"].ToString());
        damage = float.Parse(monsterData[idNumber]["Damage"].ToString());
        speed = float.Parse(monsterData[idNumber]["Speed"].ToString());
    }

    // 몬스터 활성화
    protected virtual void Generate()
    {
        health = maxHealth;
        corpseHealth = maxHealth / 2;
        dead = false;
        action = Action.Moving;
        capsuleCollider2d.enabled = true;
        circleCollider2d.enabled = true;
        StartCoroutine(UpdatePath());
        StartCoroutine(Moving());
    }

    // 경로 갱신
    protected IEnumerator UpdatePath()
    {
        while (!dead)
        {
            if (hasTarget)
            {
                direction = (player.transform.position - transform.position).normalized;
            }
            else
            {
                player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            }

            animator.SetBool("HasTarget", hasTarget);
            yield return new WaitForSeconds(0.05f);
        }
    }

    // 이동 수행
    protected virtual IEnumerator Moving()
    {
        while (!dead && hasTarget && action == Action.Moving)
        {
            rigidbody2d.velocity = direction * speed;

            if (direction.x > 0)
            {
                transform.localScale = new Vector3(-scale, scale, 1);
            }
            else
            {
                transform.localScale = new Vector3(scale, scale, 1);
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
        Debug.Log(rigidbody2d.velocity.x);

        while ((startWay.x > 0 && rigidbody2d.velocity.x > 0) || (startWay.x < 0 && rigidbody2d.velocity.x < 0)
            && (startWay.y > 0 && rigidbody2d.velocity.y > 0) || (startWay.y < 0 && rigidbody2d.velocity.y < 0))
        {
            rigidbody2d.AddForce(-diff * knockBackForce * 7f, ForceMode2D.Force);
            yield return new WaitForSeconds(0.05f);
        }

        rigidbody2d.velocity = Vector2.zero;
        action = Action.Moving;
        StartCoroutine(Moving());
    }

    // 시체 수행
    protected virtual IEnumerator Dying()
    {
        while (dead)
        {
            yield return new WaitForSeconds(0.05f);
        }
    }

    // 피격 시 실행
    public void OnDamage(float damage, float knockBackForce, Vector2 diff)
    {

        health -= damage;

        if (health <= 0)
        {
            Die();
        }
        else
        {
            //audioPlayer.PlayOneShot(hitSound);
        }

        if (action == Action.Moving || action == Action.OnDamaging)
        {
            action = Action.OnDamaging;
            if (onDamageCoroutine != null)
            {
                StopCoroutine(onDamageCoroutine);
            }
            onDamageCoroutine = StartCoroutine(OnDamaging(knockBackForce, diff));
        }

        Debug.Log("Monster Health: " + health);
    }

    // 사망 시 실행
    public virtual void Die()
    {
        onDie();

        dead = true;
        health = corpseHealth;
        capsuleCollider2d.enabled = false;
        circleCollider2d.enabled = false;
        DropGold();
        StartCoroutine(Dying());

        animator.SetTrigger("Die");
        //audioPlayer.PlayOneShot(deathSound);
    }

    // 소지금에 골드 추가
    public void DropGold()
    {
        GameManager.Instance.Player.Inventory.UpdateGold(gold);
    }

    // 부활 시 실행
    public void Revive()
    {
        onRevive();

        gold = 0;
        Generate();

        animator.SetTrigger("Revive");
    }

    protected void OnCollisionStay2D(Collision2D other)
    {
        if (dead)
        {
            return;
        }

        // 충돌한 게임 오브젝트가 추적 대상이라면 공격
        if (Time.time >= lastAttackTime + attackCoolTime)
        {
            Player attackTarget = other.gameObject.GetComponent<Player>();

            if (attackTarget == player)
            {
                lastAttackTime = Time.time;
                attackTarget.OnDamage(damage);
                animator.SetTrigger("Attack_Normal");
            }
        }
    }
}
