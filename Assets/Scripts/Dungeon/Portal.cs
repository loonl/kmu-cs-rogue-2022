using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    private BoxCollider2D _collider;

    private Portal connetecPortal = null;
    private bool _isActivated;
    private ushort _outDirect;       // 내 포탈의 방향
    private int _connectedRoomId;
    private Vector3 offset;

    private bool entered = true;

    private void Awake()
    {
        _collider = this.GetComponent<BoxCollider2D>();

        DeActivate();
    }

    public void Connect(int roomId, ushort direct)
    {
        // 포탈과 연결될 방 id 설정
        _connectedRoomId = roomId;
        _outDirect = direct;


        if (direct == 0)
        {
            offset = new Vector3(0, -.5f, 0);
        }
        else if (direct == 1)
        {
            offset = new Vector3(-.5f, 0, 0);
        }
        else if (direct == 2)
        {
            offset = new Vector3(0, .5f, 0);
        }
        else
        {
            offset = new Vector3(.5f, 0, 0);
        }

        // return generator.Rooms[dungeonRoomIndex].Portals[(direct + 2) % 4].transform.position + offset * 0.5f;

        

        // 방향 지정 및 문 방향으로 scale 조정
        // if (direct == 0 || direct == 2)
        // {
        //     this.transform.localScale = new Vector3(1f, 1.1f, 1f);
        // }
        // else
        // {
        //     this.transform.localScale = new Vector3(1.1f, 1f, 1f);
        // }
    }

    // -------------------------------------------------------------
    // 포탈 동작
    // -------------------------------------------------------------
    public void Enter(GameObject entering)
    {
        // connetecPortal
        entering.transform.position = this.transform.position + offset;
        this.entered = false;
    }

    public void Exit(GameObject exiting)
    {
        DungeonSystem.Instance.Rooms[_connectedRoomId].Enter(_outDirect, exiting);


        // Debug.Log();
        // DungeonSystem.Instance.GetConnetectedPortal(_connectedRoomId, _outDirect).Enter(exiting);
    }

    // -------------------------------------------------------------
    // 포탈 활성화
    // -------------------------------------------------------------
    public void Activate()
    {
        // 포탈 활성화
        _collider.enabled = true;
        _isActivated = true;
    }

    public void DeActivate()
    {
        // 포탈 비활성화
        _collider.enabled = false;
        _isActivated = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player") 
        {
            if (entered)
            {
                Exit(other.gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player") 
        {
            entered = true;
        }
    }
}
