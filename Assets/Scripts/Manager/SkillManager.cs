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

    public List<Monster> getMonstersInRoom(int roomindex)
    {
        List<Monster> monsters;
        monsters = DungeonSystem.Instance.monsterSpawners[roomindex].monsters;
        return monsters;
    }
}
