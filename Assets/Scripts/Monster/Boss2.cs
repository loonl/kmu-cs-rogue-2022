using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2 : Monster
{

    protected float skillCoolTime = 5f; // 스킬 대기시간
    protected float lastSkillTime; // 스킬 시작시간

    protected override void Init()
    {
        base.Init();
        lastSkillTime = Time.time;

        Monstertype = MonsterType.RushZombie;
        Sound = SoundManager.Instance.ZombieClip(Monstertype);
    }

    protected override IEnumerator Chasing() 
    {
        while (!isDead && Action == ActionList.Chasing)
        {
            rigidbody2d.velocity = direction * stat.speed;
            UpdateEyes();
            if (Time.time >= lastSkillTime + skillCoolTime)
            {
                lastSkillTime = Time.time;
                Skill();
            }
            
            yield return new WaitForSeconds(0.05f);
        }
    }

    
    // 스킬1 수행
    public virtual void Skill()
    {
        int cnt = spawner.deadMonsters.Count;
        
        for (int i = 0; i < cnt; i++)
        {
            Monster monster = spawner.deadMonsters[0];
            
            monster.Generate();
        
            spawner.monsters.Add(monster);
            spawner.deadMonsters.Remove(monster);

            monster.animator.SetTrigger("Revive");
            monster.SoundPlay(Sound[0]);
        }

        animator.SetTrigger("Skill_Magic");
    }
}
