using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ThunderStrike : BaseSkill
{
    bool firstgenerated = false; // 처음 생성된 이펙트?

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
            firstgenerated = true;
        }
        StartCoroutine(ExecuteSkill());
    }

    private void GenerateEffects()
    {
        List<Monster> targets =
            SkillManager.Instance.SortMonstersByDistance(player.gameObject, SkillManager.Instance.GetMonstersInRoom(DungeonSystem.Instance.Currentroom));

        int count = Mathf.Min(targets.Count, 3);
        for (int i = 0; i<count; i++)
        {
            GameObject realEffect = Instantiate(Resources.Load($"Prefabs/Skill/{weapon.skillName}")) as GameObject;
            realEffect.transform.position = new Vector3(targets[i].gameObject.transform.position.x, targets[i].gameObject.transform.position.y, -0.5f);
            targets[i].OnDamage(weapon.stat.skillDamage, knockbackPower, invulnerabletime: colliderValidTime);
        }
        
    }

    protected override IEnumerator SkillAction()
    {
        if (firstgenerated)
        {
            yield return GameManager.Instance.Setwfs(20);
            GenerateEffects();
            yield return GameManager.Instance.Setwfs(100);
            SkillManager.Instance.onGoingSkillInfo.Clear();
            Destroy(gameObject);
            yield break;
        }
        animator.SetTrigger(weapon.skillName);
    }

    protected override void SetPosition()
    {

    }
}
