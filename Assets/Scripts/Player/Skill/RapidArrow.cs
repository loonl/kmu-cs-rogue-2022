using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RapidArrow : BaseSkill
{
    protected override void init()
    {
        player = GameManager.Instance.Player;
        weapon = player.equipment[0];
    }

    protected override void SetPosition()
    {
    }
    
    protected override IEnumerator SkillAction()
    {
        // 3초동안 속사로 발사
        yield return GameManager.Instance.Setwfs(300);
        
        // 끝나면 Player를 다음 모션으로 이동시킴
        player.anim.SetBool("SkillFinished", true);
        
        SkillManager.Instance.onGoingSkillInfo.Clear();
        
        Destroy(gameObject);
    }
}
