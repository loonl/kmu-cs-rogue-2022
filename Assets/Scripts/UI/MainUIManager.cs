using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MainUIManager : MonoBehaviour
{
    public GameObject FadeIMG;
    float fadeTime = 3f;
    // Start is called before the first frame update
    private void Awake()
    {
        StartCoroutine("FadeIn");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LoadNextScene()
    {
        FadeIMG.SetActive(true);
        StartCoroutine("FadeOut");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Slider volumeSlider;
    [SerializeField] string parameterName = "";

    public void OnValueChanged()
    {
        audioMixer.SetFloat(parameterName,
        (volumeSlider.value <= volumeSlider.minValue) ? -80f : volumeSlider.value);
    }

    public IEnumerator FadeIn()
    {
        Image image = FadeIMG.GetComponent<Image>();
        Color color = image.color;
        while (color.a > 0f)
        {
            color.a -= Time.deltaTime / fadeTime;
            image.color = color;
            yield return null;
        }
        FadeIMG.SetActive(false);

    }

    public IEnumerator FadeOut()
    {
        Image image = FadeIMG.GetComponent<Image>();
        Color color = image.color;
        while (color.a < 1f)
        {
            color.a += Time.deltaTime / fadeTime;
            image.color = color;
            yield return null;
        }
        Debug.Log(1);

        SceneManager.LoadScene("StageTest");
    }
}
