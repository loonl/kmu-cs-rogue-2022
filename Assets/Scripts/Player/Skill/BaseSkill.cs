using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSkill : MonoBehaviour
{
    Player player;
    Animator animator;
    Item weapon;
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        weapon = player.equipment[0];
        animator = GetComponent<Animator>();
        ExecuteSkill();
    }

    protected virtual void ExecuteSkill()
    {
        StartCoroutine(SkillAction());
    }

    protected virtual IEnumerator SkillAction()
    {
        yield return new WaitForSeconds(0.2f);
        if (animator != null)
        {
            animator.SetTrigger(weapon.skillName);
        }
    }
}
