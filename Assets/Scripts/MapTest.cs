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
    Color color = new Color(0, 0, 0, 0);
    bool isrestart = false;

    // 치트키
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isrestart)
        {
            StartCoroutine(Restart());
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            StartCoroutine(TestClear());
        }
        if (Input.GetKeyDown(KeyCode.K))
            DungeonSystem.Instance.KillAll();
    }
    
    private IEnumerator Restart()
    {
        isrestart = true;
        if(fadeimg != null)
            while (color.a < 1.0f)
            {
                color.a += 0.01f;
                yield return GameManager.Instance.Setwfs(1);
                fadeimg.color = color;
            }

        DungeonSystem.Instance.ClearDungeon();
        yield return GameManager.Instance.Setwfs(20);
        DungeonSystem.Instance.CreateDungeon();
        GameManager.Instance.Player.transform.position = Vector3.zero;
        DungeonSystem.Instance.Rooms[0].Clear();
        
        if (fadeimg != null)
            while (color.a > 0.0f)
            {
                color.a -= 0.01f;
                yield return GameManager.Instance.Setwfs(1);
                fadeimg.color = color;
            }
        isrestart = false;
    }

    private IEnumerator TestClear()
    {
        if (fadeimg != null)
            while (color.a < 1.0f)
            {
                color.a += 0.01f;
                yield return wfs1;
                fadeimg.color = color;
            }

        DungeonSystem.Instance.ClearDungeon();
        yield return wfs20;
        DungeonSystem.Instance.Load();
        GameManager.Instance.Player.transform.position = Vector3.zero;
        DungeonSystem.Instance.Rooms[0].Clear();

        if (fadeimg != null)
            while (color.a > 0.0f)
            {
                color.a -= 0.01f;
                yield return wfs1;
                fadeimg.color = color;
            }
    }
}
