using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    private static List<Dictionary<string, object>> data;
    private static ItemManager _instance = null;
    public static ItemManager Instance
    { 
        get
        {
            if (_instance == null)
            {
                _instance = new ItemManager();
            }

            return _instance;
        }
    }

    public Item GetItem(int itemId)
    {
        if (data == null)
        {
            data = CSVReader.Read("Datas/Item");
        }

        return new Item(itemId, data[itemId]);
    }
}
