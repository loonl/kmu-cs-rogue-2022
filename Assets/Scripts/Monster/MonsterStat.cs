using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class MonsterStat
{
    public List<Dictionary<string, object>> monsterData; // 몬스터 데이터

    public float scale { get; private set; } // 크기
    public int gold { get; private set; } //드랍 골드
    public float maxHealth { get; private set; } // 최대 체력
    public float health { get; private set; } // 현재 체력
    public float damage { get; private set; } // 공격력
    public float speed1 { get; private set; } // 속도1
    public float speed2 { get; private set; } // 속도2
    public float speed3 { get; private set; } // 속도3
    public float speed { get; set; } // 현재 속도
    public float sight { get; private set; } // 시야

    // csv파일을 이용하여 몬스터 초기화
    public MonsterStat(List<Dictionary<string, object>> _monsterData, int id)
    {
        monsterData = _monsterData;
        scale = float.Parse(monsterData[id]["Scale"].ToString());
        gold = int.Parse(monsterData[id]["Gold"].ToString());
        maxHealth = float.Parse(monsterData[id]["MaxHealth"].ToString());
        damage = float.Parse(monsterData[id]["Damage"].ToString());
        speed1 = float.Parse(monsterData[id]["Speed1"].ToString());
        speed2 = float.Parse(monsterData[id]["Speed2"].ToString());
        speed3 = float.Parse(monsterData[id]["Speed3"].ToString());
        sight = float.Parse(monsterData[id]["Sight"].ToString());
        
        health = maxHealth;
        speed = speed1;
    }

    public void OnDamage(float amount)
    {
        health = Mathf.Clamp(health - amount, 0, maxHealth);
    }

    public void healthToMax()
    {
        health = maxHealth;
    }

    public void ChangeSpeed(int speedNum)
    {
        if (speedNum == 1)
            speed = speed1;
        else if (speedNum == 2)
            speed = speed2;
        else if (speedNum == 3)
            speed = speed3;
    }
}
