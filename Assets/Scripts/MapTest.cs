using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTest : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            DungeonSystem.Instance.ClearDungeon();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DungeonSystem.Instance.ClearDungeon();
            DungeonSystem.Instance.CreateDungeon();
            GameManager.Instance.Player.transform.position = Vector3.zero;
            DungeonSystem.Instance.Rooms[0].Clear();
        }
    }
}
