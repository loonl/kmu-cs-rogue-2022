using System.Collections;
using UnityEngine;

public class Boss1 : RushZombie
{
    protected override void Init()
    {
        base.Init();
        rushCoolTime = 3f;
        lastRushTime = Time.time;
        
        Monstertype = MonsterType.RushZombie;
        Sound = SoundManager.Instance.ZombieClip(Monstertype);
    }
    
    // 스킬 수행
    protected override IEnumerator SkillCasting1()
    {
        int rushStep = 0;
        bool rushReady = true;
        
        while (!isDead && Action == ActionList.SkillCasting1 && rushStep <=3)
        {
            if (Time.time < lastRushTime + timeForRushReady) // 대기
            {
                rigidbody2d.velocity = Vector2.zero;
            }
            else if (rushReady) // 돌진
            {
                rigidbody2d.AddForce(direction * 300f);
                rushReady = false;
                animator.SetTrigger("Attack_Normal");
            }
            else if (Time.time >= lastRushTime + timeForRushReady + 0.7f) // 돌진 종료
            {
                rushReady = true;
                lastRushTime = Time.time;
                rushStep++;
                timeForRushReady = 0.1f;
            }
            
            UpdateEyes();
            yield return new WaitForSeconds(0.05f);
        }

        timeForRushReady = 1f;
        actionFinished = true;
    }
}