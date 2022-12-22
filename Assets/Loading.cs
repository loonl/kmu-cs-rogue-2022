using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    public static string nextScene;
    [SerializeField] private Slider progressBar;

    private void Start()
    {
        StartCoroutine(Load());
    }
    
    IEnumerator Load()
    {
        float timer = 0.0f;
        while (timer<1.0f)
        {
            timer += 0.01f;
            if (timer < 0.9f)
            {
                progressBar.value = timer;
                yield return new WaitForSeconds(0.01f);
            }
            else
            {
                progressBar.value = 1.0f;
                if (progressBar.value == 1.0f)
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }
}
