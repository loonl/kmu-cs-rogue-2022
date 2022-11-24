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

    // normalized된 target에서 source를 가리키는 vector 반환
    public Vector2 GetDirectionFromObject(Transform source, Transform target)
    {
        return (source.position - target.position).normalized;
    }

    public void DestroySpawnerObject(GameObject obj)
    {
        Destroy(obj);
        onGoingSkillInfo.Clear();
    }
}
