using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemTooltip : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemDescriptionText;
    void Start()
    {
        
    }
    
    public void SetTooltip(Item item)
    {
        itemNameText.text = item.name;
        itemDescriptionText.text = "HP : " + item.stat.hp + "\n\n" + "SPEED : " + item.stat.speed + "\n\n" +
                                   "ATK : " + item.stat.damage + "\n\n" + "SKILL DAMAGE : " + item.stat.skillDamage;
    }
    
    public void ShowTooltip()
    {
        gameObject.SetActive(true);
    }
    
    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
