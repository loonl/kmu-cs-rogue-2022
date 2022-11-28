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
        Direction,
        PlayerOriginalPos,
        PlayerChangedPos,
        AliveEffectCount,
        SpawnerObject
    }

    public enum DirectionName
    {
        Up,
        RightUp,
        Right,
        RightDown,
        Down,
        LeftDown,
        Left,
        LeftUp
    }

    private static SkillManager _instance = null;
    public static SkillManager Instance { get { return _instance; } }

    public Dictionary<SkillInfo, object> onGoingSkillInfo = new Dictionary<SkillInfo, object>();
    public Dictionary<DirectionName, Vector2> DirectionDict = new Dictionary<DirectionName, Vector2>()
    {
        { DirectionName.Up, Vector2.up },
        { DirectionName.RightUp, new Vector2(1, 1).normalized },
        { DirectionName.Right, Vector2.right },
        { DirectionName.RightDown, new Vector2(1,-1).normalized },
        { DirectionName.Down, Vector2.down },
        { DirectionName.LeftDown, new Vector2(-1,-1).normalized },
        { DirectionName.Left, Vector2.left },
        { DirectionName.LeftUp, new Vector2(-1,1).normalized },
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

    // 방 안의 살아있는 몬스터 리스트를 가져옴
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

    // 게임오브젝트와의 거리에 따라 몬스터 리스트 정렬
    public List<Monster> SortMonstersByDistance(GameObject obj, List<Monster> monsters)
    {
        monsters.Sort((m1, m2) =>
            GetDistanceFromObject(obj, m1).CompareTo(GetDistanceFromObject(obj, m2))
        );
        return monsters;
    }

    // 게임오브젝트로부터 몬스터 사이의 거리를 구함
    private float GetDistanceFromObject(GameObject obj, Monster monster)
    {
        return Vector2.Distance(monster.gameObject.transform.position, obj.transform.position);
    }

    // 오브젝트로부터 가장 가까운 몬스터 반환
    public Monster GetClosestMonsterFromObject(GameObject obj)
    {
        List<Monster> monsters = SortMonstersByDistance(obj, GetMonstersInRoom(DungeonSystem.Instance.Currentroom));
        if (monsters.Count == 0) return null;
        return monsters[0];
    }
    // 속도 부드러운 변화를 위해 사용
    public IEnumerator VelocityLerp(Rigidbody2D rig, Vector2 source, Vector2 target, float overTime)
    {
        float startTime = Time.time;
        while (Time.time < startTime + overTime)
        {   
            if(rig.velocity == Vector2.zero) // 도중에 벽, 장애물 부딪혀서 속도가 0이 되어버린 경우
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

    // normalized된 source에서 target을 가리키는 normalized된 vector 반환
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
