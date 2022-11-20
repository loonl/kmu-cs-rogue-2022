using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SmokeExplosion : BaseSkill
{
    protected override void init()
    {
        base.init();
        knockbackPower = 15f;
    }
    protected override void SetPosition()
    {
        List<Monster> target = 
            SkillManager.Instance.SortMonstersByDistance(SkillManager.Instance.GetMonstersInRoom(DungeonSystem.Instance.Currentroom));
        if (target.Count != 0) transform.position = target[0].transform.position;
        else transform.position = player.transform.position;
    }

}
