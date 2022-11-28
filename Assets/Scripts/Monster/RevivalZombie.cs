using System.Collections;
using UnityEngine;

public class RevivalZombie : Monster
{
    protected PolygonCollider2D polygonCollider2D = new PolygonCollider2D();
    protected Animator attackeffect;

    protected bool revived = false;

    protected float timeBetRevive = 1f; // 부활 대기시간
    protected float startReviveTime; // 부활 시작시간

    protected float swingRange = 1.5f; // 스윙 사정거리
    protected float swingCoolTime = 3f; // 스윙 쿨타임
    protected float lastSwingTime; // 마지막 스윙 시점
    protected float timeForSwingReady = 1f; // 스윙 준비시간

    protected override void Awake()
    {
        base.Awake();
        lastSwingTime = Time.time;
        
        polygonCollider2D = GetComponentInChildren<PolygonCollider2D>();
        attackeffect = transform.GetChild(0).GetChild(2).GetComponent<Animator>();
    }
    
    protected override void Init()
    {
        polygonCollider2D.enabled = false;
        
        base.Init();
        
        Monstertype = MonsterType.RevivalZombie;
        Sound = SoundManager.Instance.ZombieClip(Monstertype);
    }
    
    protected override IEnumerator Chasing() 
    {
        while (!isDead && Action == ActionList.Chasing)
        {
            rigidbody2d.velocity = direction * stat.speed;
            UpdateEyes();
            if (distance < swingRange && Time.time >= lastSwingTime + swingCoolTime)
            {
                lastSwingTime = Time.time;
                Action = ActionList.SkillCasting1;
            }
            
            yield return new WaitForSeconds(0.05f);
        }
    }
    
    // 스킬1 수행
    protected override IEnumerator SkillCasting1()
    {
        int swingStep = 0;
        bool swingReady = true;
        UpdateEyes();
        
        while (!isDead && Action == ActionList.SkillCasting1 && swingStep == 0)
        {
            rigidbody2d.velocity = new Vector2(0, 0.01f);
            
            if (Time.time > lastSwingTime + timeForSwingReady && swingReady) // 스윙
            {
                SoundPlay(Sound[0]);
                animator.SetTrigger("Skill_Normal");
                attackeffect.SetTrigger("NormalSlash");
                StartCoroutine(EnablepolygonCollider2D());
                swingReady = false;
            }
            else if (Time.time >= lastSwingTime + timeForSwingReady + 0.5f) // 스윙 종료
            {
                swingStep++;
            }

            yield return new WaitForSeconds(0.05f);
        }
        
        actionFinished = true;
    }
    
    public IEnumerator EnablepolygonCollider2D()
    {
        polygonCollider2D.enabled = true;
        yield return new WaitForSeconds(0.1f);
        polygonCollider2D.enabled = false;
    }
    
    // 사망 시 실행
    public override void Die()
    {
        if (stat.health > 0)
            revived = true;
            
        capsuleCollider2D.enabled = false;
        isDead = true;
                
        if (revived)
        {
            spawner.aliveMonsters.Remove(this);
            spawner.deadMonsters.Add(this);
            spawner.CheckRemainEnemy();
            DropGold();
            hpBar.gameObject.SetActive(false);
        }

        animator.SetTrigger("Die");
        SoundPlay(Sound[2]);
    }

    // 부활 시 실행
    protected void Revive()
    {
        Generate();

        animator.SetTrigger("Revive");
        SoundPlay(Sound[0]);
    }

    // 시체 상태 수행 후 부활 처리
    protected override IEnumerator Dying()
    {
        startReviveTime = Time.time;

        while (isDead)
        {
            rigidbody2d.velocity = Vector2.zero;

            if (Time.time >= startReviveTime + timeBetRevive && !revived)
            {
                revived = true;
                Revive();
            }

            yield return new WaitForSeconds(0.05f);
        }
    }
    
    protected void OnTriggerStay2D(Collider2D other)
    {
        // 충돌한 게임 오브젝트가 추적 대상이라면 공격
        if (other.gameObject.CompareTag("Player"))
        {
            player.OnDamage(stat.damage, 5f, (other.transform.position - transform.position).normalized);
            polygonCollider2D.enabled = false;
        }
    }
}
