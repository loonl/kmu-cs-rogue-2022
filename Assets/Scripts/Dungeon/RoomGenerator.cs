using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using System;

public enum TileType
{
    DefaultGround = 0,
    DefaultWall = 1,
    DefaultEdge = 2,
    WaterGround = 3,
    WaterWall = 4,
    WaterEdge = 5,
    MossGround = 6,
    MossWall = 7,
    MossEdge = 8,
    MossWaterGround = 9,
    MossWaterWall = 10,
    MossWaterEdge = 11,
    VineGround = 12,
    VineWall = 13,
    VineEdge = 14,
    VineMossGround = 15,
    VineMossWall = 16,
    VineMossEdge = 17,
    VineMossWaterGround = 18,
    VineMossWaterWall = 19,
    VineMossWaterEdge = 20,
    DefaultOpenDoor = 21,
    DefaultCloseDoor = 22,
    MossOpenDoor = 23,
    MossCloseDoor = 24,
    VineOpenDoor = 25,
    VineCloseDoor = 26,
    VineMossOpenDoor = 27,
    VineMossCloseDoor = 28,
    BossGround = 29
}

public enum RoomSize
{
    Small,
    Medium
}

public enum TileDirect
{
    Default = 0,
    Right = 1,
    Bottom = 2,
    Left = 3
}

public enum RoomDirect
{
    Top = 0,
    Right = 1,
    Down = 2,
    Left = 3
}

public class RoomGenerator : MonoBehaviour
{
    private float roomWidth;
    private int maxRoomCount;
    private int roomCount;

    [Header ("Room")]
    [SerializeField]
    private GameObject roomParent;

    [Header ("Door Tiles")]
    [SerializeField]
    private List<Tile> allDoors;   // Default, Moss, Vine, VineMoss

    [Header ("Tiles")]
    [SerializeField]
    private List<Tile> defaultGrounds;
    [SerializeField]
    private List<Tile> defaultWalls;
    [SerializeField]
    private List<Tile> defaultEdges;
    [SerializeField]
    private List<Tile> mossGrounds;
    [SerializeField]
    private List<Tile> mossWalls;
    [SerializeField]
    private List<Tile> mossEdges;
    [SerializeField]
    private List<Tile> vineWalls;
    [SerializeField]
    private List<Tile> vineEdges;
    [SerializeField]
    private List<Tile> vineMossWalls;
    [SerializeField]
    private List<Tile> vineMossEdges;
    [SerializeField]
    private List<Tile> BossGrounds;

    [Header("Object")]
    [SerializeField]
    private GameObject emptyRoomPref;
    [SerializeField]
    private GameObject portalPref;
    [SerializeField]
    private GameObject Firetorch;
    [SerializeField]
    private GameObject boxesPref;
    [SerializeField]
    private GameObject ObstaclePref;
    [SerializeField]
    private GameObject HolePref;

    private List<Room> rooms;            // 모든 방 리스트
    private Stack<Room> visitedRooms;
    private List<DungeonRoom> dungeonRooms;
    public List<DungeonRoom> Rooms { get { return dungeonRooms; } }
    [SerializeField]
    private int[] distance; //첫 방과 n번째 방의 거리
    private int specialroom; //보스방, 상점방 등 특수한 방의 개수
    private int[] dx = new int[] { 1, 0, -1, 0 };
    private int[] dy = new int[] { 0, 1, 0, -1 };
    private int[,] roomlocal;//지도

    private Queue<int> currentcheckroom = new Queue<int>();
    private int shopIndex;
    private int bossIndex;

    public DungeonRoom Shop { get { return dungeonRooms[shopIndex]; } private set { value = dungeonRooms[shopIndex]; } }
    public DungeonRoom Boss { get { return dungeonRooms[bossIndex]; } private set { value = dungeonRooms[bossIndex]; } }
    public int ShopIndex { get { return shopIndex; } }
    public int BossIndex { get { return bossIndex; } }

    private void Awake()
    {
        // default
        roomCount = 0;
        shopIndex = 0;
        bossIndex = 0;
        roomWidth = 20;
        rooms = new List<Room>();
        visitedRooms = new Stack<Room>();
        dungeonRooms = new List<DungeonRoom>();
        if (Firetorch == null)
            Firetorch = Resources.Load<GameObject>("Prefabs/Dungeon/Firetorch");
    }

    // -------------------------------------------------------------
    // 던전 생성, 초기화
    // -------------------------------------------------------------
    public void Generate(int maxCount, TileType type)
    {
        // 빈 방 생성
        CreateEmptyRoom(maxCount);
        Distance();
        CreateSpecialRoom();

        for(int i = 0; i < dungeonRooms.Count; i++)
        {
           DrawRoom(dungeonRooms[i], type, i);
        }

        // 문 생성
        GenerateDoors(type);

        //보스 방과 연결된 포탈에 보스 방이라는 것을 알려주는 횃불 설치 
        NearBossRoomDraw();

        // 오브젝트 생성
        GenerateObject();
    }

    public void Clear()
    {
        // 생성된 모든 room object 삭제
        foreach (DungeonRoom room in dungeonRooms)
        {
            Destroy(room.gameObject);
        }

        // 초기화
        dungeonRooms.Clear();
        roomCount = 0;
        rooms.Clear();
        visitedRooms.Clear();
        currentcheckroom.Clear();
        distance.Initialize();
    }

    // -------------------------------------------------------------
    // Empty room 생성 및 연결
    // -------------------------------------------------------------
    private void CreateEmptyRoom(int maxCount)
    {
        roomCount = 0;
        maxRoomCount = maxCount;
        bossIndex = maxRoomCount - 2;
        shopIndex = maxRoomCount - 1;
        roomlocal = new int[maxRoomCount * 2 - 1, maxRoomCount * 2 - 1];
        Room selectRoom = new Room();       // 시작 방 생성
        rooms.Add(selectRoom);
        visitedRooms.Push(selectRoom);
        GameObject initRoomObj = Instantiate(emptyRoomPref, this.transform.position, Quaternion.identity);
        dungeonRooms.Add(initRoomObj.GetComponent<DungeonRoom>());
        initRoomObj.transform.parent = roomParent.transform;
        initRoomObj.name = roomCount.ToString();
        selectRoom.SetRoomObject(initRoomObj);
        roomCount += 1;
        roomlocal[maxRoomCount - 1 + selectRoom.X, maxRoomCount - 1 + selectRoom.Y] = roomCount;

        while (roomCount < maxRoomCount - 2)
        {
            if (selectRoom.EmptyDirects.Count == 0)
            {
                // 인접한 방이 없는 경우, 이전 room 중에서 인접 방을 찾음
                visitedRooms.Pop();
                selectRoom = visitedRooms.Peek();        // 이전 room으로 다시 인접 빈 방 검색
            }
            else
            {
                // before room index로 새 room 생성
                Room newRoom = new Room(selectRoom.X, selectRoom.Y, roomCount);
                // selectRoom에 인접한 빈 room 중 랜덤하게 선택하여 selectRoom과 상호 연결
                RoomDirect selected = selectRoom.EmptyDirects[UnityEngine.Random.Range(0, selectRoom.EmptyDirects.Count)];
                selectRoom.InterconnectRoom(newRoom, selected);
                newRoom.UpdateCoorinate(selected);  // newRoom 좌표 재설정

                // newRoom에 인접한 기존 room이 있으면 서로 연결
                ConnectNearRoom(newRoom);

                // Room 좌표 위치에 groundPref 생성
                Vector3 roomPos = new Vector3(newRoom.X, newRoom.Y, 0f) * roomWidth;
                // newRoom.SetRoomObject(Instantiate(groundPref, roomPos, Quaternion.identity));
                GameObject newRoomObj = Instantiate(emptyRoomPref, roomPos, Quaternion.identity);
                dungeonRooms.Add(newRoomObj.GetComponent<DungeonRoom>());
                newRoomObj.transform.parent = roomParent.transform;
                newRoomObj.name = roomCount.ToString();
                newRoom.SetRoomObject(newRoomObj);
                selectRoom = newRoom;
                rooms.Add(selectRoom);
                visitedRooms.Push(selectRoom);
                roomCount += 1;
                roomlocal[maxRoomCount - 1 + newRoom.X, maxRoomCount - 1 + newRoom.Y] = roomCount;
            }
        }
    }

    // -------------------------------------------------------------
    // room 거리 계산
    // -------------------------------------------------------------
    private void Distance()
    {
        int currentnum, current, num = 0;
        int x, y, tempx, tempy;
        distance = new int[roomCount];
        currentcheckroom.Enqueue(0);

        while (roomCount > 0)
        {
            currentnum = currentcheckroom.Count;
            roomCount -= currentnum;
            for (int i = 0; i < currentnum; i++)
            {
                current = currentcheckroom.Dequeue();
                x = rooms[current].X;
                y = rooms[current].Y;
                for (int j = 0; j < rooms[current].ExistDirects.Count; j++)
                {
                    tempx = x;
                    tempy = y;
                    switch (rooms[current].ExistDirects[j])
                    {
                        case RoomDirect.Top:
                            tempy++;
                            break;
                        case RoomDirect.Right:
                            tempx++;
                            break;
                        case RoomDirect.Down:
                            tempy--;
                            break;
                        case RoomDirect.Left:
                            tempx--;
                            break;
                    }

                    for (int k = 1; k < distance.Length; k++)
                    {
                        if (rooms[k].X == tempx && rooms[k].Y == tempy)
                        {
                            if (distance[k] == 0 && !currentcheckroom.Contains(k))
                                currentcheckroom.Enqueue(k);
                            break;
                        }
                    }
                }
                distance[current] = num;
            }
            num++;
        }
    }

    // -------------------------------------------------------------
    // 방을 생성할 곳 주변에 방이 있는지(막다른 방이 될 수 있는지)
    // -------------------------------------------------------------
    private bool Searchroom(int x, int y, RoomDirect roomDirect)
    {
        int flag = 0;
        int tempx = x + maxRoomCount - 1, tempy = y + maxRoomCount - 1;
        switch (roomDirect)
        {
            case RoomDirect.Top:
                tempy++;
                break;

            case RoomDirect.Right:
                tempx++;
                break;

            case RoomDirect.Left:
                tempx--;
                break;

            case RoomDirect.Down:
                tempy--;
                break;
        }

        for(int i = 0; i < 4; i++)
        {
            if (roomlocal[tempx + dx[i], tempy + dy[i]] != 0)
                flag++;
            //방 하나와는 연결되어 있어야 하므로
            if (flag == 2)
                return false;
        }
        return true;
    }

    // -------------------------------------------------------------
    // 특수 방 제작(상점, 보스방)
    // -------------------------------------------------------------
    private void CreateSpecialRoom()
    {
        //특수 방이 현재 두 개이므로
        int count = maxRoomCount - 3;
        while (count < maxRoomCount - 1)
        {
            //구해놓은 distance에서 가장 먼 값을 빼온다
            Room farthestroom = rooms[distance.ToList().IndexOf(distance.Max())];
            //가장 먼 방의 주변에 자리가 있을 경우 방을 만들 수 있다
            if (farthestroom.EmptyDirects.Count != 0)
            {
                for (int i = 0; i < farthestroom.EmptyDirects.Count; i++)
                {
                    //방 하나만 연결될 수 있을 때(막다른 방이 될 수 있을 때)
                    if (Searchroom(farthestroom.X, farthestroom.Y, farthestroom.EmptyDirects[i]))
                    {
                        count++;
                        Room specialroom = new Room(farthestroom.X, farthestroom.Y, count);
                        RoomDirect selected = farthestroom.EmptyDirects[i];
                        farthestroom.InterconnectRoom(specialroom, selected);
                        specialroom.UpdateCoorinate(selected);
                        Vector3 roomPos = new Vector3(specialroom.X, specialroom.Y, 0f) * roomWidth;
                        if(count == bossIndex)
                            specialroom.ConnectClearRoom(selected);

                        GameObject newRoomObj = Instantiate(emptyRoomPref, roomPos, Quaternion.identity);
                        dungeonRooms.Add(newRoomObj.GetComponent<DungeonRoom>());
                        newRoomObj.transform.parent = roomParent.transform;
                        newRoomObj.name = count.ToString();
                        specialroom.SetRoomObject(newRoomObj);
                        rooms.Add(specialroom);
                        roomlocal[maxRoomCount - 1 + specialroom.X, maxRoomCount - 1 + specialroom.Y] = count + 1;
                        distance[distance.ToList().IndexOf(distance.Max())] = 0;
                        break;
                    }
                }
            }
            distance[distance.ToList().IndexOf(distance.Max())] = 0;
        }
    }

    private void ConnectNearRoom(Room newRoom)
    {
        // newRoom은 좌표 설정 및 이전 room과 연결되어 있어야 함
        List<RoomDirect> directs = newRoom.EmptyDirects.ConvertAll(d => d);
        int checkX, checkY;

        foreach (RoomDirect direct in directs)
        {
            checkX = newRoom.X + maxRoomCount - 1;
            checkY = newRoom.Y + maxRoomCount - 1;
            // newRoom은 emptyDirects.Count는 무조건 3
            switch (direct)
            {
                case RoomDirect.Top:
                    checkY += 1;
                    break;
                case RoomDirect.Right:
                    checkX += 1;
                    break;
                case RoomDirect.Down:
                    checkY -= 1;
                    break;
                case RoomDirect.Left:
                    checkX -= 1;
                    break;
            }

            if (roomlocal[checkX, checkY] != 0)
                newRoom.InterconnectRoom(rooms[roomlocal[checkX, checkY] - 1], direct);
        }
    }

    // -------------------------------------------------------------
    // 방 생성 (room, door, object)
    // -------------------------------------------------------------
    private void DrawRoom(DungeonRoom room, TileType type, int num)
    {
        RoomSize size = RoomSize.Medium;
        bool Clear = false;
        if (num == 0 || num == shopIndex)
            Clear = true;

        else if (num != bossIndex && !Clear)
            size = RoomSize.Small;
        
        // Default Medium size
        int rows = 7;
        int cols = 7;

        if (size == RoomSize.Small)
        {
            if (UnityEngine.Random.value < 0.5)
            {
                rows = 7;
                cols = 5;
            }
            else
            {
                rows = 5;
                cols = 7;
            }
        }

        ushort indexTileType = (ushort)type;
        // 타입은 Ground 타입을 지정하면 됨
        // row x col 크기의 방 생성
        // 모서리 생성
        Vector3Int currentTile = new Vector3Int(0, cols - 1, 0);
        DrawTile(room.WallLayer, (TileType)(indexTileType + 2), TileDirect.Default, currentTile);
        currentTile = new Vector3Int(rows - 1, cols - 1, 0);
        DrawTile(room.WallLayer, (TileType)(indexTileType + 2), TileDirect.Right, currentTile);
        currentTile = new Vector3Int(rows - 1, 0, 0);
        DrawTile(room.WallLayer, (TileType)(indexTileType + 2), TileDirect.Bottom, currentTile);
        currentTile = new Vector3Int(0, 0, 0);
        DrawTile(room.WallLayer, (TileType)(indexTileType + 2), TileDirect.Left, currentTile);

        // 벽 생성
        for (int row = 1; row < rows - 1; row++)
        {
            currentTile = new Vector3Int(row, cols - 1, 0);
            DrawTile(room.WallLayer, (TileType)(indexTileType + 1), TileDirect.Default, currentTile);
            currentTile = new Vector3Int(row, 0, 0);
            DrawTile(room.WallLayer, (TileType)(indexTileType + 1), TileDirect.Bottom, currentTile);
        }

        for (int col = 1; col < cols - 1; col++)
        {
            currentTile = new Vector3Int(0, col, 0);
            DrawTile(room.WallLayer, (TileType)(indexTileType + 1), TileDirect.Left, currentTile);
            currentTile = new Vector3Int(rows - 1, col, 0);
            DrawTile(room.WallLayer, (TileType)(indexTileType + 1), TileDirect.Right, currentTile);
        }

        // 바닥 생성
        for (int col = 1; col < cols - 1; col++)
        {
            for (int row = 1; row < rows - 1; row++)
            {
                currentTile = new Vector3Int(row, col, 0);
                DrawTile(room.GroundLayer, (TileType)indexTileType, TileDirect.Default, currentTile);
            }
        }

        //보스방 표식
        if (num == bossIndex)
        {
            currentTile = new Vector3Int((int)(rows * 0.5f), (int)(cols * 0.5f), 0);
            DrawTile(room.GroundLayer, TileType.BossGround, TileDirect.Default, currentTile);
        }

        // 방 크기에 맞추어 정 가운데로 정렬
        Vector3 tileMapSize = new Vector3(room.WallLayer.size.x, room.WallLayer.size.y, 0f);
        room.TileMapParent.transform.localPosition -= tileMapSize * room.WallLayer.cellSize.x * 0.5f;
        room.IsClear = Clear;
    }

    //보스로 향하는 포탈을 알려줌
    private void NearBossRoomDraw()
    {
        int num = 0;
        int i = 0;
        int x = 0, y = 0;
        bool Isx = false;
        Vector3Int currentpos1, currentpos2, roomsize;
        //보스 방 근처를 탐색하여 연결된 방을 찾아냄
        for (i = 0; i < 4; i++)
        {
            x = maxRoomCount - 1 + rooms[bossIndex].X + dx[i];
            y = maxRoomCount - 1 + rooms[bossIndex].Y + dy[i];
            if (roomlocal[x, y] != 0)
            {
                num = roomlocal[x, y] - 1;
                break;
            }
        }

        roomsize = dungeonRooms[num].WallLayer.size;
        GameObject firetorchObject1 = Instantiate(Firetorch);
        GameObject firetorchObject2 = Instantiate(Firetorch);
        firetorchObject1.transform.parent = dungeonRooms[num].transform;
        firetorchObject2.transform.parent = dungeonRooms[num].transform;

        //보스방이 왼쪽일 경우
        if (dx[i] > 0)
        {
            currentpos1 = new Vector3Int(0, (int)(roomsize.y * 0.5f) - 1);
            currentpos2 = new Vector3Int(0, (int)(roomsize.y * 0.5f) + 1);
            firetorchObject1.transform.rotation = Quaternion.Euler(0, 0, 270);
            firetorchObject2.transform.rotation = Quaternion.Euler(0, 0, 270);
        }
        //오른쪽일 경우
        else if (dx[i] < 0)
        {
            currentpos1 = new Vector3Int(roomsize.x - 1, (int)(roomsize.y * 0.5f) - 1);
            currentpos2 = new Vector3Int(roomsize.x - 1, (int)(roomsize.y * 0.5f) + 1);
            firetorchObject1.transform.rotation = Quaternion.Euler(0, 0, 90);
            firetorchObject2.transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        //아래일 경우
        else if (dy[i] > 0)
        {
            currentpos1 = new Vector3Int((int)(roomsize.x * 0.5f) - 1, 0);
            currentpos2 = new Vector3Int((int)(roomsize.x * 0.5f) + 1, 0);
            Isx = true;
        }
        //위일 경우
        else
        {
            currentpos1 = new Vector3Int((int)(roomsize.x * 0.5f) - 1, roomsize.y - 1);
            currentpos2 = new Vector3Int((int)(roomsize.x * 0.5f) + 1, roomsize.y - 1);
            firetorchObject1.transform.rotation = Quaternion.Euler(0, 0, 180);
            firetorchObject2.transform.rotation = Quaternion.Euler(0, 0, 180);
            Isx = true;
        }

        firetorchObject1.transform.localPosition = Isx ? new Vector3(
        roomsize.x * -0.5f + currentpos1.x + 0.5f + 0.5f,
        roomsize.y * -0.5f + currentpos1.y + 0.5f,
        0f
        ) : 
        new Vector3(
        roomsize.x * -0.5f + currentpos1.x + 0.5f,
        roomsize.y * -0.5f + currentpos1.y + 0.5f + 0.5f,
        0f
        );

        firetorchObject2.transform.localPosition = Isx ? new Vector3(
        roomsize.x * -0.5f + currentpos2.x + 0.5f - 0.5f,
        roomsize.y * -0.5f + currentpos2.y + 0.5f,
        0f
        ) :
        new Vector3(
        roomsize.x * -0.5f + currentpos2.x + 0.5f,
        roomsize.y * -0.5f + currentpos2.y + 0.5f - 0.5f,
        0f
        );
    }

    private void GenerateDoors(TileType type)
    {
        for (int roomIndex = 0; roomIndex < rooms.Count; roomIndex++)
        {
            foreach (RoomDirect direct in rooms[roomIndex].ExistDirects)
            {
                // 벽이 인접한 방향
                Vector3Int roomSize = dungeonRooms[roomIndex].WallLayer.size;
                Vector3Int centerDoorPos;

                switch ((ushort)direct)
                {
                    case 0:
                        centerDoorPos = new Vector3Int((int)(roomSize.x * 0.5f), roomSize.y - 1);
                        break;
                    case 2:
                        centerDoorPos = new Vector3Int((int)(roomSize.x * 0.5f), 0);
                        break;
                    case 1:
                        centerDoorPos = new Vector3Int(roomSize.x - 1, (int)(roomSize.y * 0.5f));
                        break;
                    case 3:
                        centerDoorPos = new Vector3Int(0, (int)(roomSize.y * 0.5f));
                        break;
                    default:
                        centerDoorPos = Vector3Int.zero;
                        break;
                }

                TileType closeDoor = TileType.DefaultCloseDoor;
                TileType openDoor = TileType.DefaultOpenDoor;

                switch (type)
                {
                    case TileType.MossGround:
                        closeDoor = TileType.MossCloseDoor;
                        openDoor = TileType.MossOpenDoor;
                        break;
                    case TileType.VineGround:
                        closeDoor = TileType.VineCloseDoor;
                        openDoor = TileType.VineOpenDoor;
                        break;
                    case TileType.VineMossGround:
                        closeDoor = TileType.VineMossCloseDoor;
                        openDoor = TileType.VineMossOpenDoor;
                        break;
                }

                // 포탈 생성 및 연결
                DrawTile(dungeonRooms[roomIndex].CloseDoorLayer, closeDoor, (TileDirect)(ushort)direct, centerDoorPos);
                DrawTile(dungeonRooms[roomIndex].OpenDoorLayer, openDoor, (TileDirect)(ushort)direct, centerDoorPos);

                GameObject portalObject = Instantiate(portalPref);
                portalObject.transform.parent = dungeonRooms[roomIndex].transform;
                // DrawTile로 그려진 위치로 portalPref 위치 조정 (room start pos + door pos + offset)
                portalObject.transform.localPosition = new Vector3(
                    roomSize.x * -0.5f + centerDoorPos.x + 0.5f,
                    roomSize.y * -0.5f + centerDoorPos.y + 0.5f, 
                    0f
                );
                Portal portal = portalObject.GetComponent<Portal>();
                int connectedroomid = rooms[roomIndex].GetConnectedRoomId((ushort)direct);
                if(connectedroomid != -1)
                    portal.Connect(connectedroomid, (ushort)direct);
                //보스 방 클리어 이후 나갈 문 그려주기
                else
                    portal.Connect(connectedroomid, (ushort)direct, true);
                dungeonRooms[roomIndex].Portals[(ushort)direct] = portal;
            }
        }
    }

    private void GenerateObject()
    {
        int boxCount = 0, i;
        for(i = 0; i < dungeonRooms.Count; i++)
        {
            int x = (int)dungeonRooms[i].GroundLayer.size.x - 1;
            int y = (int)dungeonRooms[i].GroundLayer.size.y - 1;

            //장애물 생성하면 안 되는 곳 check
            int[] check = new int[(x * y)];

            //문 앞에는 장애물 생성 X
            foreach (RoomDirect rd in rooms[i].ExistDirects) 
            {
                switch (rd)
                {
                    case RoomDirect.Top:
                        check[(int)((x - 1) * 0.5f)] = 1;
                        break;
                    case RoomDirect.Right:
                        check[x * (int)((y + 1) * 0.5f) - 1] = 1;
                        break;
                    case RoomDirect.Down:
                        check[x * (y - 1) + (int)((x - 1) * 0.5f)] = 1;
                        break;
                    case RoomDirect.Left:
                        check[x * (int)((y - 1) * 0.5f)] = 1;
                        break;
                }
            }

            for (int j = 0; j < 4; j++)
            {
                if (UnityEngine.Random.value > 0.4)
                {
                    continue;
                }

                if (x > 4 && y > 4)
                    boxCount = UnityEngine.Random.Range(1, 4);

                else
                    boxCount = 1;

                GameObject boxes = Instantiate(boxesPref);
                boxes.transform.SetParent(dungeonRooms[i].ObjectParent.transform);
                boxes.GetComponent<Boxes>().Set(boxCount, (RoomDirect)j);

                float z = boxes.transform.position.z;
                Vector3 offset;

                //상자가 놓인 부분에는 장애물 생성 X
                switch ((RoomDirect)j)
                {
                    case RoomDirect.Top:
                        offset = new Vector3(0.5f, -0.5f, 0f);
                        boxes.transform.localPosition = new Vector3(x * -0.5f, y * 0.5f, z) + offset;
                        check[0] = 1;
                        break;
                    case RoomDirect.Right:
                        offset = new Vector3(-0.35f, -0.5f, 0f);
                        boxes.transform.localPosition = new Vector3(x * 0.5f, y * 0.5f, z) + offset;
                        boxes.transform.rotation = Quaternion.Euler(0, 0, 270);
                        check[x - 1] = 1;
                        break;
                    case RoomDirect.Down:
                        offset = new Vector3(-0.35f, 0.5f, 0f);
                        boxes.transform.localPosition = new Vector3(x * 0.5f, y * -0.5f, z) + offset;
                        boxes.transform.rotation = Quaternion.Euler(0, 0, 180);
                        check[x * y - 1] = 1;
                        break;
                    case RoomDirect.Left:
                        offset = new Vector3(0.5f, 0.5f, 0f);
                        boxes.transform.localPosition = new Vector3(x * -0.5f, y * -0.5f, z) + offset;
                        boxes.transform.rotation = Quaternion.Euler(0, 0, 90);
                        check[x * (y - 1)] = 1;
                        break;
                }
            }
            //장애물 생성
            GenerateObstacle(check, i);
        }
    }

    //맵에 장애물 생성 현재는 Hole만 생성
    private void GenerateObstacle(int[] check, int num)
    {
        float i, j;
        float xmax, ymax;
        int cnt;

        //70%확률로 방에 Hole 생성, 상점방과 처음방, 보스방에는 Hole 없게
        if (dungeonRooms[num].IsClear || UnityEngine.Random.value > 0.7 || bossIndex == num)
            return;

        float x = dungeonRooms[num].GroundLayer.size.x - 2;
        float y = dungeonRooms[num].GroundLayer.size.y - 2;
        xmax = x * 0.5f;
        ymax = y * 0.5f;
        cnt = -1;

        for (i = ymax; i >= -1 * ymax; i--)
        {
            for (j = -1 * xmax; j <= xmax; j++)
            {
                cnt++;
                //check를 확인해서 이미 door or box가 있으면 그 구역에는 hole 설치 x
                if (check[cnt] == 1)
                    continue;
                SetObstacle(j, i, dungeonRooms[num].ObstacleParent.transform);
            }
        }
    }

    //한 구역에 장애물 생성
    private void SetObstacle(float x, float y, Transform t)
    {
        int i, j, cnt = 0;
        float con = 1f / 3f;
        //문제점: 좌상단부터 구멍을 생성하므로 구역의 구멍 모양이 균일할 수 있음
        for(i = -1; i < 2; i++)
        {
            for(j = -1; j < 2; j++)
            {
                //20%확률로 hole 생성, 만약 구역에 구멍이 2개 이상이면 그만 생성
                if (UnityEngine.Random.value > 0.2)
                    continue;
                if (cnt > 1)
                    break;
                GameObject Obstacle = Instantiate(HolePref);
                Obstacle.transform.SetParent(t);
                Obstacle.transform.localPosition = new Vector3(x + j * con, y + i * con, 0);
                cnt++;
            }

            if (cnt > 1)
                break;
        }
    }

    // -------------------------------------------------------------
    // 타일 그리기
    // -------------------------------------------------------------
    private void DrawTile(Tilemap tileMap, TileType type, TileDirect direct, Vector3Int pos)
    {
        Tile tile = null;
        switch ((ushort)type)
        {
            case 0:
            case 12:
                // DefaultGrond or VineGround
                tile = RoomGenerator.SelectRandomTile(defaultGrounds);
                break;
            case 1:
                // DefaultWall
                tile = RoomGenerator.SelectRandomTile(defaultWalls);
                break;
            case 2:
                // DefaultEdge
                tile = RoomGenerator.SelectRandomTile(defaultEdges);
                break;
                // WaterWall,
                // WaterEdge,
            case 6:
            case 15:
                // MossGround or VindMossGround
                tile = RoomGenerator.SelectRandomTile(mossGrounds);
                break;
            case 7:
                // MossWall
                tile = RoomGenerator.SelectRandomTile(mossWalls);
                break;
            case 8:
                // MossEdge
                tile = RoomGenerator.SelectRandomTile(mossEdges);
                break;
            case 13:
                // VineWall
                tile = RoomGenerator.SelectRandomTile(vineWalls);
                break;
            case 14:
                // VineEdge
                tile = RoomGenerator.SelectRandomTile(vineEdges);
                break;
            case 16:
                // VineMossWall
                tile = RoomGenerator.SelectRandomTile(vineMossWalls);
                break;
            case 17:
                // VineMossEdge
                tile = RoomGenerator.SelectRandomTile(vineMossEdges);
                break;
                // MossWaterWall,
                // MossWaterEdge,
                // VineMossWaterWall,
                // VineMossWaterEdge,
            case 21:
            case 22:
            case 23:
            case 24:
            case 25:
            case 26:
            case 27:
            case 28:
                // All doors
                tile = allDoors[(ushort)type % 21];
                break;
                //BossTile
            case 29:
                tile = RoomGenerator.SelectRandomTile(BossGrounds);
                break;
        }

        if (tile != null)
        {
            tileMap.SetTile(pos, tile);
            
            if (direct != TileDirect.Default)
            {
                // 정방향 타일이 아닌 경우 타일 회전
                Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, (ushort)direct * -90f), Vector3.one);
                tileMap.SetTransformMatrix(pos, matrix);
            }
        }
        else
        {
            Debug.LogError("Invalid tile");
        }
    }

    private static Tile SelectRandomTile(List<Tile> tiles)
    {
        return tiles[UnityEngine.Random.Range(0, tiles.Count)];
    }
}