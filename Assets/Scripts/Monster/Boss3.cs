using System.Collections;
using UnityEngine;

public class Boss3 : RevivalZombie
{
    protected override void Init()
    {
        polygonCollider2D.enabled = false;
        
        base.Init();

        Monstertype = MonsterType.RevivalZombie;
        sound = SoundManager.Instance.ZombieClip(Monstertype);
        attackeffect = transform.GetChild(0).GetChild(2).GetComponent<Animator>();
    }
    
    // 스킬 수행
    protected override IEnumerator SkillCasting1()
    {
        swingRange = 2f;
        
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
                SoundPlay(Random.Range(0, 2));
                animator.SetTrigger("Skill_Normal");
                attackeffect.SetTrigger("NormalSlash");
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