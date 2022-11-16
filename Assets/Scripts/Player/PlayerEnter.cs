using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnter : MonoBehaviour
{
    private Player player;

    private int currentRoom;
    // Start is called before the first frame update
    void Start()
    {
        player = gameObject.GetComponent<Player>();
        currentRoom = DungeonSystem.Instance.Currentroom;
    }

    // Update is called once per frame
    void Update()
    {
        // 현재 위치해 있던 방이 바뀌었다면
        if (currentRoom != DungeonSystem.Instance.Currentroom)
        {
            currentRoom = DungeonSystem.Instance.Currentroom;
            
            // 1초동안 무적 부여
            StartCoroutine(player.Grace(50));
        }
    }
}
