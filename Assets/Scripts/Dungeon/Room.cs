using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    private int _x;      // 가로 축
    private int _y;      // 세로 축
    public int Id { get; private set; }        // Room ID

    private List<RoomDirect> emptyDirects;
    private List<RoomDirect> existDirects;
    private Room top;        // y - 1 room
    private Room right;      // x + 1 room
    private Room down;       // y + 1 room
    private Room left;       // x - 1 room

    private GameObject roomObject;

    public int X { get { return _x; } set { _x = value; } }
    public int Y { get { return _y; } set { _y = value; } }
    public List<RoomDirect> EmptyDirects { get { return emptyDirects; } }
    public List<RoomDirect> ExistDirects { get { return existDirects; } }

    public GameObject RoomObject { get { return roomObject; } }

    // -------------------------------------------------------------
    // Room 초기화 및 연결(검색)
    // -------------------------------------------------------------
    public Room()
    {
        _x = 0;
        _y = 0;
        Id = 0;
        emptyDirects = new List<RoomDirect>() { RoomDirect.Top, RoomDirect.Right, RoomDirect.Down, RoomDirect.Left };
        existDirects = new List<RoomDirect>();
    }

    public Room(int x, int y, int roomId)
    {
        _x = x;
        _y = y;
        Id = roomId;
        emptyDirects = new List<RoomDirect>() { RoomDirect.Top, RoomDirect.Right, RoomDirect.Down, RoomDirect.Left };
        existDirects = new List<RoomDirect>();
    }

    public void UpdateCoorinate(RoomDirect direct)
    {
        // 기존 좌표에서 direct 이동한 좌표로 수정
        switch (direct)
        {
            case RoomDirect.Top:
                _y += 1;
                break;
            case RoomDirect.Right:
                _x += 1;
                break;
            case RoomDirect.Down:
                _y -= 1;
                break;
            case RoomDirect.Left:
                _x -= 1;
                break;
        }
    }

    public void InterconnectRoom(Room newRoom, RoomDirect direct)
    {
        // 기존 room에서 새로운 room 상호 연결 (양방향 연결)
        switch (direct)
        {
            case RoomDirect.Top:
                this.ConnectRoom(newRoom, RoomDirect.Top);
                newRoom.ConnectRoom(this, RoomDirect.Down);
                break;
            case RoomDirect.Right:
                this.ConnectRoom(newRoom, RoomDirect.Right);
                newRoom.ConnectRoom(this, RoomDirect.Left);
                break;
            case RoomDirect.Down:
                this.ConnectRoom(newRoom, RoomDirect.Down);
                newRoom.ConnectRoom(this, RoomDirect.Top);
                break;
            case RoomDirect.Left:
                this.ConnectRoom(newRoom, RoomDirect.Left);
                newRoom.ConnectRoom(this, RoomDirect.Right);
                break;
        }
    }

    public void ConnectRoom(Room room, RoomDirect direct)
    {
        // 단방향 연결
        switch (direct)
        {
            case RoomDirect.Top:
                top = room;
                emptyDirects.Remove(RoomDirect.Top);
                existDirects.Add(RoomDirect.Top);
                break;
            case RoomDirect.Right:
                right = room;
                emptyDirects.Remove(RoomDirect.Right);
                existDirects.Add(RoomDirect.Right);
                break;
            case RoomDirect.Down:
                down = room;
                emptyDirects.Remove(RoomDirect.Down);
                existDirects.Add(RoomDirect.Down);
                break;
            case RoomDirect.Left:
                left = room;
                emptyDirects.Remove(RoomDirect.Left);
                existDirects.Add(RoomDirect.Left);
                break;
        }
    }

    // -------------------------------------------------------------
    // Room GameObject Set
    // -------------------------------------------------------------
    public void SetRoomObject(GameObject room)
    {
        roomObject = room;
    }

    public int GetConnectedRoomId(ushort roomDirect)
    {
        if (roomDirect == 0)
        {
            return top.Id;
        }
        else if (roomDirect == 1)
        {
            return right.Id;
        }
        else if (roomDirect == 2)
        {
            return down.Id;
        }
        else
        {
            return left.Id;
        }
    }
}