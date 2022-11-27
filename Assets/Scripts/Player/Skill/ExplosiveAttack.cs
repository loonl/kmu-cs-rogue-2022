using System.Collections;
using UnityEngine;


public class ExplosiveAttack : BaseSkill
{
    protected override void init()
    {
        base.init();
        knockbackPower = 15f;
    }
    protected override void SetPosition()
    {
        if (player.transform.localScale.x < 0)// 플레이어가 오른쪽 바라보고 있음
        {
            gameObject.transform.position = player.transform.position + new Vector3(gameObject.transform.localScale.x / 2 -0.2f, 0.5f, -0.5f);
        }
        else
        {
            gameObject.transform.position = player.transform.position + new Vector3(-gameObject.transform.localScale.x / 2 + 0.2f, 0.5f, -0.5f);
        }
    }
}
