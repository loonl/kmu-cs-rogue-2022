using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSkill : MonoBehaviour
{
    protected Player player;
    protected Animator animator;
    protected Item weapon;
    protected Collider2D collid;
    protected float knockbackPower;
    protected List<Collider2D> monsters = new List<Collider2D>();
    protected float animationLength;
    protected WaitForSeconds colliderValidTime; // 몬스터의 무적 시간 및 범위 충돌 판정 시간으로, 미리 만들어 두어 코루틴에서 메모리 낭비를 방지. 기본 0.1초. 이펙트 애니메이션 지속 시간 != 충돌 판정 시간
    protected float colliderValidTimeF = 0.1f; // 판정 시간을 0.1초 이외의 값으로 하기 위해 사용
    protected virtual void Start()
    {
        init();
        SetPosition();
        StartCoroutine(ExecuteSkill());
    }

    protected virtual void init() // 변수 초기값 설정
    {
        player = GameManager.Instance.Player;
        gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Effect";
        weapon = player.equipment[0];
        animator = GetComponent<Animator>();
        colliderValidTime = GameManager.Instance.Setwfs((int)(100 * colliderValidTimeF));
        collid = GetComponent<Collider2D>();
        if(collid != null) collid.enabled = false;
        animationLength = GetAnimationLength();
    }

    float GetAnimationLength() // 이펙트 애니메이션의 길이 반환
    {
        if (animator == null) return 0;
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        for (int i = 0; i < clips.Length; i++)
        {
            if (clips[i].name == weapon.skillName)
            {
                return clips[i].length;
            }
        }
        return 0;
    }

    protected abstract void SetPosition(); // 스킬 오브젝트 위치 설정

    protected virtual IEnumerator ExecuteSkill() // 스킬 발동
    {
        player.curState = PlayerState.Normal;
        yield return SkillAction();
    }

    protected virtual IEnumerator SkillAction() // 실제 스킬 효과 구현
    {
        yield return GameManager.Instance.Setwfs(20); // 플레이어 스킬 모션이 칼을 들었다가 내려찍는 모션이기 때문에, 자연스러운 연출을 위해 내려찍을 타이밍에 스킬을 발동시켜주도록 잠시 대기.
        if (animator != null) // 파티클시스템으로 이펙트 구현 시 애니메이터가 없을 수도 있음.
        {
            animator.SetTrigger(weapon.skillName);
        }
        collid.enabled = true;
        yield return colliderValidTime;
        collid.enabled = false;
        monsters.Clear();
        SkillManager.Instance.onGoingSkillInfo.Clear();
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Monster")) // 스킬 히트박스 내의 몬스터를 리스트에 담기
        {
            monsters.Add(collision);
        }
    }
    protected virtual void OnTriggerStay2D(Collider2D collision)
    {   
        if (monsters.Contains(collision)) // monsters 리스트에 없다면 이는 몬스터가 아님.
        {
            Monster target = collision.gameObject.GetComponent<Monster>();
            if(!target.isInvulnerable) target.OnDamage(weapon.stat.skillDamage, knockbackPower, invulnerabletime:colliderValidTime); // 데미지 주기
        }
        if (collision.gameObject.CompareTag("MapObject"))
        {
            collision.gameObject.SendMessage("OnDamage");
        }
    }

}
