using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DungeonSystem : MonoBehaviour
{
    private static DungeonSystem instance = null;
    public static DungeonSystem Instance { get { return instance; } }

    public int Floor { get; private set; } = 1;

    [SerializeField]
    public RoomGenerator generator;

    [SerializeField]
    private Image fadeimg;
    private Color color = new Color(0, 0, 0, 0);
    private bool isrestart = false;

    private int tempRoomCount;

    public GameObject DroppedItems; // 떨어진 아이템 parent

    public List<DungeonRoom> Rooms { get { return generator.Rooms; } }
    public Dictionary<int, MonsterSpawner> monsterSpawners = new Dictionary<int, MonsterSpawner>(); // key: 방 번호, MonsterSpawner: 해당 방의 MonsterSpawner
    public int Currentroom;

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
        tempRoomCount = 7;
    }

    //테스트용 코드
    public void KillAll()
    {
        Rooms[Currentroom].KillAll();
    }

    public void Load()
    {
        // 현재 Floor 기준으로 레벨 디자인
        Floor++;
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
        generator.Generate(tempRoomCount + 3 * Floor, tileSeqence[(Floor - 1) % 4]);
        CreateMonsterSpawner();   // 몬스터스포너 생성
        CreateShop();       // 상점 생성
        DroppedItems = new GameObject() { name = "DroppedItems" };
        DroppedItems.transform.SetParent(this.transform); // 드랍 아이템 부모 생성
    }

    public void ClearDungeon()
    {
        // 맵 삭제
        generator.Clear();
        // 아이템 삭제
        Destroy(DroppedItems.gameObject);
        // 딕셔너리 초기화
        monsterSpawners.Clear();
        DroppedItems = null;
    }

    // -------------------------------------------------------------
    // 몬스터스포너 생성
    // -------------------------------------------------------------
    private void CreateMonsterSpawner()
    {
        int MonsterSpawnerId;
        List<Dictionary<string, object>> monsterSpawnerData = CSVReader.Read("Datas/MonsterSpawner");
        //List<Dictionary<string, object>> monsterSpawnerData = CSVReader.Read("Datas/TestMonsterSpawner"); // !!테스트 코드
        List<Dictionary<string, object>> monsterData = CSVReader.Read("Datas/Monster");
        int[] BehindMonsterSpawnerNumArr = new int[4];
        
        // 몬스터스포너 확률 리스트 생성
        List<float> monsterSpawnerProbList = new List<float>();
        for (int i = 0; i < monsterSpawnerData.Count; i++)
        {
            if (int.Parse(monsterSpawnerData[i]["Floor"].ToString()) == 1)
            {
                BehindMonsterSpawnerNumArr[2]++;
                BehindMonsterSpawnerNumArr[3]++;
            }
            else if (int.Parse(monsterSpawnerData[i]["Floor"].ToString()) == 2)
            {
                BehindMonsterSpawnerNumArr[3]++;
            }
            
            if (Floor == int.Parse(monsterSpawnerData[i]["Floor"].ToString()))
            {
                monsterSpawnerProbList.Add(float.Parse(monsterSpawnerData[i]["Prob"].ToString()));
            }
        }

        for (int roomIndex = 1; roomIndex < generator.Rooms.Count; roomIndex++) // 0번 방에는 스포너를 안만듬
        {
            if (generator.ShopIndex == roomIndex)
            {
                Rooms[generator.ShopIndex].Clear();
                continue;
            }
            else if (generator.BossIndex == roomIndex)
            {
                MonsterSpawnerId = RandomSelect(monsterSpawnerProbList, true) + BehindMonsterSpawnerNumArr[Floor];
            }
            else
            {
                MonsterSpawnerId = RandomSelect(monsterSpawnerProbList, false) + BehindMonsterSpawnerNumArr[Floor];
            }
            
            string MonsterSpawnerPath = monsterSpawnerData[MonsterSpawnerId]["Path"].ToString();
            GameObject goSpawner = GameManager.Instance.CreateGO(MonsterSpawnerPath, generator.Rooms[roomIndex].transform);
            MonsterSpawner spawner = goSpawner.GetComponent<MonsterSpawner>();
            generator.Rooms[roomIndex].SetSpawner(spawner, roomIndex, monsterData);
            monsterSpawners.Add(roomIndex, spawner); // (해당 방 번호, 몬스터 스포너)
        }
    }

    // -------------------------------------------------------------
    // 상점 생성
    // -------------------------------------------------------------
    private void CreateShop()
    {
        // 아이템 5개 일렬 배치
        // !!! 중복 아이템에 대한 처리 X
        // -2부터 하는 이유는 아이템 스폰 위치가 0이기 때문
        for (int i = -2; i < 3; i++)
        {
            // DroppedItem 생성
            GameObject dropped = GameManager.Instance.CreateGO
            (
                "Prefabs/Dungeon/Dropped", 
                generator.Shop.transform // ??
            );

            dropped.transform.position = new Vector3
            (
                i + generator.Shop.transform.position.x,
                generator.Shop.transform.position.y,
                -.1f
            );
            // !!! 아이템 가격 표 필요 (아이템 가격 1000 고정)
            Item randomItem = GameManager.Instance.GetRandomDropItem();
            DroppedItem item = dropped.GetComponent<DroppedItem>();
            item.Set(randomItem, 15);
            item.SetCanvas();

        }
        //물약 하나 상점에 추가
        GameObject droppedItem = GameManager.Instance.CreateGO
        (
            "Prefabs/Dungeon/Portion",
            generator.Shop.transform
        );

        droppedItem.transform.position = new Vector3
            (
                generator.Shop.transform.position.x,
                generator.Shop.transform.position.y + 1,
                -.1f
            );
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

    public void LevelClear()
    {
        StartCoroutine(Clear());
        // TODO - 임시 코드 !!!! 수정 필요
        SoundManager.Instance.SoundPlay(SoundType.BGM, index: Floor + 1);
    }

    private IEnumerator Clear()
    {
        isrestart = true;
        if (fadeimg != null)
            while (color.a < 1.0f)
            {
                color.a += 0.01f;
                yield return GameManager.Instance.Setwfs(1);
                fadeimg.color = color;
            }

        DungeonSystem.Instance.ClearDungeon();
        yield return GameManager.Instance.Setwfs(20);
        DungeonSystem.Instance.Load();
        GameManager.Instance.Player.transform.position = Vector3.zero;
        DungeonSystem.Instance.Rooms[0].Clear();

        if (fadeimg != null)
            while (color.a > 0.0f)
            {
                color.a -= 0.01f;
                yield return GameManager.Instance.Setwfs(1);
                fadeimg.color = color;
            }
        isrestart = false;
    }
}