using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStat
{
    public List<Dictionary<string, object>> monsterData; // ���� ������

    public float scale { get; private set; } // ũ��
    public int gold { get; private set; } //��� ���
    public float maxHealth { get; private set; } // �ִ� ü��
    public float health { get; private set; } // ���� ü��
    public float damage { get; private set; } // ���ݷ�
    public float speed1 { get; private set; } // �ӵ�1
    public float speed2 { get; private set; } // �ӵ�2
    public float speed { get; private set; } // ���� �ӵ�

    // csv������ �̿��Ͽ� ���� �ʱ�ȭ
    public MonsterStat(List<Dictionary<string, object>> _monsterData, int id)
    {
        monsterData = _monsterData;
        scale = float.Parse(monsterData[id]["Scale"].ToString());
        gold = int.Parse(monsterData[id]["Gold"].ToString());
        maxHealth = float.Parse(monsterData[id]["MaxHealth"].ToString());
        health = maxHealth;
        damage = float.Parse(monsterData[id]["Damage"].ToString());
        speed1 = float.Parse(monsterData[id]["Speed1"].ToString());
        speed2 = float.Parse(monsterData[id]["Speed2"].ToString());
    }

    public void OnDamage(float amount)
    {
        health = Mathf.Clamp(health - amount, 0, maxHealth);
    }

    public void Revive()
    {
        health = maxHealth;
        gold = 0;
    }

    public void ChangeSpeed(int speedNum)
    {
        if (speedNum == 1)
        {
            speed = speed1;
        }
        else
        {
            speed = speed2;
        }
    }
}
