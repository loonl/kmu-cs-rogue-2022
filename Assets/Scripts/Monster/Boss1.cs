using System.Collections;
using UnityEngine;

public class Boss1 : RushZombie
{
    int rushStep = 0;
    // 스킬 수행
    protected override IEnumerator SkillCasting()
    {
        bool rushing = true;
        bool rushReady = true;
        while (!dead && rushing)
        {
            if (Time.time < lastRushTime + timeForRushReady) // 대기
            {
                rushReady = true;
                rigidbody2d.velocity = Vector2.zero;
            }
            else if (rushReady == true) // 돌진
            {
                rigidbody2d.AddForce(direction * 300f);
                damage = 40f;
                rushReady = false;
                animator.SetTrigger("Attack_Normal");
            }
            else if (Time.time >= lastRushTime + timeForRushReady + 0.7f) // 돌진 종료
            {
                lastRushTime = Time.time;
                damage = 20f;
                if (rushStep <= 2)
                {
                    rushStep++;
                    rushCoolTime = 0.9f;
                    timeForRushReady = 0.1f;
                }
                else
                {
                    rushStep = 0;
                    rushCoolTime = 5f;
                    timeForRushReady = 1f;
                    rushing = false;
                }
            }

            yield return new WaitForSeconds(0.05f);
        }

        action = Action.Moving;
        StartCoroutine(Moving());
    }
}