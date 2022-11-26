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
        Direction,
        PlayerOriginalPos,
        PlayerChangedPos,
        AliveEffectCount,
        SpawnerObject
    }

    public enum DirectionName
    {
        Up,
        Right,
        Down,
        Left
    }

    private static SkillManager _instance = null;
    public static SkillManager Instance { get { return _instance; } }

    public Dictionary<SkillInfo, object> onGoingSkillInfo = new Dictionary<SkillInfo, object>();
    public Dictionary<DirectionName, Vector2> DirectionDict = new Dictionary<DirectionName, Vector2>()
    {
        { DirectionName.Up, Vector2.up },
        { DirectionName.Right, Vector2.right },
        { DirectionName.Down, Vector2.down },
        { DirectionName.Left, Vector2.left }
    };
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
            List<Monster> allmonsters = spawner.allMonsters;
            for(int i = allmonsters.Count-1; i >=0; i--)
            {
                if (!allmonsters[i].isDead)
                {
                    monsters.Add(allmonsters[i]);
                }
            }
        }
        return monsters;
    }

    // ���ӿ�����Ʈ���� �Ÿ��� ���� ���� ����Ʈ ����
    public List<Monster> SortMonstersByDistance(GameObject obj, List<Monster> monsters)
    {
        monsters.Sort((m1, m2) =>
            GetDistanceFromObject(obj, m1).CompareTo(GetDistanceFromObject(obj, m2))
        );
        return monsters;
    }

    // ���ӿ�����Ʈ�κ��� ���� ������ �Ÿ��� ����
    private float GetDistanceFromObject(GameObject obj, Monster monster)
    {
        return Vector2.Distance(monster.gameObject.transform.position, obj.transform.position);
    }

    public Monster GetClosestMonsterFromObject(GameObject obj)
    {
        List<Monster> monsters = SortMonstersByDistance(obj, GetMonstersInRoom(DungeonSystem.Instance.Currentroom));
        if (monsters.Count == 0) return null;
        return monsters[0];
    }
    // �ӵ� �ε巯�� ��ȭ�� ���� ���
    public IEnumerator VelocityLerp(Rigidbody2D rig, Vector2 source, Vector2 target, float overTime)
    {
        float startTime = Time.time;
        while (Time.time < startTime + overTime)
        {   
            if(rig.velocity == Vector2.zero) // ���߿� ��, ��ֹ� �ε����� �ӵ��� 0�� �Ǿ���� ���
            {
                player.curState = PlayerState.Normal;
                onGoingSkillInfo.Add(SkillInfo.PlayerChangedPos, player.transform.position);
                break;
            }
            rig.velocity = Vector2.Lerp(source, target, (Time.time - startTime) / overTime);
            yield return null;
        }
        rig.velocity = target;
    }

    // normalized�� source���� target�� ����Ű�� normalized�� vector ��ȯ
    public Vector2 GetDirectionFromObject(Transform target, Transform source)
    {
        return (target.position - source.position).normalized;
    }

    public void DestroySpawnerObject(GameObject obj)
    {
        Destroy(obj);
        onGoingSkillInfo.Clear();
    }
}
