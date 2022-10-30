using System;
using System.Collections;
using UnityEngine;

public class Zombie : Monster
{
    protected override void Init()
    {
        base.Init();
        Monstertype = MonsterType.Zombie;
        Sound = SoundManager.Instance.ZombieClip(Monstertype);
    }

    private void Start()
    {
        Init();
    }
}
