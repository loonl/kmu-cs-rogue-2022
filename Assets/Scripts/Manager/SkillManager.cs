using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SkillManager : MonoBehaviour
{
    Player player;
    public enum SkillInfo // ï¿½ï¿½ï¿½ï¿½ ï¿½ßµï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½Å³ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ ï¿½ï¿½ ï¿½Ö´ï¿½ ï¿½ï¿½
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

    // ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½Ö´ï¿½ ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½Æ®ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½
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

    // ï¿½ï¿½ï¿½Ó¿ï¿½ï¿½ï¿½ï¿½ï¿½Æ®ï¿½ï¿½ï¿½ï¿½ ï¿½Å¸ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½Æ® ï¿½ï¿½ï¿½ï¿½
    public List<Monster> SortMonstersByDistance(GameObject obj, List<Monster> monsters)
    {
        monsters.Sort((m1, m2) =>
            GetDistanceFromObject(obj, m1).CompareTo(GetDistanceFromObject(obj, m2))
        );
        return monsters;
    }

    // ï¿½ï¿½ï¿½Ó¿ï¿½ï¿½ï¿½ï¿½ï¿½Æ®ï¿½Îºï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½Å¸ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½
    private float GetDistanceFromObject(GameObject obj, Monster monster)
    {
        return Vector2.Distance(monster.gameObject.transform.position, obj.transform.position);
    }

    // ¿ÀºêÁ§Æ®·ÎºÎÅÍ °¡Àå °¡±î¿î ¸ó½ºÅÍ ¹ÝÈ¯
    public Monster GetClosestMonsterFromObject(GameObject obj)
    {
        List<Monster> monsters = SortMonstersByDistance(obj, GetMonstersInRoom(DungeonSystem.Instance.Currentroom));
        if (monsters.Count == 0) return null;
        return monsters[0];
    }
    // ï¿½Óµï¿½ ï¿½Îµå·¯ï¿½ï¿½ ï¿½ï¿½È­ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½
    public IEnumerator VelocityLerp(Rigidbody2D rig, Vector2 source, Vector2 target, float overTime)
    {
        float startTime = Time.time;
        while (Time.time < startTime + overTime)
        {   
            if(rig.velocity == Vector2.zero) // ï¿½ï¿½ï¿½ß¿ï¿½ ï¿½ï¿½, ï¿½ï¿½Ö¹ï¿½ ï¿½Îµï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½Óµï¿½ï¿½ï¿½ 0ï¿½ï¿½ ï¿½Ç¾ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½
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

    // normalizedï¿½ï¿½ sourceï¿½ï¿½ï¿½ï¿½ targetï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½Å°ï¿½ï¿½ normalizedï¿½ï¿½ vector ï¿½ï¿½È¯
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
