using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SkillManager : MonoBehaviour
{
    Player player;
    public enum SkillInfo // ���� �ߵ����� ��ų ������ �� �� �ִ� ��
    {
        Name,
        Direction
    }
    private static SkillManager _instance = null;
    public static SkillManager Instance { get { return _instance; } }

    public Dictionary<SkillInfo, object> onGoingSkillInfo = new Dictionary<SkillInfo, object>();
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    public void InstantiateSkill(string skillname)
    {
        Debug.Log(skillname);
        GameObject skill = Instantiate(Resources.Load($"Prefabs/Skill/{skillname}")) as GameObject;
    }

    // �� ���� ����ִ� ���� ����Ʈ�� ������
    public List<Monster> GetMonstersInRoom(int roomindex)
    {
        List<Monster> monsters = new List<Monster>();
        if(DungeonSystem.Instance.monsterSpawners.TryGetValue(roomindex, out MonsterSpawner spawner)){
            monsters = spawner.monsters;
        }
        return monsters;
    }

    // �÷��̾���� �Ÿ��� ���� ���� ����Ʈ ����
    public List<Monster> SortMonstersByDistance(List<Monster> monsters)
    {
        monsters.Sort((m1, m2) =>
            GetDistanceFromPlayer(m1).CompareTo(GetDistanceFromPlayer(m2))
        );
        return monsters;
    }

    // �÷��̾�κ����� �Ÿ��� ����
    private float GetDistanceFromPlayer(Monster monster)
    {
        return Vector2.Distance(monster.gameObject.transform.position, player.gameObject.transform.position);
    }
}
