using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portion : Interact
{
    public override void InteractEvent()
    {
        GameManager.Instance.Player.stat.Recover(25);
        Destroy(this.gameObject);

        // !!! 포션 사운드 추가
    }
}