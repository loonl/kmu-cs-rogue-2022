using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    // player stat variables
    public Stat stat = new Stat(false);
    [HideInInspector]
    public bool isAttacking;
    [HideInInspector]
    public float remainCool;
    [HideInInspector]
    public bool dead;

    // equipments => 0 : Weapon, 1 : Helmet, 2 : Armor, 3 : Pants, 4 : Shield
    [HideInInspector]
    public List<Item> equipment;

    Animator anim;
    WeaponCollider wpnColl;
    Rigidbody2D rig;
    PlayerAttack playerAttack;
    [HideInInspector]
    public SPUM_SpriteList spumMgr;

    // 인벤토리
    public Inventory Inventory { get; private set; }

    // 상호작용
    public Interact _interact;

    private void Awake()
    {
        this.Inventory = GameObject.Find("Inventory").GetComponent<Inventory>();    
    }

    void Start()
    {
        playerAttack = GetComponent<PlayerAttack>();
        anim = transform.GetChild(0).gameObject.GetComponent<Animator>();
        wpnColl = transform.GetChild(0).gameObject.GetComponent<WeaponCollider>();
        spumMgr = transform.GetChild(0).GetChild(0).GetComponent<SPUM_SpriteList>();
        rig = GetComponent<Rigidbody2D>();

        // player's first equipments (플레이어 첫 장비)
        equipment = new List<Item> { ItemManager.Instance.GetItem(0), // weapon
                                     ItemManager.Instance.GetItem(7), // helmet
                                     ItemManager.Instance.GetItem(10), // armor
                                     ItemManager.Instance.GetItem(15), // pants
                                     ItemManager.Instance.GetItem(1)  // shield
                                    };

        List<Stat> temp = new List<Stat>();
        for (int i = 0; i < equipment.Count; i++)
            temp.Add(equipment[i].stat);
        stat.SyncStat(temp);

        // used in animator end event - death
        anim.GetComponent<PlayerAnimreciver>().onDieComplete = () =>
        {
            // hide character
            //gameObject.SetActive(false);

            // temporary code
            // disable physics
            GetComponent<CapsuleCollider2D>().enabled = false;
            rig.velocity = new Vector2(0, 0);
            rig.isKinematic = true;
        };

        // used in animator end event - attack
        anim.GetComponent<PlayerAnimreciver>().onAttackComplete = () =>
        {
            // enable re-attack
            isAttacking = false;

            // disable attack collider
            wpnColl.poly.enabled = false;

            // clear attack collider monster list
            if (wpnColl.monsters.Count > 0)
            {
                wpnColl.monsters.Clear();
            }
        };

        // used in animator end event - skill
        anim.GetComponent<PlayerAnimreciver>().onSkillComplete = () =>
        {
            // enable re-attack
            isAttacking = false;

            // reset elasped skill cool-time
            remainCool = equipment[0].stat.coolTime;
        };

        // player stat variables init
        dead = false;
        remainCool = -1f;

        // attack & skill range init
        wpnColl.SetAttackRange(equipment[0].stat.range);
    }

    void Update()
    {
        remainCool -= Time.deltaTime;
        if (remainCool < -100.0f)
            remainCool = -1.0f;

        // should not work in dead condition
        if (dead)
            return;

        // get move-related input
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0).normalized;

        // FLIP character depending on heading direction
        if (moveInput.x > 0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else if (moveInput.x < 0 && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }

        // change character's position
        rig.velocity = moveInput * stat.speed;

        // change animation depending on speed
        anim.SetFloat("Speed", moveInput.magnitude * stat.speed);


        /**
        * Input Handling
        */

        // Attack Input
        if (Input.GetButtonDown("Fire1") && !isAttacking)
        {
            // update weapon state
            anim.SetInteger("WpnState", equipment[0].itemType);

            // change animation to attack
            anim.SetTrigger("Attack");
            isAttacking = true;

            playerAttack.Attack(equipment[0].id);

            // enable weapon collider
            wpnColl.poly.enabled = true;
        }

        // test debugging skill cool-time
        if (Input.GetButtonDown("Fire2"))
        {
            print("remaining skill cool : " + remainCool);
        }

        // test skill input
        if (Input.GetButtonDown("Fire2") && remainCool <= 0.0f && !isAttacking)
        {
            // update weapon state
            anim.SetInteger("WpnState", equipment[0].itemType);

            // change animation to skill
            anim.SetTrigger("Skill");

            // do not let attack and use skill at the same time
            isAttacking = true;

            // 스킬 관련 구현
            playerAttack.SkillAttack(equipment[0].id);
        }

        // test code - change equipments
        if (Input.GetKeyDown(KeyCode.G)) // helmet
            Equip(ItemManager.Instance.GetItem(82));

        if (Input.GetKeyDown(KeyCode.H)) // armor
            Equip(ItemManager.Instance.GetItem(86));

        if (Input.GetKeyDown(KeyCode.J)) // pants
            Equip(ItemManager.Instance.GetItem(87));

        if (Input.GetKeyDown(KeyCode.K)) // shield
            Equip(ItemManager.Instance.GetItem(89));

        if (Input.GetKeyDown(KeyCode.B)) // sword
            Equip(ItemManager.Instance.GetItem(77));

        if (Input.GetKeyDown(KeyCode.N)) // bow
            Equip(ItemManager.Instance.GetItem(81));

        if (Input.GetKeyDown(KeyCode.M)) // staff
            Equip(ItemManager.Instance.GetItem(61));

        if (Input.GetKeyDown(KeyCode.P))
        {
            print("MaxHP : " + stat.maxHp + "\nHP : " + stat.hp + "\nAttackPower : " + stat.damage
               + "\nAttackRange : " + stat.range + "\nSkillPower : " + stat.skillDamage
               + "\nSpeed : " + stat.speed + "\nCoolTime : " + stat.coolTime);
        }

        // 상호작용
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (this._interact == null)
            {
                return;
            }

            this._interact.InteractEvent();
        }
    }

    // -------------------------------------------------------------
    // Player 아이템 착용 / 해제
    // -------------------------------------------------------------
    public void Equip(Item item)
    {

        // 바뀌는 장비가 어느 부위인지 판단
        int partsIndex;
        int type = item.itemType;
        if (type == 0 || type == 1 || type == 2)
            partsIndex = 0;
        else
            partsIndex = item.itemType - 3;

        // 입고 있는 것 먼저 un-equip
        if (!item.isEmpty())
            UnEquip(equipment[partsIndex]);

        // 플레이어 스탯 수정
        List<Stat> itemStat = new List<Stat> { item.stat };
        stat.SyncStat(itemStat);


        // TO-DO 상의 필요 (근접 무기 -> 활 / 스태프로 무기 변경 시 방패 자동 장착 해제)
        if (partsIndex == 0)
        {
            if (equipment[0].itemType == 1 && (item.itemType == 2 || item.itemType == 3))
            {
                UnEquip(equipment[4]);
                equipment[4] = ItemManager.Instance.GetItem(94);
            }
        }

        // 활 / 스태프 상태에서 방패 장착 시 활 / 스태프 자동 장착 해제
        else if (partsIndex == 4)
        {
            if (equipment[0].itemType == 2 || equipment[0].itemType == 3)
            {
                UnEquip(equipment[0]);
                equipment[0] = ItemManager.Instance.GetItem(1);
            }
        }

        // 플레이어 외형 수정
        switch (type)
        {
            case 1: // sword
                spumMgr._weaponListString[0] = item.path;
                spumMgr.SyncPath(spumMgr._weaponList, spumMgr._weaponListString);
                break;
            case 2: // bow
            case 3: // staff
                spumMgr._weaponListString[2] = item.path;
                spumMgr.SyncPath(spumMgr._weaponList, spumMgr._weaponListString);
                break;
            case 4: // helmet
                spumMgr._hairListString[1] = item.path;
                spumMgr.SyncPath(spumMgr._hairList, spumMgr._hairListString);
                break;
            case 5: // armor
                spumMgr._armorListString[0] = spumMgr._armorListString[1] =
                    spumMgr._armorListString[2] = item.path;
                spumMgr.SyncPath(spumMgr._armorList, spumMgr._armorListString);
                break;
            case 6: // pants
                spumMgr._pantListString[0] = spumMgr._pantListString[1] = item.path;
                spumMgr.SyncPath(spumMgr._pantList, spumMgr._pantListString);
                break;
            case 7: // shield
                spumMgr._weaponListString[3] = item.path;
                spumMgr.SyncPath(spumMgr._weaponList, spumMgr._weaponListString);
                break;
        }

        // 장착 슬롯에 아이템 추가
        equipment[partsIndex] = item;
    }

    public void UnEquip(Item item)
    {
        var spum = spumMgr;
        int type = item.itemType;

        // 플레이어 스탯 수정
        List<Stat> itemStat = new List<Stat> { item.stat };
        stat.SubStat(itemStat);

        // Shield나 무기라면 플레이어 외형 수정
        if (type == 7)
        {
            spum._weaponListString[3] = "";
            spum.SyncPath(spumMgr._weaponList, spumMgr._weaponListString);
        }
        else if (type == 1)
        {
            spum._weaponListString[0] = "";
            spum.SyncPath(spumMgr._weaponList, spumMgr._weaponListString);
        }
        else if (type == 2 || type == 3)
        {
            spum._weaponListString[2] = "";
            spum.SyncPath(spumMgr._weaponList, spumMgr._weaponListString);
        }
    }

    public void OnDamage(float damage)
    {
        stat.Damaged(damage);

        // trigger die if health is below 0
        if (stat.hp == 0)
            Die();

        // change animation to stunned
        anim.SetTrigger("Hit");

        // debug
        print("Player's health : " + stat.hp);
    }

    public void Die()
    {
        // change animation to death
        anim.SetTrigger("Die");

        // TO-DO DEAD 로 TRUE 만들기
        dead = true;
    }

    // -------------------------------------------------------------
    // 상호 작용
    // -------------------------------------------------------------
    public void AddInteractEvent(Interact interact)
    {
        if (this._interact != null)
        {
            return;
        }

        this._interact = interact;
    }

    public void RemoveInteractEvent()
    {
        this._interact = null;
    }
}