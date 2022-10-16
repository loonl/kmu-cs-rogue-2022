using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    private BoxCollider2D _collider;
    [SerializeField]
    private Portal connetecPortal = null;
    [SerializeField]
    private bool _isActivated;
    [SerializeField]
    private ushort _outDirect;       // 내 포탈의 방향
    [SerializeField]
    private int _connectedRoomId;
    [SerializeField]
    private Vector3 offset;

    [SerializeField]
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
            offset = new Vector3(0, -1f, 0);
        }
        else if (direct == 1)
        {
            offset = new Vector3(-.75f, 0, 0);
        }
        else if (direct == 2)
        {
            offset = new Vector3(0, .75f, 0);
        }
        else
        {
            offset = new Vector3(.75f, 0, 0);
        }
    }

    // -------------------------------------------------------------
    // 포탈 동작
    // -------------------------------------------------------------
    public void Enter(GameObject entering)
    {
        // connetecPortal
        entering.transform.position = this.transform.position + offset;
        //this.entered = false;
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
