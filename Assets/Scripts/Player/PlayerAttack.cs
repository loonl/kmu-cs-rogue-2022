using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static UnityEditor.Progress;

public class PlayerAttack : MonoBehaviour
{
    Player player;
    public Animator effectanim;
    Transform effectTransform; // for changing attack effect size or flipping

    private void Start()
    {
        player = GetComponent<Player>();
        effectanim = transform.GetChild(0).GetChild(2).GetComponent<Animator>();
        effectTransform = transform.GetChild(0).GetChild(2).GetComponent<Transform>();
    }

    public void Attack(int itemId)
    {
        switch (itemId)
        {
            case 2: // sword1 Normal
                effectanim.SetTrigger("NormalSlash");
                break;
            case 3: // sword2 Normal
                effectanim.SetTrigger("NormalSlash");
                break;
            case 4: // sword6 Nomral
                effectanim.SetTrigger("NormalSlash");
                break;
            case 5: // sword8 Normal
                effectanim.SetTrigger("NormalSlash");
                break;
            case 22: // sword3 Rare
                effectanim.SetTrigger("FireSlash");
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
                StartCoroutine(DoubleSlash());
                break;
            case 3:
                StartCoroutine(FireSlash());
                break;
            default:
                break;
        }
    }

    // ���� �� effect�� ũ��, flip �� ����
    public void SetUpEffect(string effectname = "None", Item item = null, float range = 1f)
    {   
        if (item != null) {
            range = item.stat.range;
            player.wpnColl.SetAttackRange(range); // ���� ����
            effectname = IDtoEffectName(item.id); // ����Ʈ�� �޾ƿ���
        } 
        Debug.Log(effectname);
        switch (effectname)
        {
            case "NormalSlash":
                effectTransform.localScale = new Vector2(range * 2.5f, range * -2.5f); // flip y
                effectTransform.eulerAngles = new Vector3(0, 0, 64f); // rotation
                effectTransform.localPosition = new Vector3(range * 0.08f, range * 0.8f, 0);
                break;
            default:
                effectTransform.localScale = new Vector2(range * 1.5f, range * 3);
                effectTransform.eulerAngles = new Vector3(0, 0, 0);
                effectTransform.position = player.transform.position;
                break;
        }
        if (player.transform.localScale.x < 0)
        {
            effectTransform.eulerAngles = new Vector3(0, 0, -effectTransform.eulerAngles.z); // �÷��̾ ������ ���������� �ǵ��Ѵ�� rotation�� ������ �ȵǼ� �������ִ� �ڵ�
        }
    }

    public string IDtoEffectName(int itemid)
    {
        switch (itemid) {
            case 2:
            case 3:
            case 4:
            case 5:
                return "NormalSlash";
            default:
                return "None";
        }
    }
    IEnumerator DoubleSlash()
    {
        player.wpnColl.poly.enabled = true;
        effectanim.SetTrigger("DoubleNormalSlash");
        yield return new WaitForSeconds(0.05f); // ��� �ð�. ���� �� collider ���� ���� ������ �޾ƿ��� ����...
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
    IEnumerator FireSlash()
    {
        player.wpnColl.poly.enabled = true;
        SetUpEffect("FireSlash", range:2);
        effectanim.SetTrigger("FireSlash");
        yield return new WaitForSeconds(0.05f);
        List<Monster> monsters;
        monsters = player.wpnColl.monsters.ToList();
        Debug.Log($"ds {monsters.Count}");
        foreach (Monster monster in monsters)
        {
            monster.OnDamage(player.stat.skillDamage, 5f, (monster.transform.position - transform.position).normalized);
        }
        yield return new WaitForSeconds(0.4f); // ����Ʈ ���ӽð�..
        player.wpnColl.poly.enabled = false;
        SetUpEffect("NormalSlash");
    }
}
