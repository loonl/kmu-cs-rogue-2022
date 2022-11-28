using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerAttack : MonoBehaviour
{
    Player player;

    // !! meleeWeaponID, meleeWeaponIdx - 편한 테스트를 위한 코드 !!
    List<int> meleeWeaponID = new List<int>() { 1, 2, 3, 4, 21, 22, 23, 24, 25, 26, 27, 28, 29, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 76, 77, 78, 79 };
    int meleeWeaponIdx = 0;
    private void Start()
    {
        player = GetComponent<Player>();
    }
    
    // !! 편한 테스트를 위한 코드 !!
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftBracket)) //'[' 키
        {
            if (meleeWeaponIdx == 0)
                meleeWeaponIdx = meleeWeaponID.Count - 1;
            else meleeWeaponIdx -= 1;
            Item it = ItemManager.Instance.GetItem(meleeWeaponID[meleeWeaponIdx]);
            Debug.Log($"{it.name}, id {it.id}, skillname {it.skillName}");
            player.Equip(it);
        }
        if (Input.GetKeyDown(KeyCode.RightBracket)) //']' 키
        {
            if (meleeWeaponIdx == meleeWeaponID.Count - 1)
                meleeWeaponIdx = 0;
            else meleeWeaponIdx += 1;
            Item it = ItemManager.Instance.GetItem(meleeWeaponID[meleeWeaponIdx]);
            Debug.Log($"{it.name}, id {it.id}, skillname {it.skillName}");
            player.Equip(it);
        }
    }
}
