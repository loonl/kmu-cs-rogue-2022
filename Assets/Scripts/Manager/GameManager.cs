using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;
    public static GameManager Instance { get { return _instance; } }
    public Player Player { get; set; }

    Dictionary<int, WaitForSeconds> wfs = new Dictionary<int, WaitForSeconds>();
    
    public int score; // 게임 점수

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        InitMain();
    }
    // -------------------------------------------------------------
    // MainScene 시작
    // -------------------------------------------------------------
    public void InitMain()
    {
        // Scene과 BGM 전환
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        SoundManager.Instance.SoundPlay(SoundType.BGM, index: 0);
        
        // 다음 게임 시작을 위해 null로 변경
        Player = null;
    }
    

    // -------------------------------------------------------------
    // Game 시작
    // -------------------------------------------------------------
    public void InitGame()
    {
        // GameScene 으로 변경
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");

        // BGM 변경
        SoundManager.Instance.SoundPlay(SoundType.BGM, index: 1);
        
        // 로딩할 시간 부여
        Invoke("InitGame2", 0.5f);
    }

    private void InitGame2()
    {
        // 던전 만들어주기
        DungeonSystem.Instance.CreateDungeon();
        DungeonSystem.Instance.Rooms[0].Clear();    // 첫번째 방은 클리어 된 상태
        score = 0;
        
        // test code - TODO

        //NanooController.instance.SetPlugin();

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
    
    // -------------------------------------------------------------
    // 랜덤으로 선택된 아이템 리스트 반환
    // -------------------------------------------------------------
    public List<Item> GetRandomItemList(int Level = 0, float N = 0f, float R = 0f, float SR = 0f, float SSR = 0f)
    {
        List<Item> itemList = new List<Item>();
        List<int> indexList = new List<int>();
        
        // 확률을 직접 지정해줬다면 그것에 맞춰서 생성
        if (!(N == 0f && R == 0f && SR == 0f && SSR == 0f))
        {
            R += N;
            SR += R;
            SSR += SR;
        }
        else // Floor만 지정해줬다면 사전 설정한 값에 의해 설정
        {
            switch (DungeonSystem.Instance.Floor)
            {
                case 1:
                    N = 0.7f;
                    R = N + 0.2f;
                    SR = R + 0.1f;
                    SSR = SR + 0.0f;
                    break;
                case 2:
                    N = 0.1f;
                    R = N + 0.5f;
                    SR = R + 0.35f;
                    SSR = SR + 0.05f;
                    break;
                case 3:
                    N = 0f;
                    R = N + 0f;
                    SR = R + 0.75f;
                    SSR = SR + 0.25f;
                    break;
            }
        }

        int start = 0, end = 0;
        // 각 부위별로 하나씩 담아주기
        
        // 1. 무기
        string weaponRarity = GetRarity(N, R, SR, SSR);
        switch (weaponRarity)
        {
            // TODO - 임시로 하드코딩 :: 이거도 CSV로 관리하면 좋을 듯?
            case "N":
                start = 5;
                end = 9;
                break;
            case "R":
                start = 9;
                end = 20;
                break;
            case "SR":
                start = 20;
                end = 31;
                break;
            case "SSR":
                start = 31;
                end = 34;
                break;
        }
        
        indexList.Add(Random.Range(start, end));
        
        // 2. 투구
        string helmetRarity = GetRarity(N, R, SR, SSR);
        switch (helmetRarity)
        {
            case "N":
                start = 34;
                end = 36;
                break;
            case "R":
                start = 46;
                end = 51;
                break;
            case "SR":
                start = 62;
                end = 68;
                break;
            case "SSR":
                start = 77;
                end = 81;
                break;
        }
        
        indexList.Add(Random.Range(start, end));
        
        // 3. 방어구
        string armorRarity = GetRarity(N, R, SR, SSR);
        switch (armorRarity)
        {
            case "N":
                start = 36;
                end = 40;
                break;
            case "R":
                start = 51;
                end = 55;
                break;
            case "SR":
                start = 68;
                end = 71;
                break;
            case "SSR":
                start = 81;
                end = 82;
                break;
        }
        
        indexList.Add(Random.Range(start, end));
        
        // 4. 바지
        string pantsRarity = GetRarity(N, R, SR, SSR);
        switch (pantsRarity)
        {
            case "N":
                start = 40;
                end = 43;
                break;
            case "R":
                start = 55;
                end = 59;
                break;
            case "SR":
                start = 71;
                end = 75;
                break;
            case "SSR":
                start = 82;
                end = 84;
                break;
        }
        
        indexList.Add(Random.Range(start, end));
        
        // 5. 방패
        string shieldRarity = GetRarity(N, R, SR, SSR);
        switch (shieldRarity)
        {
            case "N":
                start = 43;
                end = 46;
                break;
            case "R":
                start = 59;
                end = 62;
                break;
            case "SR":
                start = 75;
                end = 77;
                break;
            case "SSR":
                start = 84;
                end = 85;
                break;
        }
        
        indexList.Add(Random.Range(start, end));
        
        // 뽑은 Index를 바탕으로 Item 생성 후 담아주기
        foreach (int idx in indexList) 
            itemList.Add(ItemManager.Instance.GetItem(idx));

        return itemList;
    }
    
    
    // -------------------------------------------------------------
    // 랜덤으로 선택된 아이템 반환
    // -------------------------------------------------------------
    public Item GetRandomDropItem(int floor = 0)
    {
        List<Item> randomList = GetRandomItemList(floor);

        int x = (int)Random.Range(0f, (float)randomList.Count);
        return randomList[x];
    }

    // -------------------------------------------------------------
    // 코루틴 사용할 때 WaitForSeconds 사용하는 함수
    // -------------------------------------------------------------
    public WaitForSeconds Setwfs(int time)
    {
        if (wfs.ContainsKey(time))
            return wfs[time];

        else
        {
            wfs.Add(time, new WaitForSeconds(time * 0.01f));
            return wfs[time];
        }
    }
    
    // -------------------------------------------------------------
    // Rarity 골라주는 함수
    // -------------------------------------------------------------
    private string GetRarity(float N, float R, float SR, float SSR)
    {
        float temp = Random.Range(0f, 1f);
        if (temp <= N)
            return "N";
        else if (temp > N && temp <= R)
            return "R";
        else if (temp > R && temp <= SR)
            return "SR";
        else
            return "SSR";
    }
}
