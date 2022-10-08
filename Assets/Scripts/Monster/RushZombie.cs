using System.Collections;
using UnityEngine;

public class RushZombie : Monster
{
    bool rushing = false; // 돌진 중 여부
    protected float rushCoolTime = 5f; // 돌진 쿨타임
    protected float lastRushTime = -4f; // 마지막 돌진 시점
    protected float timeForRushReady = 1f; // 돌진 준비시간

    // 추적 수행
    protected override IEnumerator Tracing()
    {
        lastRandomDirectionUpdate = Time.time;
        randomDirection = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1.0f, 1.0f)).normalized;

        float randomDirectionCoolTime = UnityEngine.Random.Range(1f, 3f);
        while (player != null && !player.dead && !isDead && action == Action.Tracing)
        {
            if (distance < sight)
            {
                stat.ChangeSpeed(2);
                rigidbody2d.velocity = direction * stat.speed;
                UpdateEyes(direction.x);
            } 
            else
            {
                if (Time.time >= lastRandomDirectionUpdate + randomDirectionCoolTime)
                {
                    randomDirection = new Vector2(UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f));
                    lastRandomDirectionUpdate = Time.time;
                }
                stat.ChangeSpeed(1);
                rigidbody2d.velocity = randomDirection * stat.speed;
                UpdateEyes(randomDirection.x);
            }

            yield return new WaitForSeconds(0.05f);
        }
    }

    // 스킬 수행
    protected virtual IEnumerator SkillCasting()
    {
        bool rushing = true;
        bool rushReady = true;

        while (action == Action.SkillCasting && rushing)
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

            yield return new WaitForSeconds(0.05f);
        }

        rigidbody2d.velocity = Vector2.zero;
        action = Action.Tracing;
        StartCoroutine(Tracing());
    }

    private void OnTriggerStay2D(Collider2D other)
    {

        // 목표가 가까워지면 돌진
        if (Time.time >= lastRushTime + rushCoolTime && rushing == false)
        {
            Player attackTarget = other.gameObject.GetComponent<Player>();
            Debug.Log("a");
            if (player == attackTarget)
            {
                lastRushTime = Time.time;
                action = Action.SkillCasting;
                StartCoroutine(SkillCasting());
            }
        }
    }
}
