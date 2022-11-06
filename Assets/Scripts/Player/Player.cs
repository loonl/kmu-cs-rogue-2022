using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using static UnityEditor.Progress;

public enum PlayerState {
    Normal = 0,
    Attacking = 1,
    Dashing = 2,
    Stunned = 3,
    Invincible = 4,
    Dead = 5
}

public class Player : MonoBehaviour {
    // player stat variables
    public Stat stat = new Stat(false);
    [HideInInspector]
    public float remainCool;

    [HideInInspector] public float dashCool;

    // 현재 Player의 상태 enum
    [HideInInspector]
    public PlayerState curState;

    // equipments => 0 : Weapon, 1 : Helmet, 2 : Armor, 3 : Pants, 4 : Shield
    [HideInInspector]
    public List<Item> equipment;

    Animator anim;
    public WeaponCollider wpnColl;
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
        playerAttack.SetUpEffect(equipment[0].effectName); // 첫 공격 effect
        
        List<Stat> temp = new List<Stat>();
        for (int i = 0; i < equipment.Count; i++)
            temp.Add(equipment[i].stat);
        stat.SyncStat(temp);

        // start hp is max
        stat.hp = stat.maxHp;
        
        // used in animator end event - death
        anim.GetComponent<PlayerAnimreciver>().onDieComplete = () =>
        {
            // hide character
            //gameObject.SetActive(false);
            
            // disable physics
            GetComponent<CapsuleCollider2D>().enabled = false;
            rig.velocity = new Vector2(0, 0);
            rig.isKinematic = true;
        };

        // used in animator end event - attack
        anim.GetComponent<PlayerAnimreciver>().onAttackComplete = () =>
        {
            // update state
            curState = PlayerState.Normal;

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
            // update state
            curState = PlayerState.Normal;
            
            //// disable attack collider
            //wpnColl.poly.enabled = false;

            // reset elasped skill cool-time
            //remainCool = equipment[0].stat.coolTime;

            // !!!!!!!! 테스트용 코드 - 스킬 쿨타임 0 !!!!!!!!!
            remainCool = 0;
        };

        // used in animator end event - stun
        anim.GetComponent<PlayerAnimreciver>().onStunComplete = () =>
        {
            // 무적 시간 측정 시작
            StartCoroutine(Grace());
        };

        // player stat variables init
        remainCool = -1f;
        dashCool = -1f;
        curState = PlayerState.Normal;

        // attack & skill range init
        wpnColl.SetAttackRange(equipment[0].stat.range);
    }


    void Update()
    {
        remainCool -= Time.deltaTime;
        if (remainCool < -100.0f)
            remainCool = -1.0f;

        dashCool -= Time.deltaTime;
        if (dashCool < -100.0f)
            dashCool = -1.0f;

        // should not work in dead condition
        if (curState == PlayerState.Dead)
            return;

        // get move-related input
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0).normalized;
        
        if (curState == PlayerState.Normal || curState == PlayerState.Invincible)
        {
            // FLIP character depending on heading direction
            if (moveInput.x > 0 && transform.localScale.x > 0)
            {
                transform.localScale =
                    new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
            else if (moveInput.x < 0 && transform.localScale.x < 0)
            {
                transform.localScale =
                    new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }

            // change character's position
            rig.velocity = moveInput * stat.speed;

            // change animation depending on speed
            anim.SetFloat("Speed", moveInput.magnitude * stat.speed);
        }

        /**
        * Input Handling
        */

        // Attack Input
        if (Input.GetButtonDown("Fire1") && curState != PlayerState.Attacking)
        {
            // update weapon state
            anim.SetInteger("WpnState", equipment[0].itemType);

            // change animation to attack
            anim.SetTrigger("Attack");
            
            // update current state to attacking
            curState = PlayerState.Attacking;

            // cannot move - freeze
            rig.velocity = Vector2.zero;
            
            // play effect
            playerAttack.Attack(equipment[0].effectName);
            
            // TODO - play sound => 이상하면 고쳐야 함
            // string 검색 코스트가 굉장히 비싸서 앞글자만 따서 검색하는 형식으로 코딩
            switch (equipment[0].effectName[0])
            {
                case 'N': // Normal
                    SoundManager.Instance.SoundPlay(SoundType.PlayerAttack_Normal);
                    break;
                
                case 'E':
                    SoundManager.Instance.SoundPlay(SoundType.PlayerAttack_Electric);
                    break;
                
                case 'F': // Fire
                    SoundManager.Instance.SoundPlay(SoundType.PlayerAttack_Fire);
                    break;
            }

            // enable weapon collider
            wpnColl.poly.enabled = true;
        }
        
        // test debugging skill cool-time
        if (Input.GetButtonDown("Fire2"))
        {
            print("remaining skill cool : " + remainCool);
        }

        // test skill input
        if (Input.GetButtonDown("Fire2") && remainCool <= 0.0f && curState != PlayerState.Attacking)
        {
            // update weapon state
            anim.SetInteger("WpnState", equipment[0].itemType);

            // change animation to skill
            anim.SetTrigger("Skill");

            // update current state to attacking
            curState = PlayerState.Attacking;
            
            // cannot move - freeze
            rig.velocity = Vector2.zero;

            // 스킬 관련 구현
            if (equipment[0] != null)
            {
                SkillManager.Instance.InstantiateSkill(equipment[0].skillName);
            }
            //playerAttack.SkillAttack(equipment[0].id);
        }
        
        // dash input
        if (Input.GetKeyDown(KeyCode.LeftShift) && dashCool <= 0.0f && moveInput.magnitude != 0 && curState == PlayerState.Normal)
            StartCoroutine(Dash());

            // test code - change equipments
        if (Input.GetKeyDown(KeyCode.Alpha9)) // bow
            Equip(ItemManager.Instance.GetItem(81));
        if (Input.GetKeyDown(KeyCode.Alpha0)) // staff
            Equip(ItemManager.Instance.GetItem(61));
        if (Input.GetKeyDown(KeyCode.Alpha1)) // 1 - sword1
            Equip(ItemManager.Instance.GetItem(2));
        if (Input.GetKeyDown(KeyCode.Alpha2)) // 2 - sword2
            Equip(ItemManager.Instance.GetItem(3));
        if (Input.GetKeyDown(KeyCode.Alpha3)) // 3 - sword6
            Equip(ItemManager.Instance.GetItem(4));
        if (Input.GetKeyDown(KeyCode.Alpha4)) // 4 - sword8
            Equip(ItemManager.Instance.GetItem(5));
        if (Input.GetKeyDown(KeyCode.Alpha5)) // 5 - sword3 (rare)
            Equip(ItemManager.Instance.GetItem(22));
        if (Input.GetKeyDown(KeyCode.P))
        {
            print("MaxHP : " + stat.maxHp + "\nHP : " + stat.hp + "\nAttackPower : " + stat.damage
               + "\nAttackRange : " + stat.range + "\nSkillPower : " + stat.skillDamage
               + "\nSpeed : " + stat.speed + "\nKnockBackForce : " + stat.knockBackForce 
               + "\nCoolTime : " + stat.coolTime);
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
    // Player 아이템 초기화
    // -------------------------------------------------------------
    public void EquipInit()
    {
        for(int i = 0; i < equipment.Count; i++)
            UnEquip(equipment[i]);
        equipment.Clear();
        equipment = new List<Item> { ItemManager.Instance.GetItem(0), // weapon
                                     ItemManager.Instance.GetItem(7), // helmet
                                     ItemManager.Instance.GetItem(10), // armor
                                     ItemManager.Instance.GetItem(15), // pants
                                     ItemManager.Instance.GetItem(1)  // shield
                                    };
        for (int i = 0; i < equipment.Count; i++)
            Equip(equipment[i]);
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
        
        // 근접 무기 -> 활 / 스태프로 무기 변경 시 방패 자동 장착 해제
        if (partsIndex == 0)
        {
            if (equipment[0].itemType == 1 && (item.itemType == 2 || item.itemType == 3))
            {
                UnEquip(equipment[4]);
                
                // 끼던 방패를 DroppedItem으로 생성
                var shieldDropped = GameManager.Instance.CreateGO
                (
                    "Prefabs/Dungeon/Dropped",
                    DungeonSystem.Instance.DroppedItems.transform
                );
                
                shieldDropped.transform.position = this.gameObject.transform.position;
                shieldDropped.GetComponent<DroppedItem>().Set(equipment[4]);
                
                equipment[4] = ItemManager.Instance.GetItem(1);
            }
            playerAttack.SetUpEffect(item: item);// effect setting
        }

        // 활 / 스태프 -> 방패 장착 시 활 / 스태프 자동 장착 해제
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

    // -------------------------------------------------------------
    // Player 피격
    // -------------------------------------------------------------

    public void OnDamage(float damage, float knockBackForce, Vector2 direction)
    {
        if (curState == PlayerState.Invincible || curState == PlayerState.Stunned || curState == PlayerState.Dead)
            return;

        stat.Damaged(damage);

        // trigger die if health is below 0
        if (stat.hp == 0)
            Die();
        else
        {
            // TODO - 피격 사운드 삽입

            // change animation to stunned
            anim.SetTrigger("Hit");

            // 피격당했음
            curState = PlayerState.Stunned;

            // 넉백
            StartCoroutine(KnockBack(knockBackForce, direction));

            // debug
            print("Player's health : " + stat.hp);
        }
    }
    
    // -------------------------------------------------------------
    // Player 사망
    // -------------------------------------------------------------

    public void Die()
    {
        // update state
        curState = PlayerState.Dead;
        
        // TODO Death 사운드 적용
        
        // change animation to death
        anim.SetTrigger("Die");
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
    
    
    /*
     *   Coroutine 함수
    */
    
    // -------------------------------------------------------------
    // Player 넉백
    // -------------------------------------------------------------
    IEnumerator KnockBack(float knockBackForce, Vector2 direction)
    {
        rig.velocity = Vector2.zero;
        rig.AddForce(direction * knockBackForce, ForceMode2D.Impulse);
        Vector2 orig = rig.velocity;
        
        // TODO 넉백 사운드 적용

        // 속도가 0일 때까지 0.05초마다 힘 가해서 감속
        // TODO 버그 존재
        while (((orig.x > 0 && rig.velocity.x > 0) || (orig.x < 0 && rig.velocity.x < 0))
               && ((orig.y > 0 && rig.velocity.y > 0) || (orig.y < 0 && rig.velocity.y < 0)))
        {
            rig.AddForce(-7 * direction * knockBackForce, ForceMode2D.Force);
            yield return GameManager.Instance.Setwfs(5);
        }

        rig.velocity = Vector2.zero;

    }
    
    // -------------------------------------------------------------
    // Player Dash 대시
    // -------------------------------------------------------------
    IEnumerator Dash()
    {
        // 속도 3배로
        curState = PlayerState.Dashing;
        rig.velocity *= 3;
        
        // 사운드 출력
        SoundManager.Instance.SoundPlay(SoundType.PlayerDash);

        // 0.1초 유지
        yield return GameManager.Instance.Setwfs(10);
        
        // 쿨타임 부여 - 2초
        dashCool = 2f;

        // 속도 원래대로
        rig.velocity /= 3;
        curState = PlayerState.Normal;
    }

    // -------------------------------------------------------------
    // Player Grace - 무적 시간 측정
    // -------------------------------------------------------------
    IEnumerator Grace()
    {
        // 스턴 종료 & 무적 시간 시작
        curState = PlayerState.Invincible;

        // 무적 시간 설정
        yield return GameManager.Instance.Setwfs(50);
        
        // 다시 피격 가능하게 조정
        curState = PlayerState.Normal;
    }
}