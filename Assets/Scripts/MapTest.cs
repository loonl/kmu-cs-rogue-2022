using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapTest : MonoBehaviour
{
    WaitForSeconds wfs20 = new WaitForSeconds(0.2f);
    WaitForSeconds wfs1 = new WaitForSeconds(0.01f);
    [SerializeField]
    private Image fadeimg;
    float fade = 0.0f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(Restart());
        }
    }

    private IEnumerator Restart()
    {
        //while (fade < 1.0f)
        //{
        //    fade += 0.01f;
        //    yield return wfs1;
        //    fadeimg.color = new Color(0, 0, 0, fade);
        //}
        DungeonSystem.Instance.ClearDungeon();
        yield return wfs20;
        DungeonSystem.Instance.CreateDungeon();
        GameManager.Instance.Player.transform.position = Vector3.zero;
        DungeonSystem.Instance.Rooms[0].Clear();
        //GameManager.Instance.Player.EquipInit();
        //while (fade > 0.0f)
        //{
        //    fade -= 0.01f;
        //    yield return wfs1;
        //    fadeimg.color = new Color(0, 0, 0, fade);
        //}
    }
}
