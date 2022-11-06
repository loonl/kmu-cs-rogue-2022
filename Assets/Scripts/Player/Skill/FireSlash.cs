using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


public class FireSlash : BaseSkill
{
    protected override void init()
    {
        base.init();
        knockbackPower = 10f;
    }
    protected override void SetPosition()
    {
        if (player.transform.localScale.x < 0)// 플레이어가 오른쪽 바라보고 있음
        {
            collid.offset = new Vector2(0.3f, collid.offset.y);
            gameObject.GetComponent<SpriteRenderer>().flipX = false;
            gameObject.transform.position = player.transform.position + new Vector3(0.4f, 0, -0.5f);
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
            gameObject.transform.position = player.transform.position + new Vector3(-0.4f, 0, -0.5f);
        }
    }
}
