using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimreceiver : MonoBehaviour
{
    public System.Action onDieStart;
    public System.Action onAttackComplete;
    public System.Action onSkillComplete;
    public System.Action onStunComplete;
    public System.Action onArrowShoot;
    public System.Action onBowSkillStart;
    public System.Action onSkillArrowShoot;
    public System.Action onMoveStart;

    // executed at the end of animation death
    public void OnDieStart()
    {
        if (onDieStart != null)
        {
            this.onDieStart();
        }
    }

    // executed at the end of animation attack
    public void OnAttackComplete()
    {
        if (onAttackComplete != null)
        {
            this.onAttackComplete();
        }
    }

    // executed at the end of animation skill
    public void OnSkillComplete()
    {
        if (onSkillComplete != null)
        {
            this.onSkillComplete();
        }
    }

    // executed at the end of animation stun
    public void OnStunComplete()
    {
        if (onStunComplete != null)
        {
            this.onStunComplete();
        }
    }
    
    // executed at the time of arrow shooting motion
    public void OnArrowShoot()
    {
        if (onArrowShoot != null)
        {
            this.onArrowShoot();
        }
    }
    
    // executed at the time of skill using motion
    public void OnBowSkillStart()
    {
        if (onBowSkillStart != null)
        {
            this.onBowSkillStart();
        }
    }
    
    // executed at the time of skill using motion
    public void OnSkillArrowShoot()
    {
        if (onSkillArrowShoot != null)
        {
            this.onSkillArrowShoot();
        }
    }
    
    // executed at the start of move motion
    // catch bug about arrow attack
    public void OnMoveStart()
    {
        if (onMoveStart != null)
        {
            this.onMoveStart();
        }
            
    }
}
