using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;
    public static GameManager Instance { get { return _instance; } }
    public Player Player { get; private set; }

    public StageUIManager stageUIManager;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
            this.Player = GameObject.Find("Player").GetComponent<Player>();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        stageUIManager.init(Player);
        DungeonSystem.Instance.CreateDungeon();
        DungeonSystem.Instance.Rooms[0].Clear();    // 첫번째 방은 클리어 된 상태
    }

    // -------------------------------------------------------------
    // 프리팹 생성
    // -------------------------------------------------------------
    public GameObject CreateGO(string url, Transform parent)
    {
        Object obj = Resources.Load(url);
        GameObject go = Instantiate(obj) as GameObject;
        go.transform.SetParent(parent);

        return go;
    }

    public List<Item> GetRandomItemList(int floor, float N = 0f, float R = 0f, float SR = 0f, float SSR = 0f)
    {
        List<Item> itemList = new List<Item>();
        List<int> indexList = new List<int>();

        if (N == 0f && R == 0f && SR == 0f && SSR == 0f)
        {
            // TO-DO floor에 따라 확률식 조절하기
            N = 0.25f;
        }

        R = N + 0.25f;
        SR = SR + 0.25f;
        SSR = SSR + 0.25f;

        int number = UnityEngine.Random.Range(3, 6);
        for (int i = 0; i < number; i++)
        {
            int rarity;

            float temp = Random.Range(0f, 1f);
            if (temp <= N)
                rarity = 0;
            else if (N < temp && temp <= R)
                rarity = 1;
            else if (R < temp && temp <= SR)
                rarity = 2;
            else
                rarity = 3;

            int itemIndex = -1;
            switch (rarity)
            {
                case 0:
                    itemIndex = Random.Range(2, 22);

                    while (indexList.Contains(itemIndex))
                    {
                        itemIndex++;
                        if (itemIndex > 21)
                            itemIndex = 2;
                    }

                    break;
                case 1:
                    itemIndex = Random.Range(22, 50);

                    while (indexList.Contains(itemIndex))
                    {
                        itemIndex++;
                        if (itemIndex > 49)
                            itemIndex = 22;
                    }

                    break;
                case 2:
                    itemIndex = Random.Range(50, 77);

                    while (indexList.Contains(itemIndex))
                    {
                        itemIndex++;
                        if (itemIndex > 76)
                            itemIndex = 50;
                    }

                    break;
                case 3:
                    itemIndex = Random.Range(77, 90);

                    while (indexList.Contains(itemIndex))
                    {
                        itemIndex++;
                        if (itemIndex > 89)
                            itemIndex = 77;
                    }

                    break;
            }

            indexList.Add(itemIndex);
        }

        foreach (int idx in indexList) 
            itemList.Add(ItemManager.Instance.GetItem(idx));

        return itemList;
    }

    public Item GetRandomDropItem(int floor = 0)
    {
        List<Item> randomList = GetRandomItemList(floor);

        int x = (int)Random.Range(0f, (float)randomList.Count);
        return randomList[x];
    }
}
