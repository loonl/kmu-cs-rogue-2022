using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InfiniteClaw : BaseSkill
{
    bool firstGenerated = true;
    Monster targetMonster;
    protected override void init()
    {
        base.init();
        knockbackPower = 2f;
    }
    protected override void SetPosition()
    {
        List<Monster> target = 
            SkillManager.Instance.SortMonstersByDistance(player.gameObject, SkillManager.Instance.GetMonstersInRoom(DungeonSystem.Instance.Currentroom));
        if (target.Count != 0)
        {
            transform.position = target[0].transform.position; // 플레이어로부터 가장 가까운 몬스터 위치로 설정
            targetMonster = target[0];
        }
        else if (firstGenerated) transform.position = player.transform.position;
    }

    private IEnumerator DestroyObj() // 애니메이션 이벤트로 호출, 오브젝트 삭제
    {
        yield return GameManager.Instance.Setwfs(100);
        Destroy(gameObject);
    }
    private IEnumerator EnalbeCollider(float scale) // 애니메이션 이벤트로 호출, 콜라이더 활성화 및 위치 재지정
    {
        if (targetMonster == null) yield break;
        firstGenerated = false;
        if (!targetMonster.isDead) transform.position = targetMonster.transform.position;
        else SetPosition();
        gameObject.transform.localScale = new Vector2((float)scale, (float)scale);
        collid.enabled = true;
        yield return colliderValidTime;
        collid.enabled = false;
        monsters.Clear();
    }
    protected override void OnTriggerStay2D(Collider2D collision)
    {
        if (monsters.Contains(collision)) // monsters 리스트에 없다면 이는 몬스터가 아님.
        {
            Monster target = collision.gameObject.GetComponent<Monster>();
            if (!target.isInvulnerable) target.OnDamage(weapon.stat.skillDamage/16, knockbackPower, invulnerabletime: GameManager.Instance.Setwfs(0)); // 대미지 주기, 총 16회 공격이므로 16으로 나눔.
        }
        if (collision.gameObject.CompareTag("MapObject"))
        {
            collision.gameObject.SendMessage("OnDamage");
        }
    }
}
