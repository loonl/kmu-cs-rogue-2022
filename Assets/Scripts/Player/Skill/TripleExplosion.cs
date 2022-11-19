using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


public class TripleExplosion: BaseSkill
{
    protected override void init()
    {
        base.init();
        knockbackPower = 10f;
    }
    protected override void SetPosition()
    {   
        Vector2 direction = player.GetComponent<Rigidbody2D>().velocity.normalized;
        if (direction == Vector2.zero) {
            if (player.transform.localScale.x < 0)// 플레이어가 오른쪽 바라보고 있음
            {
                direction = new Vector2(1, 0);
            }
            else
            {
                direction = new Vector2(-1, 0);
            }
        }
        player.GetComponent<Rigidbody2D>().AddForce(direction * 200f);
        gameObject.transform.position = player.transform.position + new Vector3(gameObject.transform.localScale.x / 3 * direction.x, gameObject.transform.localScale.y / 3 * direction.y, -0.5f);
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
            yield return GameManager.Instance.Setwfs((int)(100*animationLength));
        }
        yield return GameManager.Instance.Setwfs(100);
        Destroy(gameObject);
    }

}
