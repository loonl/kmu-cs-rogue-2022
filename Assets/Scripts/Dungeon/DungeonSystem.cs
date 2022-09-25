using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonSystem : MonoBehaviour
{
    private static DungeonSystem instance = null;
    public static DungeonSystem Instance { get { return instance; } }

    public int Floor { get; private set; } = 1;

    [SerializeField]
    private RoomGenerator generator;
    [SerializeField]
    private int tempRoomCount;

    public Transform DroppedItems;        // 떨어진 아이템 parent transform


    public List<DungeonRoom> Rooms { get { return generator.Rooms; } }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void Load()
    {
        // 현재 Floor 기준으로 레벨 디자인
        CreateDungeon();
    }

    // -------------------------------------------------------------
    // 던전 생성
    // -------------------------------------------------------------
    public void CreateDungeon()
    {
        List<TileType> tileSeqence = new List<TileType>()
        {
            TileType.DefaultGround,
            TileType.MossGround,
            TileType.VineGround,
            TileType.VineMossGround
        };

        // 맵 생성
        generator.Generate(tempRoomCount, tileSeqence[(Floor - 1) % 4]);
        CreateMonsterSpawner();   // 몬스터스포너 생성
        CreateShop();       // 상점 생성
    }

    public void ClearDungeon()
    {
        // 맵 삭제
        generator.Clear();
    }

    // -------------------------------------------------------------
    // 몬스터스포너 생성
    // -------------------------------------------------------------
    private void CreateMonsterSpawner()
    {
        int MonsterSpawnerId;
        List<Dictionary<string, object>> monsterSpawnerData = CSVReader.Read("Datas/MonsterSpawner");
        List<Dictionary<string, object>> monsterData = CSVReader.Read("Datas/Monster");

        // 몬스터스포너 확률 리스트 생성
        List<float> monsterSpawnerProbList = new List<float>();
        for (int i = 0; i < monsterSpawnerData.Count; i++)
        {
            if(Floor == int.Parse(monsterSpawnerData[i]["Floor"].ToString()))
            {
                monsterSpawnerProbList.Add(float.Parse(monsterSpawnerData[i]["Prob"].ToString()));
            }
        }

        for (int roomIndex = 1; roomIndex < generator.Rooms.Count; roomIndex++) // 0번 방에는 스포너를 안만듬
        // foreach (DungeonRoom room in generator.Rooms)
        {
            if (generator.ShopIndex == roomIndex)
            {
                continue;
            }
            else if (generator.BossIndex == roomIndex)
            {
                MonsterSpawnerId = RandomSelect(monsterSpawnerProbList, true);
            }
            else
            {
                MonsterSpawnerId = RandomSelect(monsterSpawnerProbList, false);
            }
       
            string MonsterSpawnerPath = monsterSpawnerData[MonsterSpawnerId]["Path"].ToString();
            GameObject goSpawner = GameManager.Instance.CreateGO(MonsterSpawnerPath, generator.Rooms[roomIndex].transform);
            MonsterSpawner spawner = goSpawner.GetComponent<MonsterSpawner>();
            generator.Rooms[roomIndex].SetSpawner(spawner, roomIndex, monsterData);
        }
    }



    // -------------------------------------------------------------
    // 상점 생성
    // -------------------------------------------------------------
    private void CreateShop()
    {
        // 아이템 5개 일렬 배치
        // !!! 중복 아이템에 대한 처리 X
        for (int i = -2; i < 3; i++)
        {
            // DroppedItem 생성
            GameObject dropped = GameManager.Instance.CreateGO
            (
                "Prefabs/Dungeon/Dropped", 
                generator.Shop.transform
            );

            dropped.transform.position = new Vector3
            (
                i + generator.Shop.transform.position.x,
                generator.Shop.transform.position.y, 
                -1f
            );
            // !!! 아이템 가격 표 필요 (아이템 가격 1000 고정)
            Item randomItem = GameManager.Instance.GetRandomDropItem();
            dropped.GetComponent<DroppedItem>().Set(randomItem, 0);
        }
    }

    // csv파일에 기재된 확률에 의거해 무작위로 스포너 선택
    private int RandomSelect(List<float> list, bool boss)
    {
        if (boss)
        {
            return list.Count - 1;
        }

        float total = 0;
        foreach (float elem in list)
        {
            total += elem;
        }

        float randomPoint = Random.value * total;
        for (int i = 0; i < list.Count; i++)
        {
            if (randomPoint < list[i])
            {
                return i;
            }
            else
            {
                randomPoint -= list[i];
            }
        }

        return list.Count - 2;
    }
}