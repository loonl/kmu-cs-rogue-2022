using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleArrows : BaseSkill
{
    protected override void SetPosition()
    {
        gameObject.transform.position = GameManager.Instance.Player.transform.position;
    }

    protected override IEnumerator SkillAction()
    {
        // 20ms 지연과 Collider 활성화 작업은 불필요함.
        yield return GameManager.Instance.Setwfs(0);
        
        if (animator != null)
            animator.SetTrigger(weapon.skillName);
        
        
        Vector2[] directions =
        {
            new Vector2(-1, -1), new Vector2(-1, 0), new Vector2(-1, 1),
            new Vector2(0, 1), new Vector2(0, -1),
            new Vector2(1, -1), new Vector2(1, 0), new Vector2(1, 1)
        };
        
        // 화살 8방향으로 발사
        for (int i = 0; i < directions.Length; i++)
        {
            // 데미지는 2배
            player.GetComponent<ArrowGenerate>().Attack(weapon.effectName, directions[i]);
            player.GetComponent<ArrowGenerate>().Attack(weapon.effectName, directions[i]);
        }

        SkillManager.Instance.onGoingSkillInfo.Clear();
    }
}
