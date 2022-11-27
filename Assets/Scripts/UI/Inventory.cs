using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{
    private int gold = 0;
    public int Gold { get { return gold; } }
    // -------------------------------------------------------------
    // 골드
    // -------------------------------------------------------------

    public void UpdateGold(int diff)
    {
        gold += diff;
        // UIManager.Instance.UpdateGoldText(gold);
    }
}
