using System.Collections;
using UnityEngine;

public class RevivalZombie : Monster
{
    protected bool revived = false;
    protected float timeBetRevive = 3f; // 부활 대기시간
    protected float startReviveTime; // 부활 시작시간

    protected override void Init()
    {
        base.Init();
        Monstertype = MonsterType.RevivalZombie;
        Sound = SoundManager.Instance.ZombieClip(Monstertype);
    }

    private void Start()
    {
        Init();
    }
    // 사망 시 실행
    public override void Die()
    {
        if (stat.health > 0)
            revived = true;
            
        capsuleCollider2D.enabled = false;
        isDead = true;

        if (revived)
        {
            spawner.monsters.Remove(this);
            spawner.deadMonsters.Add(this);
            spawner.CheckRemainEnemy();
            DropGold();
        }

        animator.SetTrigger("Die");
        SoundPlay(Sound[2]);
    }

    // 시체 상태 수행 후 부활 처리
    protected override IEnumerator Dying()
    {
        startReviveTime = Time.time;

        while (isDead)
        {
            rigidbody2d.velocity = Vector2.zero;

            if (Time.time >= startReviveTime + timeBetRevive && !revived)
            {
                revived = true;
                Revive();
            }

            yield return new WaitForSeconds(0.05f);
        }
    }
}
