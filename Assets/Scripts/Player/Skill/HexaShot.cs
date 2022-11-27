using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexaShot : BaseSkill
{
    // 뒤집어서 애니메이션 동시에 재생해줄 Animator
    [SerializeField]
    private Animator anim2;
    
    // 플레이어가 바라보고 있는 방향 - 화살 방향 정할 때 사용
    private int weight;
    
    protected override void SetPosition()
    {
        // 플레이어가 보는 방향에 따라 Effect를 뒤집어줘야 함.
        if (player.transform.localScale.x < 0) // 플레이어가 오른쪽 바라보고 있음 
        {
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
            anim2.gameObject.GetComponent<SpriteRenderer>().flipX = true;
            gameObject.transform.position = GameManager.Instance.Player.transform.position + new Vector3(1, 1, 0);
            weight = -1;
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().flipX = false;
            anim2.gameObject.GetComponent<SpriteRenderer>().flipX = false;
            gameObject.transform.position = GameManager.Instance.Player.transform.position + new Vector3(-1, 1, 0);
            weight = 1;
        }
    }

    protected override IEnumerator SkillAction()
    {
        // 20ms 지연과 Collider 활성화 작업은 불필요함.
        yield return GameManager.Instance.Setwfs(0);
        
        if (animator != null)
            animator.SetTrigger(weapon.skillName);
        
        // 애니메이션 동시에 재생
        if (anim2 != null)
            anim2.SetTrigger(weapon.skillName);
        
        // 발사 방향
        Vector2[] directions =
        {
             new Vector2(-5.5f, 4.5f), new Vector2(-6.5f, 3.5f),
             new Vector2(-9, 1), new Vector2(-9.05f, -0.95f),
             new Vector2(-5.55f, -4.45f), new Vector2(-6.55f, -3.45f)
        };
        
        // 화살 발사 - 데미지는 2배
        for (int i = 0; i < directions.Length; i++)
        {
            player.GetComponent<ArrowGenerate>().Attack(weapon.effectName, directions[i].normalized * weight);
            player.GetComponent<ArrowGenerate>().Attack(weapon.effectName, directions[i].normalized * weight);
        }

        SkillManager.Instance.onGoingSkillInfo.Clear();
    }
}
