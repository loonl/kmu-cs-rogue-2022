using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapManager : MonoBehaviour
{
    // Start is called before the first frame update
    static public MinimapManager instance;
    public int[,] minimap;
    private GameObject canvas;
    private GameObject minimapObjectPrefab;
    private GameObject clearMiniRoom;
    private GameObject bossMiniRoom;
    private GameObject shopMiniRoom;
    private GameObject miniRoom;
    private GameObject minimapObject;
    private GameObject[,] miniRooms;
    public int mapSize;
    public int posY;
    public int posX;
    void Start()
    {
        instance = this;
        canvas = Resources.Load<GameObject>("Prefabs/UI/DynamicCanvas");
        minimapObjectPrefab = Resources.Load<GameObject>("Prefabs/UI/Minimap");
        clearMiniRoom = Resources.Load<GameObject>("Prefabs/UI/MiniClearRoom");
        bossMiniRoom = Resources.Load<GameObject>("Prefabs/UI/MiniBossRoom");
        shopMiniRoom = Resources.Load<GameObject>("Prefabs/UI/MiniShopRoom");
        miniRoom = Resources.Load<GameObject>("Prefabs/UI/MiniRoom");
        miniRooms = new GameObject[5, 5];
    }
    
    public void SetMinimap(int[,] map, int size, int shopIdx,int bossIdx)
    {
        minimap = map;
        mapSize = size;
        posX = posY = size/2;
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (minimap[j, i]==shopIdx)
                    minimap[j, i] = 3;
                else if(minimap[j, i]==bossIdx)
                    minimap[j, i] = 4;
                else if (minimap[j, i] != 0)
                    minimap[j, i] = 1;
            }
        }
        minimap[posX, posY] = 2;

        if (minimapObject == null)
        {
            canvas = Instantiate(canvas);
            minimapObject = Instantiate(minimapObjectPrefab, canvas.transform);
        }
        
        DrawMinimap();
        
    }
    
    public void SetPosition(ushort dir)
    {
        if (dir == 0)
            posY++;
        if (dir == 1)
            posX++;
        if (dir == 2)
            posY--;
        if(dir == 3)
            posX--;
        minimap[posX, posY] = 2;
    }

    public void DrawMinimap()
    {
        for (int i = -2; i <= 2; i++)
        {
            for (int j = -2; j <= 2; j++)       
            {
                if(posX+i >= 0 && posX+i < mapSize && posY+j >= 0 && posY+j < mapSize)
                {
                    Destroy(miniRooms[i + 2, j + 2]);
                    if (minimap[posX + i, posY + j] != 0)
                    {
                        switch (minimap[posX + i, posY + j])
                        {
                            case 1:
                                miniRooms[i + 2, j + 2] = Instantiate(miniRoom, minimapObject.transform);
                                break;
                            case 2:
                                miniRooms[i + 2, j + 2] = Instantiate(clearMiniRoom, minimapObject.transform);
                                break;
                            case 3:
                                miniRooms[i + 2, j + 2] = Instantiate(shopMiniRoom, minimapObject.transform);
                                break;
                            case 4:
                                miniRooms[i + 2, j + 2] = Instantiate(bossMiniRoom, minimapObject.transform);
                                break;
                        }
                        miniRooms[i + 2, j + 2].transform.localPosition = new Vector3(i * 19, j * 19, 0);
                    }
                }
            }
        }
    }
}
