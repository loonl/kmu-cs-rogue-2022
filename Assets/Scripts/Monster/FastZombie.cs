using System;
using System.Collections;
using UnityEngine;

public class FastZombie : Monster
{
    protected override void Init()
    {
        base.Init();
        Monstertype = MonsterType.FastZombie;
        Sound = SoundManager.Instance.ZombieClip(Monstertype);
    }

    private void Start()
    {
        Init();
    }
}
