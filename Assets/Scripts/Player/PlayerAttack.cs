using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerAttack : MonoBehaviour
{
    Player player;
    Animator effectanim;

    private void Start()
    {
        player = GetComponent<Player>();
        effectanim = transform.GetChild(0).GetChild(2).GetComponent<Animator>();
    }

    public void Attack(int itemId)
    {
        switch (itemId)
        {
            case 2:
                effectanim.SetTrigger("FireSlash");
                break;
            case 3:
                effectanim.SetTrigger("ElectricSlash");
                break;
            default:
                break;
        }
    }

    public void SkillAttack(int itemId)
    {
        switch (itemId)
        {
            case 2:
                player.wpnColl.poly.enabled = true;
                StartCoroutine(DoubleSlash());
                effectanim.SetTrigger("DoubleFireSlash");
                break;
            case 3:
                effectanim.SetTrigger("ElectricSlash");
                break;
            default:
                break;
        }
    }

    IEnumerator DoubleSlash()
    {
        yield return new WaitForSeconds(0.05f);
        List<Monster> monsters;        
        monsters = player.wpnColl.monsters.ToList();
        Debug.Log($"ds {monsters.Count}");
        foreach (Monster monster in monsters)
        {
            monster.OnDamage(player.stat.skillDamage, 5f, (monster.transform.position - transform.position).normalized);
        }
        yield return new WaitForSeconds(0.2f);
        foreach (Monster monster in monsters)
        {
            monster.OnDamage(player.stat.skillDamage, 5f, (monster.transform.position - transform.position).normalized);
        }
        player.wpnColl.poly.enabled = false;
        if (player.wpnColl.monsters.Count > 0)
        {
            player.wpnColl.monsters.Clear();
        }
    }
}
