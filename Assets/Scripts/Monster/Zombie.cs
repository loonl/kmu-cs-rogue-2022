using System.Collections;
using UnityEngine;

public class Zombie : Monster
{
    CircleCollider2D circleCollider2D;

    
    protected float rushCoolTime = 3f; // 돌진 쿨타임
    protected float lastRushTime; // 마지막 돌진 시점
    protected float timeForRushReady = 0.5f; // 돌진 준비시간
    protected float rushPower = 100f; // 돌진 파워

    protected override void Awake()
    {

        base.Awake();
        
        circleCollider2D = GetComponentInChildren<CircleCollider2D>();
    }

    protected override void Init()
    {
        base.Init();
        lastRushTime = Time.time;
        
        Monstertype = MonsterType.Zombie;
        Sound = SoundManager.Instance.ZombieClip(Monstertype);
    }

    // 스킬 수행
    protected override IEnumerator SkillCasting1()
    {
        bool rushing = true;
        bool rushReady = true;

        while (!isDead && Action == ActionList.SkillCasting1 && rushing)
        {
            if (Time.time < lastRushTime + timeForRushReady) // 대기
            {
                UpdateEyes();
                rigidbody2d.velocity = Vector2.zero;
            }
            else if (rushReady) // 돌진
            {
                SoundPlay(Sound[0]);
                rigidbody2d.AddForce(direction * rushPower);
                rushReady = false;
                animator.SetTrigger("Attack_Normal");
            }
            else if (Time.time >= lastRushTime + timeForRushReady + 0.2f) // 돌진 종료
            {
                rushing = false;
            }
            
            yield return new WaitForSeconds(0.05f);
        }

        actionFinished = true;
    }

    protected void OnTriggerStay2D(Collider2D other)
    {
        // 목표가 가까워지면 돌진
        if (other.gameObject.CompareTag("Player"))
        {
            if (Time.time >= lastRushTime + rushCoolTime)
            {
                lastRushTime = Time.time;
                Action = ActionList.SkillCasting1;
            }
        }
    }
    
    // 스킬 수행
    protected override IEnumerator SkillCasting1()
    {
        bool rushing = true;
        bool rushReady = true;

        while (Action == ActionList.SkillCasting1 && rushing)
        {
            if (Time.time < lastRushTime + timeForRushReady) // 대기
            {
                rigidbody2d.velocity = Vector2.zero;
            }
            else if (rushReady) // 돌진
            {
                SoundPlay(Sound[0]);
                rigidbody2d.AddForce(direction * rushPower);
                rushReady = false;
                animator.SetTrigger("Attack_Normal");
            }
            else if (Time.time >= lastRushTime + timeForRushReady + 0.2f) // 돌진 종료
            {
                rushing = false;
            }

            UpdateEyes();
            yield return new WaitForSeconds(0.05f);
        }

        actionFinished = true;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // 목표가 가까워지면 돌진
        if (Time.time >= lastRushTime + rushCoolTime && rushing == false)
        {
            Player attackTarget = other.gameObject.GetComponent<Player>();
            if (player == attackTarget)
            {
                lastRushTime = Time.time;
                Action = ActionList.SkillCasting1;
            }
        }
    }
}
