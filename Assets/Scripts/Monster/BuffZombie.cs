using System.Collections;
using UnityEngine;

public class BuffZombie : Monster
{
    protected float skillCoolTime = 5f; // 스킬 대기시간
    protected float lastSkillTime; // 스킬 시작시간

    protected override void Init()
    {
        base.Init();
        lastSkillTime = Time.time;
        
        Monstertype = MonsterType.RushZombie;
        sound = SoundManager.Instance.ZombieClip(Monstertype);
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
    protected void Skill()
    {
        int cnt = spawner.aliveMonsters.Count;
        
        for (int i = 0; i < cnt; i++)
        {
            Monster monster = spawner.aliveMonsters[i];
            
            StartCoroutine(SpeedUp(monster));
        }

        animator.SetTrigger("Skill_Magic");
    }

    public IEnumerator SpeedUp(Monster monster)
    {
        monster.stat.speed += 0.8f;
        yield return new WaitForSeconds(2f);
        monster.stat.speed -= 0.8f;
    }
}