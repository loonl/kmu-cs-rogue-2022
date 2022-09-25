using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedItem : Interact
{
    [SerializeField]
    private SpriteRenderer _renderer;

    private Item _item;
    public Item Item { get { return _item; } }

    public int _price = 0;
    // private Image _sprite;


    public void Set(Item item, int price = 0)
    {
        this._item = item;

        // 이미지 할당
        this._renderer.sprite = item.image;

        this._price = price;
    }

    public override void InteractEvent()
    {
        if (this._price == 0)
        {
            int itemIndex;
            int type = _item.itemType;
            if (type == 0 || type == 1 || type == 2)
                itemIndex = 0;
            else
                itemIndex = _item.itemType - 3;

            Item temp = GameManager.Instance.Player.equipment[itemIndex];

            // Get
            GameManager.Instance.Player.Equip(this._item);

            // 만약 기존 장착한 아이템이 있으면
            if (!temp.isEmpty())
                Set(temp);
            // 없으면
            else
                Destroy(this.gameObject);
        }
        else
        {
            // Buy
            if (GameManager.Instance.Player.Inventory.Gold < this._price)
            {
                return;
            }

            // price를 0으로 바꾸어 Get으로 바뀌게 만듬
            GameManager.Instance.Player.Inventory.UpdateGold(-1 * this._price);
            this._price = 0;

            // !!! Sound (구매음 추가하기)
        }
    }
}
