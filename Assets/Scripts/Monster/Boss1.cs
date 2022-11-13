using System.Collections;
using UnityEngine;

public class Boss1 : RushZombie
{
    int rushStep = 0;
    // 스킬 수행
    protected override IEnumerator SkillCasting1()
    {
        bool rushing = true;
        bool rushReady = true;
        while (!isDead && Action == ActionList.SkillCasting1 && rushing)
        {
            if (Time.time < lastRushTime + timeForRushReady) // 대기
            {
                rushReady = true;
                rigidbody2d.velocity = Vector2.zero;
            }
            else if (rushReady == true) // 돌진
            {
                rigidbody2d.AddForce(direction * 300f);
                rushReady = false;
                animator.SetTrigger("Attack_Normal");
            }
            else if (Time.time >= lastRushTime + timeForRushReady + 0.7f) // 돌진 종료
            {
                lastRushTime = Time.time;
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

            UpdateEyes();
            yield return new WaitForSeconds(0.05f);
        }

        actionFinished = true;
    }
}