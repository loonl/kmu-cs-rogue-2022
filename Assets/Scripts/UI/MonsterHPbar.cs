using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MonsterHPbar : MonoBehaviour
{
    // Start is called before the first frame update
    public Slider hpBar;
    public Monster monster;
    public MonsterStat monsterStat;

    // Update is called once per frame
    void Update()
    {
        UpdateHPbar();
    }
    
    public void CreateHPbar(MonsterStat monsterStat, Monster monster)
    {
        this.monsterStat = monsterStat;
        this.monster = monster;
        //this.hpBar = hpBar;
    }
    
    void UpdateHPbar()
    {
        //hpBar.value = monsterStat.health / monsterStat.maxHealth;
        //위치 갱신
        this.transform.position = monster.transform.position;
        
    }
}
