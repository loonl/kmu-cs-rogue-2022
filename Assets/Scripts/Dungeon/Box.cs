using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boxes : MonoBehaviour
{
    public void Set(int number, RoomDirect direct)
    {
        switch (number)
        {
            case 1:
                CreateObject(direct).transform.localPosition = new Vector3(-.25f, .25f, -.1f);
                break;
            case 2:
                CreateObject(direct).transform.localPosition = new Vector3(-.25f, .25f, -.1f);
                if (Random.value < 0.5)
                {
                    CreateObject(direct).transform.localPosition = new Vector3(.25f, .25f, -.1f);
                }
                else
                {
                    CreateObject(direct).transform.localPosition = new Vector3(-.25f, -.25f, -.1f);
                }
                break;
            case 3:
                CreateObject(direct).transform.localPosition = new Vector3(-.25f, .25f, -.1f);
                CreateObject(direct).transform.localPosition = new Vector3(.25f, .25f, -.1f);
                CreateObject(direct).transform.localPosition = new Vector3(-.25f, -.25f, -.1f);
                break;
        }

    }

    private GameObject CreateObject(RoomDirect direct)
    {
        GameObject box = Random.value < 0.5
            ? GameManager.Instance.CreateGO("Prefabs/Dungeon/barrel", this.transform)
            : GameManager.Instance.CreateGO("Prefabs/Dungeon/box", this.transform);

        switch (direct)
        {
            case RoomDirect.Right:
                box.transform.rotation = Quaternion.Euler(0, 0, 90);
                break;
            case RoomDirect.Down:
                box.transform.rotation = Quaternion.Euler(0, 0, 180);
                break;
            case RoomDirect.Left:
                box.transform.rotation = Quaternion.Euler(0, 0, 270);
                break;
        }

        return box;
    }
}
