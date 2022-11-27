using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


public class ThunderDash: BaseSkill
{   
    
    protected override void Start()
    {
        init();
        StartCoroutine(ExecuteSkill());
    }

    protected override void init()
    {
        base.init();
        knockbackPower = 10f;
    }
    protected override void SetPosition() // 플레이어 이동 방향 및 거리에 맞춰서 위치, 각도 설정
    {   
        Vector2 PlayerChangedPos = player.transform.position;
        if (SkillManager.Instance.onGoingSkillInfo.TryGetValue(SkillManager.SkillInfo.PlayerChangedPos, out object pos))
        {
            PlayerChangedPos = (Vector3)pos;
        }
        
        Vector2 PlayerOriginalPos = (Vector3)SkillManager.Instance.onGoingSkillInfo[SkillManager.SkillInfo.PlayerOriginalPos];

        transform.position = new Vector2( (PlayerChangedPos.x + PlayerOriginalPos.x) / 2, (PlayerChangedPos.y + PlayerOriginalPos.y) / 2); // 스킬 위치: 이동한 거리의 중간 지점

        Vector3 vectorToTarget = PlayerChangedPos - PlayerOriginalPos;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg; // 스킬 각도
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward); // angle을 quaternion으로, rotation 설정
    }

    protected override IEnumerator ExecuteSkill() // 스킬 발동
    {
        SkillManager.Instance.onGoingSkillInfo.Add(SkillManager.SkillInfo.PlayerOriginalPos, player.transform.position);
        yield return Dash();
        SetPosition();
        yield return SkillAction();
    }

    private IEnumerator Dash()
    {
        if (player.GetComponent<Rigidbody2D>().velocity == Vector2.zero) // 플레이어가 가만히 있을 시 바라보고 있는 쪽으로 대쉬하도록
        {
            if (player.transform.localScale.x < 0)// 플레이어가 오른쪽 바라보고 있음
            {
                player.GetComponent<Rigidbody2D>().velocity = new Vector2(2, 0);
            }
            else
            {
                player.GetComponent<Rigidbody2D>().velocity = new Vector2(-2, 0);
            }
        }

        player.GetComponent<Rigidbody2D>().velocity *= 3; // 대쉬: 속도 3배
        
        player.curState = PlayerState.Dashing;
        StartCoroutine(SkillManager.Instance.VelocityLerp(player.GetComponent<Rigidbody2D>(), player.GetComponent<Rigidbody2D>().velocity, new Vector2(0, 0), 0.8f)); // Lerp를 이용해 속도를 부드럽게 0으로 만듦

        yield return GameManager.Instance.Setwfs(80);
    }

    protected override IEnumerator SkillAction() // 실제 스킬 효과 구현
    {
        animator.SetTrigger(weapon.skillName);
        collid.enabled = true;
        yield return colliderValidTime;
        collid.enabled = false;
        
        monsters.Clear();
        SkillManager.Instance.onGoingSkillInfo.Clear();
        player.curState = PlayerState.Normal;
    }

}
