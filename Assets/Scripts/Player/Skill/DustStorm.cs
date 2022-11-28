using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DustStorm: BaseSkill
{
    [SerializeField]
    private float velocity = 0.0003f;
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

    IEnumerator Chase()
    {
        Monster chasingMonster = SkillManager.Instance.GetClosestMonsterFromObject(gameObject);
        if(chasingMonster == null)
        {
            yield break;
        }
        Vector2 dir;
        while (!chasingMonster.isDead)
        {
            dir = SkillManager.Instance.GetDirectionFromObject(chasingMonster.transform, gameObject.transform);
            gameObject.transform.position += (Vector3)dir * velocity;
            yield return null;
        }
        StartCoroutine(Chase());
    }

    protected override IEnumerator SkillAction() // 실제 스킬 효과 구현
    {
        yield return GameManager.Instance.Setwfs(20);
        animator.SetTrigger(weapon.skillName);
        StartCoroutine(Chase());
        collid.enabled = true;
        for (int i = 0; i < 3; i++)
        {
            yield return GameManager.Instance.Setwfs((int)(100*animationLength));
        }
        Destroy(gameObject);
    }

    // 다른 점: 몬스터 무적 시간을 애니메이션 길이와 연관지음
    protected override void OnTriggerStay2D(Collider2D collision)
    {
        if (monsters.Contains(collision))
        {
            Monster target = collision.gameObject.GetComponent<Monster>();
            if (!target.isInvulnerable) target.OnDamage(weapon.stat.skillDamage, knockbackPower, invulnerabletime: GameManager.Instance.Setwfs((int)(70 * animationLength))); // 대미지 주기
        }
        if (collision.gameObject.CompareTag("MapObject"))
        {
            collision.gameObject.SendMessage("OnDamage");
        }
    }

}
