using System.Collections;
using UnityEngine;

public class Boss3 : RevivalZombie
{
    protected override void Init()
    {
        polygonCollider2D.enabled = false;
        
        base.Init();
        swingRange = 2f;
        
        Monstertype = MonsterType.RevivalZombie;
        Sound = SoundManager.Instance.ZombieClip(Monstertype);
    }
    
    // 스킬 수행
    protected override IEnumerator SkillCasting1()
    {
        int swingStep = 0;
        bool swingReady = true;
        UpdateEyes();
        
        while (!isDead && Action == ActionList.SkillCasting1 && swingStep <= 3)
        {
            if (swingStep % 2 == 0)
            {
                rigidbody2d.velocity = new Vector2(0.01f, 0.01f);
            }
            else
            {
                rigidbody2d.velocity = new Vector2(-0.01f, -0.01f);
            }
            
            if (Time.time > lastSwingTime + timeForSwingReady && swingReady) // 스윙
            {
                print(swingStep);
                UpdateEyes();
                SoundPlay(Sound[0]);
                animator.SetTrigger("Skill_Normal");
                StartCoroutine(EnablepolygonCollider2D());
                swingReady = false;
            }
            else if (Time.time >= lastSwingTime + timeForSwingReady + 0.5f) // 스윙 종료
            {
                swingReady = true;
                lastSwingTime = Time.time;
                swingStep++;
                timeForSwingReady = 0.5f;
            }

            yield return new WaitForSeconds(0.05f);
        }

        timeForSwingReady = 1f;
        actionFinished = true;
    }
}