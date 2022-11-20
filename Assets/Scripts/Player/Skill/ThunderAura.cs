using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


public class ThunderAura: BaseSkill
{
    protected override void init()
    {
        base.init();
        knockbackPower = 3f;
    }
    protected override void SetPosition()
    {
        transform.position = player.transform.position;
        transform.SetParent(player.transform, true);
    }

    protected override IEnumerator SkillAction() // 실제 스킬 효과 구현
    {
        yield return GameManager.Instance.Setwfs(20);
        
        for (int i = 0; i < 3; i++)
        {
            animator.SetTrigger(weapon.skillName);
            collid.enabled = true;
            yield return colliderValidTime;
            collid.enabled = false;
            monsters.Clear();
            yield return GameManager.Instance.Setwfs((int)(200*animationLength));
        }
        yield return GameManager.Instance.Setwfs(100);
        Destroy(gameObject);
    }

}
