using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


public class ThunderStrike : BaseSkill
{
    bool firstgenerated = false; // 처음 생성된 이펙트?
    [SerializeField] float NextExplosionGenerateTimeF = 0.15f; // 다음 폭발 생성 딜레이, 이펙트 애니메이션 길이보다 짧아야함!

    protected override void Start()
    {
        init();
    }
    protected override void init()
    {
        base.init();
        knockbackPower = 2f;
        if (SkillManager.Instance.onGoingSkillInfo.Count == 0)
        {
            SkillManager.Instance.onGoingSkillInfo.Add(SkillManager.SkillInfo.Name, "ThunderStrike");
            ExecuteSkill();
            return;
        }
        SetPosition();
    }

    protected override void SetPosition()
    {   
        
    }

    protected override IEnumerator SkillAction()
    {   
        yield return new WaitForSeconds(0.2f);
        
        animator.SetTrigger(weapon.skillName);
        
        collid.enabled = true;
        yield return colliderValidTime;
        collid.enabled = false;
        monsters.Clear();
    }

}
