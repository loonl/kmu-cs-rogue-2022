using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    Player player;

    private void Start()
    {
        player = GetComponent<Player>();
    }

    public void Attack(int itemId)
    {
        switch (itemId)
        {
            case 0:
                break;
        }
    }

    public void SkillAttack(int itemId)
    {
        switch (itemId)
        {
            case 0:
                break;
        }
    }
}
