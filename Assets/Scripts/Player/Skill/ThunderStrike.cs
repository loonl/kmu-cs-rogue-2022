using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
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
        ExecuteSkill();
    }

    private void GenerateEffects()
    {
        List<Monster> targets =
            SkillManager.Instance.SortMonstersByDistance(SkillManager.Instance.GetMonstersInRoom(DungeonSystem.Instance.Currentroom));

        List<Monster> temp = new List<Monster>();
        int count = Mathf.Min(targets.Count, 3);
        for (int i = 0; i<count; i++)
        {
            Debug.Log(i+1);
            GameObject realEffect = Instantiate(Resources.Load($"Prefabs/Skill/{weapon.skillName}")) as GameObject;
            realEffect.transform.position = new Vector3(targets[i].gameObject.transform.position.x, targets[i].gameObject.transform.position.y, -0.5f);
           
        }
        for(int i = temp.Count - 1; i >= 0; i--) // targets는 얕은복사가 되어있기 때문에 위의 for문 내에서 몬스터에게 대미지를 주어 죽는다면 리스트의 크기가 달라짐 - 버그 발생. 따라서 대미지는 나중에 줌..
        {
            temp[i].OnDamage(weapon.stat.skillDamage, knockbackPower, invulnerabletime: colliderValidTime);
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
