using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowGenerate : MonoBehaviour
{
    private Player player;
    [SerializeField]
    private Transform parent;
    
    [SerializeField]
    private List<GameObject> arrowPrefabs;
    
    // test - 오브젝트 풀링
    private List<Queue<GameObject>> arrowPool;
    private int index;

    private void Start()
    {
        player = gameObject.GetComponent<Player>();
        parent = GameObject.Find("ArrowPrefabs").transform;
        
        ArrowPoolInit();
    }
    
    /*
     * 초기 설정 코드
     */
    
    // -------------------------------------------------------------
    // 오브젝트 풀링 첫 작업
    // -------------------------------------------------------------
    private void ArrowPoolInit()
    {
        arrowPool = new List<Queue<GameObject>>();
        for (int i = 0; i < arrowPrefabs.Count; i++)
        {
            Queue<GameObject> temp = new Queue<GameObject>();
            
            // 화살 2발씩 미리 로딩
            for (int j = 0; j < 2; j++)
            {
                GameObject obj = Instantiate(arrowPrefabs[i]);
                obj.SetActive(false);
                obj.transform.SetParent(parent);
                temp.Enqueue(obj);
            }
            
            arrowPool.Add(temp);
        }
    }
    
    /*
     * 호출되는 함수
     */
    
    // -------------------------------------------------------------
    // Arrow 공격
    // -------------------------------------------------------------
    public void Attack(string effectName)
    {
        // 사용할 화살 이펙트 prefab index 결정
        switch (effectName)
        {
            case "NormalArrow":
                index = 0;
                break;
        }
        
        GameObject arrowPrefab = arrowPool[index].Dequeue();
        arrowPrefab.SetActive(true);
        
        // 화살 스폰 위치 조정
        arrowPrefab.transform.position = player.transform.position;

        // 화살 발사 - 오토에임
        // TODO - 속도 조절
        arrowPrefab.GetComponent<Rigidbody2D>().velocity = NearestShootDirection() * 5f;
        

        /** PlayStore 출시할 때 오토에임 기능 Off 만든다고 결정되면 사용할 코드
        if (player.moveInfo != Vector3.zero)
            arrowPrefab.GetComponent<Rigidbody2D>().velocity = player.moveInfo * 15f;
        else
            arrowPrefab.GetComponent<Rigidbody2D>().velocity = new Vector2(-player.transform.localScale.x, 0) / 7 * 150f;
         */
    }
    
    // -------------------------------------------------------------
    // 사용이 끝나고 다시 Pool 안에 반납
    // -------------------------------------------------------------
    public void ReturnObject(GameObject obj)
    {
        obj.gameObject.SetActive(false);
        arrowPool[index].Enqueue(obj);
    }
    
    // -------------------------------------------------------------
    // Player가 바라보고 있는 방향에 있는 몬스터들 중에서 가장 가까운 몬스터의 방향 반환
    // -------------------------------------------------------------
    private Vector2 NearestShootDirection()
    {
        List<Monster> monstersList = new List<Monster>(); // 겨냥 가능한 Monster List  
        int roomIndex = DungeonSystem.Instance.Currentroom;
        Player player = GameManager.Instance.Player;

        // 모든 몬스터 List에서 각각 조건 확인
        // 현재 방의 MonsterSpawner 불러옴
        if (DungeonSystem.Instance.monsterSpawners.TryGetValue(roomIndex, out MonsterSpawner spawner))
        {
            foreach (var monster in spawner.allMonsters)
            {
                // 겨냥 가능 조건1 : 몬스터가 살아있음
                if (!monster.isDead)
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
        
        // 겨냥 가능한 Monster가 없다면 Player가 보는 방향으로 발사
        if (monstersList.Count == 0)
            return new Vector2(-player.transform.localScale.x, 0).normalized;

        // 조건을 만족하는 Monster들 중에서 가장 가까운 Monster 판별
        Vector2 returnVal = monstersList[0].transform.position - player.transform.position;
        float mini = returnVal.magnitude;
        for (int i = 1; i < monstersList.Count; i++)
        {
            Monster monster = monstersList[i];
            Vector2 direction = monster.transform.position - player.transform.position;
            if (direction.magnitude < mini)
            {
                mini = direction.magnitude;
                returnVal = direction;
            }
        }

        return returnVal.normalized;
    }
}
