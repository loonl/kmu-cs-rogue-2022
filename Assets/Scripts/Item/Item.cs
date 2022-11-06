using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item
{

    public Sprite image;
    public int id { get; set; }
    public new string name { get; set; }
    public int itemType { get; set; }
    public Stat stat { get; set; }
    public string path { get; set; }

    public string effectName { get; set; }
    public string skillName { get; set; }
    public Item(int itemId, Dictionary<string, object> data)
    {
        id = itemId;
        name = (string)data["name"];
        stat = new Stat(float.Parse(data["hp"].ToString()), float.Parse(data["dmg"].ToString()), float.Parse(data["range"].ToString()),
                        float.Parse(data["skilldmg"].ToString()), float.Parse(data["cooltime"].ToString()), 
                        float.Parse(data["knockbackforce"].ToString()), float.Parse(data["speed"].ToString()));
        itemType = (int)data["type"];
        path = (string)data["path"];
        effectName = (string)data["effectName"] == "" ? "NormalSlash2" : (string)data["effectName"];
        skillName = (string)data["skillName"] == "" ? "ExplosiveAttack" : (string)data["skillName"];
        if (path != "")
            image = Resources.Load<Sprite>(path.Substring(17, path.Length - 21));
    }

    public bool isEmpty()
    {
        if (this.stat == new Stat(true) && path == "")
            return true;
        return false;
    }
}
