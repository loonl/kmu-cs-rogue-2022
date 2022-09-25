using System.Collections;
using UnityEngine;

public class RushZombie : Monster
{
    bool rushing = false; // 돌진 중 여부
    protected float rushCoolTime = 5f; // 돌진 쿨타임
    protected float lastRushTime = -4f; // 마지막 돌진 시점
    protected float timeForRushReady = 1f; // 돌진 준비시간

    // 스킬 수행
    protected virtual IEnumerator SkillCasting()
    {
        bool rushing = true;
        bool rushReady = true;

        while (!dead && rushing)
        {
            if (Time.time < lastRushTime + timeForRushReady) // 대기
            {
                rigidbody2d.velocity = Vector2.zero;
            }
            else if (rushReady == true) // 돌진
            {
                rigidbody2d.AddForce(direction * 300f);
                damage = 40f;
                rushReady = false;
                animator.SetTrigger("Attack_Normal");
            }
            else if (Time.time >= lastRushTime + timeForRushReady + 0.5f) // 돌진 종료
            {
                damage = 20f;
                rushing = false;
            }

            yield return new WaitForSeconds(0.05f);
        }

        action = Action.Moving;
        StartCoroutine(Moving());
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (dead)
        {
            return;
        }

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
