using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Staff : MonoBehaviour
{
    private Player player;
    List<Monster> monstersList = new List<Monster>();
    float range = 1.5f;
    private Transform parent;

    [SerializeField]
    private GameObject EffectPrefab;
    private Queue<GameObject> EffectPool = new Queue<GameObject>();

    private void Start()
    {
        player = GameManager.Instance.Player;
        Init();
    }

    void Init()
    {
        //하이라키 창이 깔끔하게 보이도록 만들어줄 프리팹의 parent 생성
        GameObject go = Instantiate(Resources.Load<GameObject>("Prefabs/Magic/EffectPrefabs"));
        parent = go.transform;
        // 마법 5발씩 미리 로딩
        for (int j = 0; j < 5; j++)
        {
            GameObject obj = Instantiate(EffectPrefab);
            obj.transform.SetParent(parent);
            obj.SetActive(false);
            EffectPool.Enqueue(obj);
        }
    }

    // -------------------------------------------------------------
    // Staff 공격
    // -------------------------------------------------------------
    public void Attack(string effectName, bool isattack = true)
    {
        GameObject EffectPrefab = EffectPool.Dequeue();
        EffectPrefab.name = effectName;
        EffectPrefab.SetActive(true);

        EffectPrefab.transform.position = NearestShootDirection(isattack);

        StartCoroutine(ReturnObject(EffectPrefab, isattack));
    }


    // -------------------------------------------------------------
    // 사용이 끝나고 다시 Pool 안에 반납
    // -------------------------------------------------------------
    public IEnumerator ReturnObject(GameObject obj, bool isattack = true)
    {
        if(isattack)
            yield return GameManager.Instance.Setwfs(100);
        else
            yield return GameManager.Instance.Setwfs(75);
        obj.SetActive(false);
        EffectPool.Enqueue(obj);
    }

    // -------------------------------------------------------------
    // Player가 바라보고 있는 방향에 있는 몬스터들 중에서 가장 가까운 몬스터 반환
    // -------------------------------------------------------------
    private Vector3 NearestShootDirection(bool isattack = true)
    {
        int roomIndex = DungeonSystem.Instance.Currentroom;

        // 모든 몬스터 List에서 각각 조건 확인
        // 현재 방의 MonsterSpawner 불러옴
        if (DungeonSystem.Instance.monsterSpawners.TryGetValue(roomIndex, out MonsterSpawner spawner))
        {
            foreach (Monster monster in spawner.allMonsters)
            {
                // 겨냥 가능 조건1 : 몬스터가 살아있으며 범위 내에 있어야 함
                if (!monster.isDead && Vector3.Distance(monster.transform.position, player.transform.position) < range)
                {
                    // Player가 왼쪽을 보고 있다면 
                    if (player.transform.localScale.x > 0)
                    {
                        // 겨냥 가능 조건2 : 몬스터가 Player 왼쪽에 있음
                        if (monster.transform.position.x < player.transform.position.x)
                            monstersList.Add(monster);
                    }
                    else // Player가 오른쪽을 보고 있다면
                    {
                        // 겨냥 가능 조건2 : 몬스터가 Player 오른쪽에 있음
                        if (monster.transform.position.x > player.transform.position.x)
                            monstersList.Add(monster);
                    }
                }
            }
        }
        
        // 겨냥 가능한 Monster가 없다면 Player가 보는 방향의 range거리를 반환
        if (monstersList.Count == 0)
            return player.transform.position + new Vector3(-player.transform.localScale.x, 0, 0).normalized * range;

        // 조건을 만족하는 Monster들 중에서 가장 가까운 Monster 판별
        Vector3 returnVal = monstersList[0].transform.position - player.transform.position;
        float mini = returnVal.magnitude;
        int listnum = 0;
        for (int i = 1; i < monstersList.Count; i++)
        {
            Vector3 direction = monstersList[i].transform.position - player.transform.position;
            if (direction.magnitude < mini)
            {
                mini = direction.magnitude;
                returnVal = direction;
                listnum = i;
            }
        }
        if(isattack)
            monstersList[listnum].OnDamage(player.stat.damage, player.stat.knockBackForce,
                (monstersList[listnum].transform.position - transform.position).normalized, GameManager.Instance.Setwfs(10));
        monstersList.Clear();
        //몬스터의 위치 반환
        return returnVal + player.transform.position;
    }
}
