using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayNANOO;

public class RankManager : MonoBehaviour
{
    public GameObject rankPrefab;
    // Start is called before the first frame update

    private void Start()
    {
        foreach (Dictionary<string, object> item in (ArrayList)NanooController.instance.list["items"])
        {
            rankPrefab = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Rankprefab"));
            rankPrefab.transform.GetComponent<SetRankTxt>().SetRank(item["ranking"].ToString(),
                item["nickname"].ToString(), item["score"].ToString());
            rankPrefab.transform.parent = transform;

        }
    }
    
    public void BackScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
    }
}
