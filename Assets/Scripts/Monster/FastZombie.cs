using UnityEngine;
using UnityEngine.UI;

public class FastZombie : Monster
{
    protected override void Init()
    {
        base.Init();
        Monstertype = MonsterType.FastZombie;
        Sound = SoundManager.Instance.ZombieClip(Monstertype);
    }

    // 피격 시 실행
    public override void OnDamage(float damage, float _knockBackForce, Vector2 _knockBackDirection, WaitForSeconds invulnerabletime = null)
    {
        stat.OnDamage(damage);
        targetOn = true;
        
        hpBar.GetComponent<Slider>().value = stat.health / stat.maxHealth;
        if (stat.health < stat.maxHealth)
        {
            hpBar.SetActive(true);
        }

        if(invulnerabletime != null) StartCoroutine(SetInvulnerable(invulnerabletime));
        
        if (stat.health <= 0)
        {
            Die();
        }
        else
        {
            
            if (stat.health < stat.maxHealth / 2)
                stat.ChangeSpeed(3);
            if (Action != ActionList.SkillCasting1) // !! 스킬시전 중 스턴가능 여부 추가
            {
                if (Action == ActionList.OnDamaging)
                {
                    StopCoroutine(currentActionCoroutine);
                }

                knockBackForce = _knockBackForce;
                knockBackDirection = _knockBackDirection;
                Action = ActionList.OnDamaging;
            }

            SoundPlay(Sound[1]);
        }
    }
}
