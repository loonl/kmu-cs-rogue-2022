using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SkillManager : MonoBehaviour
{
    Player player;
    public enum SkillInfo // 현재 발동중인 스킬 정보가 될 수 있는 것
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

    // 방 안의 살아있는 몬스터 리스트를 가져옴
    public List<Monster> GetMonstersInRoom(int roomindex)
    {
        List<Monster> monsters = new List<Monster>();
        if(DungeonSystem.Instance.monsterSpawners.TryGetValue(roomindex, out MonsterSpawner spawner)){
            monsters = spawner.monsters;
        }
        return monsters;
    }

    // 플레이어와의 거리에 따라 몬스터 리스트 정렬
    public List<Monster> SortMonstersByDistance(List<Monster> monsters)
    {
        monsters.Sort((m1, m2) =>
            GetDistanceFromPlayer(m1).CompareTo(GetDistanceFromPlayer(m2))
        );
        return monsters;
    }

    // 플레이어로부터의 거리를 구함
    private float GetDistanceFromPlayer(Monster monster)
    {
        return Vector2.Distance(monster.gameObject.transform.position, player.gameObject.transform.position);
    }
}
