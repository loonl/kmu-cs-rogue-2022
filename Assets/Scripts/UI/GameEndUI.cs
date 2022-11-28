using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using PlayNANOO;

public class GameEndUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TMP_InputField inputField;

    // Update is called once per frame
    void Update()
    {
        scoreText.text = GameManager.Instance.score.ToString();
    }
    
    public void Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
    
    public void Quit()
    {
        Application.Quit();
    }
    
    public void SaveScore()
    {
        
        // GameManager.Instance.plugin.Storage.Save("BestScore", scoreText.text, false, (state, error, jsonString, values) => {
        //     if (state.Equals(Configure.PN_API_STATE_SUCCESS))
        //     {
        //         Debug.Log("Success");
        //     }
        //     else
        //     {
        //         Debug.Log("Fail");
        //     }
        // });
        NanooController.instance.plugin.AccountNickanmePut(inputField.text, false, (status, errorCode, jsonString, values) => {
            if (status.Equals(Configure.PN_API_STATE_SUCCESS))
            {
                Debug.Log(values["nickname"].ToString());
                NanooController.instance.plugin.RankingRecord("rouge-RANK-852BBDB6-3F56AA43", int.Parse(scoreText.text),"BestScore" , (state, message, rawData, dictionary) => {
                    if (state.Equals(Configure.PN_API_STATE_SUCCESS))
                    {
                        Debug.Log("랭킹 기록 완료");
                    }
                    else
                    {
                        Debug.Log("랭킹 기록 실패");
                    }
                });
            }
            else
            {
                if (values != null)
                {
                    if (values["ErrorCode"].ToString() == "30007")
                    {
                        Debug.Log(values["WithdrawalKey"].ToString());
                    }
                    else
                    {
                        Debug.Log("Fail");
                    }
                }
                else 
                {
                    Debug.Log("Fail");
                }
            }
        });
    }
}
