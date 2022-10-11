using System.Collections;
using UnityEngine;

public class RushZombie : Monster
{
    CircleCollider2D circleCollider2D;

    bool rushing = false; // 돌진 중 여부
    protected float rushCoolTime = 5f; // 돌진 쿨타임
    protected float lastRushTime = -4f; // 마지막 돌진 시점
    protected float timeForRushReady = 1f; // 돌진 준비시간

    protected override void Awake()
    {
        // 컴포넌트 초기화
        base.Awake();
        circleCollider2D = GetComponentInChildren<CircleCollider2D>();
    }

    // 스킬 수행
    protected override IEnumerator SkillCasting2()
    {
        bool rushing = true;
        bool rushReady = true;

        while (Action == ActionList.SkillCasting2 && rushing)
        {
            if (Time.time < lastRushTime + timeForRushReady) // 대기
            {
                rigidbody2d.velocity = Vector2.zero;
            }
            else if (rushReady == true) // 돌진
            {
                rigidbody2d.AddForce(direction * 300f);
                rushReady = false;
                animator.SetTrigger("Attack_Normal");
            }
            else if (Time.time >= lastRushTime + timeForRushReady + 0.5f) // 돌진 종료
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
                Action = ActionList.SkillCasting2;
            }
        }
    }
}
