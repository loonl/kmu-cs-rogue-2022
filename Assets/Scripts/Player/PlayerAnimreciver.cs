using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimreciver : MonoBehaviour
{
    public System.Action onDieComplete;
    public System.Action onAttackComplete;
    public System.Action onSkillComplete;
    public System.Action onStunComplete;
    public System.Action onArrowShoot;

    // executed at the end of animation death
    public void OnDieComplete()
    {
        if (onDieComplete != null)
        {
            this.onDieComplete();
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
}
