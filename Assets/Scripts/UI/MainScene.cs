using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScene : MonoBehaviour
{
    [SerializeField] private GameObject light;
    [SerializeField] private GameObject light2;

    private void Awake()
    {
        // if (NanooController.instance ==null)
        // {
        //     Instantiate(Resources.Load<GameObject>("Prefabs/UI/NanooManager"));
        // }
        // NanooController.instance.SetPlugin();
    }

    //다음 씬으로 전환
    public void NextScene()
    {
        light.SetActive(false);
        light2.SetActive(false);

        GameManager.Instance.InitGame();
    }
    
    public void LoadScoreScene()
    {
        // NanooController.instance.plugin.RankingRange("rouge-RANK-852BBDB6-3F56AA43", 1, 10, (status, errorMessage, jsonString, values) =>
        // {
        //     if (status.Equals(Configure.PN_API_STATE_SUCCESS))
        //     {
        //         NanooController.instance.list = values;
        //         Debug.Log("Load Success");
        //         UnityEngine.SceneManagement.SceneManager.LoadScene("RankBoard");
        //     }
        //     else
        //     {
        //         Debug.Log("Fail");
        //     }
        // });
        // light.SetActive(false);
        // light2.SetActive(false);
    }
    
    //게임 종료
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
