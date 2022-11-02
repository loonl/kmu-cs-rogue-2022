using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerAttack : MonoBehaviour
{
    Player player;
    public Animator effectanim;
    Transform effectTransform; // for changing attack effect size or flipping

    // !! meleeWeaponID, meleeWeaponIdx - ���� �׽�Ʈ�� ���� �ڵ� !!
    List<int> meleeWeaponID = new List<int>() { 0, 2, 3, 4, 5, 22, 23, 24, 25, 26, 27, 28, 29, 30, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 77, 78, 79, 80 };
    int meleeWeaponIdx = 0;
    private void Start()
    {
        player = GetComponent<Player>();
        effectanim = transform.GetChild(0).GetChild(2).GetComponent<Animator>();
        effectTransform = transform.GetChild(0).GetChild(2).GetComponent<Transform>();
    }

    public void Attack(string effectname)
    {
        effectanim.SetTrigger(effectname);
    }
    public void SkillAttack(int itemId)
    {
        
        switch (itemId)
        {
            case 2:
                StartCoroutine(BigNormalSlash());
                break;
            case 3:
                StartCoroutine(FireSlash());
                break;
            default:
                break;
        }
    }

    // ���� �� effect�� ũ��, flip �� ����
    public void SetUpEffect(string effectname = "NormalSlash2", Item item = null, float range = 1f)
    {   
        if (item != null) {
            effectname = item.effectName;
            range = item.stat.range;
            player.wpnColl.SetAttackRange(range); // ���� ����
        } 
        switch (effectname)
        {
            case "NormalSlash":
                effectTransform.localScale = new Vector2(range * 2.5f, range * 2.5f); 
                effectTransform.eulerAngles = new Vector3(0, 0, 64f); // rotation
                effectTransform.localPosition = new Vector3(range * 0.08f, range * 0.8f, 0);
                break;
            case "NormalSlash2":
                effectTransform.localScale = new Vector2(range * 1.5f, range * 3f); 
                effectTransform.eulerAngles = new Vector3(0, 0, 0); // rotation
                effectTransform.localPosition = new Vector3(0.2f, -0.2f, 0);
                break;
            case "NormalSlash3":
                effectTransform.localScale = new Vector2(range * 1.5f, -range * 2.5f);
                effectTransform.eulerAngles = new Vector3(0, 0, 0); // rotation
                effectTransform.localPosition = new Vector3(0.04f, -0.09f, 0);
                break;
            case "FireSlash":
            case "ElectricSlash":
                effectTransform.localScale = new Vector2(range * 1.5f, -range * 2.5f);
                effectTransform.eulerAngles = new Vector3(0, 0, 0); // rotation
                effectTransform.localPosition = new Vector3(0.08f, -0.09f, 0);
                break;
            case "ElectricSlash2":
            case "FireSlash2":
                effectTransform.localScale = new Vector2(range * 2f, range * 2.5f);
                effectTransform.eulerAngles = new Vector3(0, 0, 30); // rotation
                effectTransform.localPosition = new Vector3(0.08f, 0.28f, 0);
                break;
            case "FireSlash3":
            case "ElectricSlash3":
                effectTransform.localScale = new Vector2(range * 1.5f, range * 3.2f);
                effectTransform.eulerAngles = new Vector3(0, 0, 0); // rotation
                effectTransform.localPosition = new Vector3(0, -0.2f, 0);
                break;
            case "ElectricClaw":
            case "NormalClaw":
            case "FireClaw":
                effectTransform.localScale = new Vector2(range * 1.5f, range * 2f);
                effectTransform.eulerAngles = new Vector3(0, 0, 0); // rotation
                effectTransform.localPosition = new Vector3(-0.21f, 0.07f, 0);
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

    IEnumerator DoubleFireSlash()
    {
        player.wpnColl.poly.enabled = true;
        effectanim.SetTrigger("DoubleFireSlash");
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
        player.wpnColl.SetAttackRange(2); // �Ϲ� ���� �Ÿ��� 1������ ��ų ���ݿ��� 2�� ��� ����
        yield return new WaitForSeconds(0.05f);
        List<Monster> monsters;
        monsters = player.wpnColl.monsters.ToList();
        Debug.Log($"monster in collider {monsters.Count}");
        foreach (Monster monster in monsters)
        {
            monster.OnDamage(player.stat.skillDamage, 5f, (monster.transform.position - transform.position).normalized);
        }
        yield return new WaitForSeconds(0.4f); // ����Ʈ �ִϸ��̼� ���ӽð�
        player.wpnColl.SetAttackRange(1); // ���� �Ÿ� �ǵ���
        player.wpnColl.poly.enabled = false;
        SetUpEffect("NormalSlash");
    }
    IEnumerator BigNormalSlash()
    {
        player.wpnColl.poly.enabled = true;
        SetUpEffect("NormalSlash", range: 2);
        effectanim.SetTrigger("NormalSlash");
        player.wpnColl.SetAttackRange(2); // �Ϲ� ���� �Ÿ��� 1������ ��ų ���ݿ��� 2�� ��� ����
        yield return new WaitForSeconds(0.05f);
        List<Monster> monsters;
        monsters = player.wpnColl.monsters.ToList();
        Debug.Log($"monster in collider {monsters.Count}");
        foreach (Monster monster in monsters)
        {
            monster.OnDamage(player.stat.skillDamage, 5f, (monster.transform.position - transform.position).normalized);
        }
        yield return new WaitForSeconds(0.2f); // ����Ʈ �ִϸ��̼� ���ӽð�
        player.wpnColl.poly.enabled = false;
        player.wpnColl.SetAttackRange(1); // ���� �Ÿ� �ǵ���
        SetUpEffect("NormalSlash");
    }

    // !! ���� �׽�Ʈ�� ���� �ڵ� !!
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftBracket)) //'[' Ű
        {
            if (meleeWeaponIdx == 0)
                meleeWeaponIdx = meleeWeaponID.Count - 1;
            else meleeWeaponIdx -= 1;
            Item it = ItemManager.Instance.GetItem(meleeWeaponID[meleeWeaponIdx]);
            Debug.Log($"{it.name}, id {it.id}");
            player.Equip(it);
        }
        if (Input.GetKeyDown(KeyCode.RightBracket)) //']' Ű
        {
            if (meleeWeaponIdx == meleeWeaponID.Count - 1)
                meleeWeaponIdx = 0;
            else meleeWeaponIdx += 1;
            Item it = ItemManager.Instance.GetItem(meleeWeaponID[meleeWeaponIdx]);
            Debug.Log($"{it.name}, id {it.id}");
            player.Equip(it);
        }
    }
}
