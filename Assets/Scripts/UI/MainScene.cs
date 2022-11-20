using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScene : MonoBehaviour
{
    [SerializeField] private GameObject light;
    [SerializeField] private GameObject light2;
    
    //다음 씬으로 전환
    public void NextScene()
    {
        light.SetActive(false);
        light2.SetActive(false);
            
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }
    
    //게임 종료
    public void QuitGame()
    {
        Application.Quit();
    }
}
