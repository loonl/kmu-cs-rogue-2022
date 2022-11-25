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
    protected WaitForSeconds colliderValidTime; // ������ ���� �ð� �� ���� �浹 ���� �ð�����, �̸� ����� �ξ� �ڷ�ƾ���� �޸� ���� ����. �⺻ 0.1��. ����Ʈ �ִϸ��̼� ���� �ð� != �浹 ���� �ð�
    protected float colliderValidTimeF = 0.1f; // ���� �ð��� 0.1�� �̿��� ������ �ϱ� ���� ���
    protected virtual void Start()
    {
        init();
        SetPosition();
        StartCoroutine(ExecuteSkill());
    }

    protected virtual void init() // ���� �ʱⰪ ����
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

    float GetAnimationLength() // ����Ʈ �ִϸ��̼��� ���� ��ȯ
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

    protected abstract void SetPosition(); // ��ų ������Ʈ ��ġ ����

    protected virtual IEnumerator ExecuteSkill() // ��ų �ߵ�
    {
        player.curState = PlayerState.Normal;
        yield return SkillAction();
    }

    protected virtual IEnumerator SkillAction() // ���� ��ų ȿ�� ����
    {
        yield return GameManager.Instance.Setwfs(20); // �÷��̾� ��ų ����� Į�� ����ٰ� ������� ����̱� ������, �ڿ������� ������ ���� �������� Ÿ�ֿ̹� ��ų�� �ߵ������ֵ��� ��� ���.
        if (animator != null) // ��ƼŬ�ý������� ����Ʈ ���� �� �ִϸ����Ͱ� ���� ���� ����.
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
        if (collision.gameObject.CompareTag("Monster")) // ��ų ��Ʈ�ڽ� ���� ���͸� ����Ʈ�� ���
        {
            monsters.Add(collision);
        }
    }
    protected virtual void OnTriggerStay2D(Collider2D collision)
    {   
        if (monsters.Contains(collision)) // monsters ����Ʈ�� ���ٸ� �̴� ���Ͱ� �ƴ�.
        {
            Monster target = collision.gameObject.GetComponent<Monster>();
            if(!target.isInvulnerable) target.OnDamage(weapon.stat.skillDamage, knockbackPower, invulnerabletime:colliderValidTime); // ����� �ֱ�
        }
        if (collision.gameObject.CompareTag("MapObject"))
        {
            collision.gameObject.SendMessage("OnDamage");
        }
    }

}
