using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portion : Interact
{
    public override void InteractEvent()
    {
        GameManager.Instance.Player.stat.Recover(GameManager.Instance.Player.stat.maxHp * 0.1f);
        Destroy(this.gameObject);

        // !!! 포션 사운드 추가
        SoundManager.Instance.SoundPlay(SoundType.Portion);
    }
}