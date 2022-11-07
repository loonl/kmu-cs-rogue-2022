using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    public GameObject DroppedItems;        // 떨어진 아이템 parent

    public List<DungeonRoom> Rooms { get { return generator.Rooms; } }
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
        tempRoomCount = 13;
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
        generator.Generate(tempRoomCount + 2 * Floor, tileSeqence[(Floor - 1) % 4]);
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
        DroppedItems = null;
    }

    // -------------------------------------------------------------
    // 몬스터스포너 생성
    // -------------------------------------------------------------
    private void CreateMonsterSpawner()
    {
        int MonsterSpawnerId;
        List<Dictionary<string, object>> monsterSpawnerData = CSVReader.Read("Datas/MonsterSpawner");
        //List<Dictionary<string, object>> monsterSpawnerData = CSVReader.Read("Datas/TestMonsterSpawner"); // 테스트 코드
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
        {
            if (generator.ShopIndex == roomIndex)
            {
                Rooms[generator.ShopIndex].Clear();
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
        // -2부터 하는 이유는 아이템 스폰 위치가 0이기 때문
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
                -.1f
            );
            // !!! 아이템 가격 표 필요 (아이템 가격 1000 고정)
            Item randomItem = GameManager.Instance.GetRandomDropItem();
            dropped.GetComponent<DroppedItem>().Set(randomItem, 0);
            
            // UI 함수화 필요(수정중)
            GameObject canvas = Resources.Load<GameObject>("Prefabs/UI/MonsterHPCanvas");
            canvas = Instantiate(canvas, dropped.transform.position, Quaternion.identity);
            canvas.transform.SetParent(dropped.transform);
            canvas.transform.localPosition = new Vector3(0, -1f, 0);
            canvas.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            GameObject hpBarPrefab = Resources.Load<GameObject>("Prefabs/UI/ShopTxt");
            GameObject hpBar = Instantiate(hpBarPrefab, transform.position, Quaternion.identity);
            hpBar.GetComponent<TextMeshProUGUI>().text = dropped.GetComponent<DroppedItem>()._price.ToString();
            hpBar.transform.SetParent(canvas.transform);
            hpBar.transform.localPosition = new Vector3(0, 0, 0);
            hpBar.transform.localScale = new Vector3(0.01f, 0.01f,0);
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
    public void LevelClear()
    {
        StartCoroutine(Clear());
    }
    private IEnumerator Clear()
    {
        DungeonSystem.Instance.ClearDungeon();
        yield return new WaitForSeconds(0.2f);
        DungeonSystem.Instance.Load();
        GameManager.Instance.Player.transform.position = Vector3.zero;
        DungeonSystem.Instance.Rooms[0].Clear();
    }
}