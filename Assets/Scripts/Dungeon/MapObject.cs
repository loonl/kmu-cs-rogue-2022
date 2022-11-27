using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObject : MonoBehaviour
{
    public void OnDamage()
    {
        // !!! SOUND 상자 부숴지는 소리
        SoundManager.Instance.SoundPlay(SoundType.Box);

        GameObject droppedItem;

        if (Random.value < 0.05)  // 아이템 확률
        {
            droppedItem = GameManager.Instance.CreateGO
            (
                "Prefabs/Dungeon/Dropped",
                DungeonSystem.Instance.DroppedItems.transform
            );

            // 드랍 확률에 따라 아이템 할당
            Item item = GameManager.Instance.GetRandomDropItem();
            droppedItem.GetComponent<DroppedItem>().Set(item);
            droppedItem.transform.position = this.transform.position;
        }

        else if(Random.value < 0.4)
        {
            droppedItem = GameManager.Instance.CreateGO
            (
                "Prefabs/Dungeon/Portion",
                DungeonSystem.Instance.DroppedItems.transform
            );
            droppedItem.transform.position = this.transform.position;
        }

        Destroy(this.gameObject);
    }
}
