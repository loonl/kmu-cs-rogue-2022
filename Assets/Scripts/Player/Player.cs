using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [HideInInspector]
    public Animator anim;
    [HideInInspector]
    public WeaponCollider wpnColl;
    Rigidbody2D rig;
    [HideInInspector]
    public SPUM_SpriteList spumMgr;
    [HideInInspector]
    public ArrowGenerate arrowGen;
    Staff staff;

    // 인벤토리
    public Inventory Inventory { get; private set; }

    // 상호작용
    public Interact _interact;
    
    // 이동정보
    [HideInInspector]
    public Vector3 moveInfo;

    PlayerAnimreceiver playerAnimreceiver;
    private Vector3 moveInput;

    private void Awake()
    {
        this.Inventory = GameObject.Find("Inventory").GetComponent<Inventory>();
    }

    void Start()
    {
        // GameManager에 Player 할당
        GameManager.Instance.Player = this;
        
        anim = transform.GetChild(0).gameObject.GetComponent<Animator>();
        playerAnimreceiver = anim.GetComponent<PlayerAnimreceiver>();
        wpnColl = transform.GetChild(0).gameObject.GetComponent<WeaponCollider>();
        spumMgr = transform.GetChild(0).GetChild(0).GetComponent<SPUM_SpriteList>();
        rig = GetComponent<Rigidbody2D>();
        arrowGen = gameObject.GetComponent<ArrowGenerate>();
        staff = gameObject.GetComponent<Staff>();

        // 첫 장비 설정
        EquipInit();
        
        List<Stat> temp = new List<Stat>();
        for (int i = 0; i < equipment.Count; i++)
            temp.Add(equipment[i].stat);
        stat.SyncStat(temp);

        // start hp is max
        stat.hp = stat.maxHp;
        
        // 애니메이터 이벤트 init
        AnimEventInit();

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
        //Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0).normalized;
        moveInfo = moveInput;

        if (curState == PlayerState.Normal || curState == PlayerState.Invincible || curState == PlayerState.Attacking)
        {
            if (curState != PlayerState.Attacking)
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

        // if (Input.GetButtonDown("Fire1") && curState != PlayerState.Attacking)
        // {
        //     // update weapon state
        //     anim.SetInteger("WpnState", equipment[0].itemType);
        //
        //     // change animation to attack
        //     // change animation to attack
        //     anim.SetTrigger("Attack");
        //     
        //     // update current state to attacking
        //     curState = PlayerState.Attacking;
        //
        //     //// cannot move - freeze
        //     //rig.velocity = Vector2.zero;
        //
        //     // play effect
        //     if (equipment[0].itemType == 1)
        //     {
        //         wpnColl.Attack(equipment[0].effectName);
        //     }
        //
        //     else if (equipment[0].itemType == 3)
        //         staff.Attack(equipment[0].effectName);
        //     
        //     // 활 공격은 애니메이션 이벤트에서 행해짐
        //
        //     // TODO - 공격 sound => 이상하면 고쳐야 함
        //     switch (equipment[0].effectName)
        //     {
        //         case "NormalSlash":
        //         case "NormalSlash2":
        //             SoundManager.Instance.SoundPlay(SoundType.PlayerAttack_Normal);
        //             break;
        //         
        //         case "ElectricSlash":
        //             SoundManager.Instance.SoundPlay(SoundType.PlayerAttack_Electric);
        //             break;
        //         
        //         case "FireSlash":
        //             SoundManager.Instance.SoundPlay(SoundType.PlayerAttack_Fire);
        //             break;
        //     }
        // }

        
        // test debugging skill cool-time
        //if (Input.GetButtonDown("Fire2"))
        //{
        //    print("remaining skill cool : " + remainCool);
        //}

        // test skill input
        // if (Input.GetButtonDown("Fire2") && remainCool <= 0.0f && curState != PlayerState.Attacking)
        // {
        //     // update weapon state
        //     anim.SetInteger("WpnState", equipment[0].itemType);
        //
        //     // change animation to skill
        //     if (equipment[0].skillName == "RapidArrow")
        //     {
        //         anim.SetTrigger("BowGroundSkill");
        //         
        //         // 쿨타임 적용
        //         SkillCoolDown.Instance.TriggerSkill();
        //     }
        //     else
        //         anim.SetTrigger("Skill");
        //
        //     // update current state to attacking
        //     curState = PlayerState.Attacking;
        //
        //     // 스킬 관련 구현
        //     SkillCoolDown.Instance.TriggerSkill();
        //
        //     if (equipment[0] != null)
        //     {
        //         // 근접 무기 스킬
        //         if (equipment[0].itemType == 1)
        //         {
        //             SkillCoolDown.Instance.TriggerSkill();
        //             SkillManager.Instance.InstantiateSkill(equipment[0].skillName);
        //         }
        //
        //         // 활 스킬은 애니메이션 delegate 쪽에서 호출 - onBowSkillStart 참조
        //
        //         //스태프 스킬
        //         else if (equipment[0].itemType == 3)
        //             staff.Attack(equipment[0].skillName, false);
        //     }
        // }
        
        // dash input
        // if (Input.GetKeyDown(KeyCode.LeftShift) && dashCool <= 0.0f && moveInput.magnitude != 0 && curState == PlayerState.Normal)
        //     StartCoroutine(Dash());
        
        /**
        if (Input.GetKeyDown(KeyCode.Alpha8)) // bow
            Equip(ItemManager.Instance.GetItem(31));
        // test code - change equipments
        if (Input.GetKeyDown(KeyCode.Alpha9)) // bow
            Equip(ItemManager.Instance.GetItem(59));
        if (Input.GetKeyDown(KeyCode.Alpha0)) // staff
            Equip(ItemManager.Instance.GetItem(60));
        if (Input.GetKeyDown(KeyCode.Alpha1)) // 1 - sword1
            Equip(ItemManager.Instance.GetItem(1));
        if (Input.GetKeyDown(KeyCode.Alpha2)) // 2 - sword2
            Equip(ItemManager.Instance.GetItem(2));
        if (Input.GetKeyDown(KeyCode.Alpha3)) // 3 - sword6
            Equip(ItemManager.Instance.GetItem(3));
        if (Input.GetKeyDown(KeyCode.Alpha4)) // 4 - sword8
            Equip(ItemManager.Instance.GetItem(4));
        if (Input.GetKeyDown(KeyCode.Alpha5)) // 5 - sword3 (rare)
            Equip(ItemManager.Instance.GetItem(21));
        if (Input.GetKeyDown(KeyCode.Alpha6)) // 6 - Cheat Weapon
            Equip(ItemManager.Instance.GetItem(89));
        if (Input.GetKeyDown(KeyCode.Equals))
            stat.hp = stat.maxHp;
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            print("MaxHP : " + stat.maxHp + "\nHP : " + stat.hp + "\nAttackPower : " + stat.damage
               + "\nAttackRange : " + stat.range + "\nSkillPower : " + stat.skillDamage
               + "\nSpeed : " + stat.speed + "\nKnockBackForce : " + stat.knockBackForce 
               + "\nCoolTime : " + stat.coolTime + "\nGameScore : " + GameManager.Instance.score);
        }

        // 상호작용
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (this._interact == null)
            {
                return;
            }

            this._interact.InteractEvent();
        }*/
    }
    
    // -------------------------------------------------------------
    // Player 애니메이션 관련 이벤트 초기 설정
    // -------------------------------------------------------------
    public void AnimEventInit()
    {
        // 사망 시
        playerAnimreceiver.onDieComplete = () =>
        {
            // hide character
            //gameObject.SetActive(false);
            
            // disable physics
            GetComponent<CapsuleCollider2D>().enabled = false;
            rig.velocity = new Vector2(0, 0);
            rig.isKinematic = true;
        };

        // 공격 종료
        playerAnimreceiver.onAttackComplete = () =>
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

        // 스킬 종료
        playerAnimreceiver.onSkillComplete = () =>
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

        // 스턴 종료
        playerAnimreceiver.onStunComplete = () =>
        {
            // 무적 시간 측정 시작
            StartCoroutine(NoHit(50));
        };

        // 화살 쏴야할 때
        playerAnimreceiver.onArrowShoot = () =>
        {
            arrowGen.Attack(equipment[0].effectName);
        };
        
        // 활 스킬 시작해야 할 때
        anim.GetComponent<PlayerAnimreceiver>().onBowSkillStart = () =>
        {
            SkillCoolDown.Instance.TriggerSkill();
            SkillManager.Instance.InstantiateSkill(equipment[0].skillName);
        };
        
        // 활 스킬 중에서 화살 쏴야 할 때
        anim.GetComponent<PlayerAnimreceiver>().onSkillArrowShoot = () =>
        {
            anim.SetBool("SkillFinished", false);
            SkillManager.Instance.InstantiateSkill(equipment[0].skillName);
        };
    }

    // -------------------------------------------------------------
    // Player 아이템 초기화
    // -------------------------------------------------------------
    public void EquipInit()
    {
        equipment = new List<Item> { ItemManager.Instance.GetItem(1), // weapon
                                     ItemManager.Instance.GetItem(6), // helmet
                                     ItemManager.Instance.GetItem(9), // armor
                                     ItemManager.Instance.GetItem(14), // pants
                                     ItemManager.Instance.GetItem(0)  // shield
                                    };
        for (int i = 0; i < equipment.Count; i++)
            Equip(equipment[i], true);
    }

    // -------------------------------------------------------------
    // Player 아이템 착용 / 해제
    // true = 정상 작동, false = 비정상 작동
    // -------------------------------------------------------------
    public bool Equip(Item item, bool first = false)
    {
        // 바뀌는 장비가 어느 부위인지 판단
        int partsIndex;
        int type = item.itemType;
        if (type == 1 || type == 2 || type == 3)
            partsIndex = 0;
        else
            partsIndex = item.itemType - 3;
        
        
        // 근접 무기
        if (partsIndex == 0 && type == 1)
        {
            wpnColl.SetUpEffect(item: item);
        }
        
        // 활
        else if (partsIndex == 0 && type == 2)
        {
            // 근접 무기 -> 활로 무기 변경 시 방패 자동 장착 해제
            if (equipment[0].itemType == 1 && !equipment[4].isEmpty())
            {
                // 방패 un-equip 후 DroppedItem으로 생성
                UnEquip(equipment[4]);
                MakeDroppedItem(equipment[4], true);
                
                // Index 업데이트
                equipment[4] = ItemManager.Instance.GetItem(0);
            }
            
            // 쏠 화살 변경
            arrowGen.ChangeIndex(item.effectName);
        }
        
        // 스태프
        else if (partsIndex == 0 && type == 3)
        {
            // 근접 무기 -> 스태프로 무기 변경 시 방패 자동 장착 해제
            if (equipment[0].itemType == 1 && !equipment[4].isEmpty())
            {
                // 방패 un-equip 후 DroppedItem으로 생성
                UnEquip(equipment[4]);
                MakeDroppedItem(equipment[4], true);
                
                // Index 업데이트
                equipment[4] = ItemManager.Instance.GetItem(0);
            }
            
            // 효과 적용
            // magic.SetupEffect(item: item);
        }
        
        // 방패 
        else if (partsIndex == 4)
        {
            // 활 / 스태프 -> 방패 장착 시도 시엔 장착 불가
            if (equipment[0].itemType == 2 || equipment[0].itemType == 3)
            {
                Debug.Log("Cannot Equip Shield!");
                
                // 비정상 작동 알림
                return false;
            }
        }

        // 입고 있는 것 먼저 un-equip
        if (!item.isEmpty() && !first)
            UnEquip(equipment[partsIndex]);

        // 플레이어 스탯 수정
        List<Stat> itemStat = new List<Stat> { item.stat };
        stat.SyncStat(itemStat);

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

        // 장착 아이템 index 업데이트 
        equipment[partsIndex] = item;
        
        // 정상 작동 알림
        return true;
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
        if (curState == PlayerState.Invincible || curState == PlayerState.Stunned || curState == PlayerState.Dead || curState == PlayerState.Dashing)
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
        
        // BestScore 저장 - TODO 온라인 연동 기능 추가
        GameObject.Find("MainCanvas").SetActive(false);
        GameObject.Find("MobileCanvas").SetActive(false);
        GameObject.Find("DynamicCanvas(Clone)").SetActive(false);
        Instantiate(Resources.Load<GameObject>("Prefabs/UI/ScoreCanvas"));

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
    
    // -------------------------------------------------------------
    // DroppedItem 생성
    // -------------------------------------------------------------
    private void MakeDroppedItem(Item item, bool beside = false)
    {
        // 없어진 방패 재생성
        var GO = GameManager.Instance.CreateGO
        (
            "Prefabs/Dungeon/Dropped",
            DungeonSystem.Instance.DroppedItems.transform
        );
                
        // 같은 위치 혹은 그 옆에 생성
        if (beside)
            GO.transform.position = this.gameObject.transform.position;
        else
            GO.transform.position = this.gameObject.transform.position + new Vector3(2f, 0, 0);
        
        GO.GetComponent<DroppedItem>().Set(item);
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
        
        // 쿨타임 부여 - 1.5초
        dashCool = 1.5f;

        // 속도 원래대로
        rig.velocity /= 3;
        curState = PlayerState.Normal;
    }

    // -------------------------------------------------------------
    // Player NoHit - 무적 시간 측정
    // -------------------------------------------------------------
    public IEnumerator NoHit(int time)
    {
        // 스턴 종료 & 무적 시간 시작
        curState = PlayerState.Invincible;

        // 무적 시간 설정
        yield return GameManager.Instance.Setwfs(time);
        
        // 다시 피격 가능하게 조정
        curState = PlayerState.Normal;
    }
    
    // 충돌 체크
    private void OnCollisionEnter2D(Collision2D collision) // 대쉬 중 몬스터와 충돌 무시
    {
        if(curState == PlayerState.Dashing && collision.gameObject.CompareTag("Monster"))
        {
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), GetComponent<CapsuleCollider2D>());
        }
    }
    
    // -------------------------------------------------------------
    public void SetAngle(Vector2 Dir)
    {
        moveInput = Dir;

    }

    public void DashBtn()
    {
        if(dashCool <= 0.0f && moveInput.magnitude != 0 && curState == PlayerState.Normal)
        {
            //print(123);
            StartCoroutine(Dash());
        }
    }

    public void SkillBtn()
    {
        if (remainCool <= 0.0f && (curState == PlayerState.Normal || curState == PlayerState.Invincible))
        {
            // update weapon state
            anim.SetInteger("WpnState", equipment[0].itemType);

            // change animation to skill
            if (equipment[0].skillName == "RapidArrow")
            {
                anim.SetTrigger("BowGroundSkill");
                
                // 쿨타임 적용
                SkillCoolDown.Instance.TriggerSkill();
            }
            else
                anim.SetTrigger("Skill");

            // update current state to attacking
            curState = PlayerState.Attacking;

            // 스킬 관련 구현
            SkillCoolDown.Instance.TriggerSkill();

            if (equipment[0] != null)
            {
                // 근접 무기 스킬
                if (equipment[0].itemType == 1)
                {
                    SkillCoolDown.Instance.TriggerSkill();
                    SkillManager.Instance.InstantiateSkill(equipment[0].skillName);
                }

                // 활 스킬은 애니메이션 delegate 쪽에서 호출 - onBowSkillStart 참조

                //스태프 스킬
                else if (equipment[0].itemType == 3)
                    staff.Attack(equipment[0].skillName, false);
            }
        }
    }

    public void AttackBtn()
    {
        if (this._interact != null)
        {
            this._interact.InteractEvent();
        }
        else
        {
            if (curState == PlayerState.Normal || curState == PlayerState.Invincible)
            {
                // update weapon state
                anim.SetInteger("WpnState", equipment[0].itemType);

                // change animation to attack
                anim.SetTrigger("Attack");

                // update current state to attacking
                curState = PlayerState.Attacking;

                //// cannot move - freeze
                //rig.velocity = Vector2.zero;

                // play effect
                if (equipment[0].itemType == 1)
                {
                    wpnColl.Attack(equipment[0].effectName);
                }

                else if (equipment[0].itemType == 3)
                    staff.Attack(equipment[0].effectName);

                // 활 공격은 애니메이션 이벤트에서 행해짐

                // TODO - 공격 sound => 이상하면 고쳐야 함
                switch (equipment[0].effectName)
                {
                    case "NormalSlash":
                    case "NormalSlash2":
                        SoundManager.Instance.SoundPlay(SoundType.PlayerAttack_Normal);
                        break;

                    case "ElectricSlash":
                        SoundManager.Instance.SoundPlay(SoundType.PlayerAttack_Electric);
                        break;

                    case "FireSlash":
                        SoundManager.Instance.SoundPlay(SoundType.PlayerAttack_Fire);
                        break;
                }
            }
        }

    }
}