using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObject : MonoBehaviour
{
    public void OnDamage()
    {
        // !!! SOUND 상자 부숴지는 소리

        GameObject droppedItem;

        float itemPercent = 0.5f;
        if (Random.Range(0f, 1f) <= itemPercent)  // 아이템 확률
        {
            droppedItem = GameManager.Instance.CreateGO
            (
                "Prefabs/Dungeon/Dropped",
                DungeonSystem.Instance.DroppedItems
            );

            // 드랍 확률에 따라 아이템 할당
            Item item = GameManager.Instance.GetRandomDropItem();
            droppedItem.GetComponent<DroppedItem>().Set(item);
        }
        else
        {
            droppedItem = GameManager.Instance.CreateGO
            (
                "Prefabs/Dungeon/Portion",
                DungeonSystem.Instance.DroppedItems
            );
        }

        droppedItem.transform.position = this.transform.position;

        Destroy(this.gameObject);
    }
}
