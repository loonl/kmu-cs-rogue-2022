using System.Collections;
using UnityEngine;

public class RushZombie : Monster
{
    CircleCollider2D circleCollider2D;

    protected float rushCoolTime = 2f; // 돌진 쿨타임
    protected float lastRushTime = 0f; // 마지막 돌진 시점
    protected float timeForRushReady = 1f; // 돌진 준비시간
    protected float rushPower = 300f; // 돌진 파워

    protected override void Awake()
    {
        // 컴포넌트 초기화
        base.Awake();
        
        circleCollider2D = GetComponentInChildren<CircleCollider2D>();
    }

    protected override void Init()
    {
        base.Init();
        lastRushTime = Time.time;
        
        Monstertype = MonsterType.RushZombie;
        sound = SoundManager.Instance.ZombieClip(Monstertype);
    }


    // 스킬1 수행
    protected override IEnumerator SkillCasting1()
    {
        int rushStep = 0;
        bool rushReady = true;

        while (!isDead && Action == ActionList.SkillCasting1 && rushStep == 0)
        {
            if (Time.time < lastRushTime + timeForRushReady) // 대기
            {
                rigidbody2d.velocity = Vector2.zero;
            }
            else if (rushReady) // 돌진
            {
                SoundPlay(Random.Range(0, 2));
                rigidbody2d.AddForce(direction * rushPower);
                rushReady = false;
                animator.SetTrigger("Attack_Normal");
            }
            else if (Time.time >= lastRushTime + timeForRushReady + 0.5f) // 돌진 종료
            {
                rushStep++;
            }

            yield return new WaitForSeconds(0.05f);
        }

        UpdateEyes();
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
}
