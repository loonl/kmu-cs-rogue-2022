using System.Collections;
using UnityEngine;


public class DoubleFireSlash : BaseSkill
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
            collid.offset = new Vector2(-0.3f, collid.offset.y);
            gameObject.GetComponent<SpriteRenderer>().flipX = false; // 작동을 안함.. 정확힌 하기는 하는데 의미 없음.. 애니메이터 때문인데 해결방안 불명.
            gameObject.transform.localScale = new Vector2(-gameObject.transform.localScale.x, gameObject.transform.localScale.y);
            gameObject.transform.position = player.transform.position + new Vector3(0.4f, 0, -0.5f);
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
            gameObject.transform.position = player.transform.position + new Vector3(-0.4f , 0, -0.5f);
        }
    }

    protected override IEnumerator SkillAction() // 실제 스킬 효과 구현
    {
        yield return GameManager.Instance.Setwfs(20);

        animator.SetTrigger(weapon.skillName);

        collid.enabled = true;
        yield return colliderValidTime;
        collid.enabled = false;
        monsters.Clear();

        yield return GameManager.Instance.Setwfs((int)(100*(animationLength/2 - colliderValidTimeF))); // 애니메이션 재생 시간의 절반까지 대기. 즉 두 번 휘두르는 이펙트이므로 한 번 휘두르는 이펙트가 종료될 때 까지 대기.

        collid.enabled = true; // 두번째 fire slash 이펙트 발동
        yield return colliderValidTime;
        collid.enabled = false;
        monsters.Clear();
    }

    protected override void OnTriggerStay2D(Collider2D collision)
    {
        if (monsters.Contains(collision))
        {
            Monster target = collision.gameObject.GetComponent<Monster>();
            if (!target.isInvulnerable) {
                target.OnDamage(weapon.stat.skillDamage, knockbackPower, (collision.gameObject.transform.position - player.transform.position).normalized, colliderValidTime);
                target.SetDotDmg(0.3f, 5f, 2f, 8f, "FireOrange");
            }
        }
        if (collision.gameObject.CompareTag("MapObject"))
        {
            collision.gameObject.SendMessage("OnDamage");
        }
    }
}
