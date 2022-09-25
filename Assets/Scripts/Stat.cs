using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat
{
    public float hp { get; set; }
    public float maxHp { get; set; }

    public float damage { get; set; }
    public float range { get; set; }
    public float skillDamage { get; set; }
    public float coolTime { get; set; }

    public float speed { get; set; }
    public Stat(bool empty)
    {
        if (!empty)
        {
            maxHp = 100f;
            hp = 100f;
            damage = 0f;
            range = 0;
            skillDamage = 0f;
            coolTime = 0f;
            speed = 2f;
        }
        else
        {
            maxHp = 0f;
            hp = 0f;
            damage = 0f;
            range = 0f;
            skillDamage = 0f;
            coolTime = 0f;
            speed = 0f;
        }
    }

    public Stat(float _hp, float _damage, float _range, float _skillDamage, float _coolTime, float _speed)
    {
        maxHp = _hp;
        hp = _hp;
        damage = _damage;
        range = _range;
        skillDamage = _skillDamage;
        coolTime = _coolTime;
        speed = _speed;
    }

    public void SyncStat(List<Stat> stats)
    {
        foreach (var stat in stats)
        {
            maxHp += stat.maxHp;
            hp += stat.hp;
            damage += stat.damage;
            range += stat.range;
            skillDamage += stat.skillDamage;
            coolTime += stat.coolTime;
            speed += stat.speed;
        }
    }

    public void SubStat(List<Stat> stats)
    {
        foreach (var stat in stats) 
        {
            maxHp -= stat.maxHp;
            hp -= stat.hp;
            damage -= stat.damage;
            range -= stat.range;
            skillDamage -= stat.skillDamage;
            coolTime -= stat.coolTime;
            speed -= stat.speed;
        }
    }

    public void SyncHP(float _hp, float _maxHp)
    {
        hp = _hp;
        maxHp = _maxHp;
    }

    public void Damaged(float amount)
    {
        hp = Mathf.Clamp(hp - amount, 0, maxHp);
    }

    public void Recover(float amount)
    {
        hp = Mathf.Clamp(hp + amount, 0, maxHp);
    }

    public static bool operator ==(Stat a, Stat b)
    {
        if (a.hp == b.hp && a.maxHp == b.maxHp && a.damage == b.damage &&
            a.range == b.range && a.skillDamage == b.skillDamage && a.coolTime == b.coolTime &&
            a.speed == b.speed)
            return true;
        return false;
    }

    public static bool operator !=(Stat a, Stat b)
    {
        if (a.hp == b.hp && a.maxHp == b.maxHp && a.damage == b.damage &&
    a.range == b.range && a.skillDamage == b.skillDamage && a.coolTime == b.coolTime &&
    a.speed == b.speed)
            return false;
        return true;
    }
}
