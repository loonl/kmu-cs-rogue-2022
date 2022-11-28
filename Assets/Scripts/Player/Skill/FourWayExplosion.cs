using System.Collections;
using System.Collections.Generic;
using static SkillManager;
using UnityEngine;


public class FourWayExplosion : BaseSkill
{
    bool canGenerateNextExplosion = true; // 다음 이펙트 생성 가능 여부
    bool firstgenerated = false; // 처음 생성된 이펙트?
    Vector2 direction;
    [SerializeField] float NextExplosionGenerateTimeF = 0.15f; // 다음 폭발 생성 딜레이, 이펙트 애니메이션 길이보다 짧아야함!

    protected override void Start()
    {
        init();
        SetDirection();
        StartCoroutine(ExecuteSkill());
    }
    protected override void init()
    {
        base.init();
        knockbackPower = 5f;
    }

    private void SetDirection()
    {
        if (Instance.onGoingSkillInfo.Count != 0) //딕셔너리에 정보가 있음 - 스포너 오브젝트가 아님
        {
            direction = Instance.GetDirectionFromObject(transform, ((GameObject)Instance.onGoingSkillInfo[SkillInfo.SpawnerObject]).transform);
            return;
        }
        firstgenerated = true;
        SkillManager.Instance.onGoingSkillInfo.Add(SkillManager.SkillInfo.AliveEffectCount, 4);
        Instance.onGoingSkillInfo.Add(SkillInfo.SpawnerObject, gameObject);

        SetPosition();
    }
    protected override void SetPosition()
    {
        gameObject.transform.position = player.transform.position;
    }

    protected override IEnumerator SkillAction()
    {
        if (firstgenerated)
        {
            yield return GameManager.Instance.Setwfs(20);
            List<DirectionName> dirs = new List<DirectionName>() { DirectionName.Up, DirectionName.Right, DirectionName.Down, DirectionName.Left };
            for(int i = 0; i < dirs.Count; i++)
            {
                Vector2 direction = Instance.DirectionDict[dirs[i]];
                Vector2 newpos = (Vector2)gameObject.transform.position + new Vector2(gameObject.transform.localScale.x / 2 * direction.x, gameObject.transform.localScale.y / 2 * direction.y);
                Instantiate(Resources.Load("Prefabs/Skill/FourWayExplosion"), newpos, Quaternion.identity, gameObject.transform); // 새 이펙트 생성
            }
            yield break;
        }

        animator.SetTrigger(weapon.skillName);
        
        collid.enabled = true;
        yield return colliderValidTime;
        collid.enabled = false;
        monsters.Clear();
        StartCoroutine(GenerateNextExplosion()); // 다음 이펙트 생성 시도
    }

    IEnumerator GenerateNextExplosion()
    {
        yield return GameManager.Instance.Setwfs((int)(NextExplosionGenerateTimeF * 100));
        if (!canGenerateNextExplosion) // 다음 이펙트 못 만들 시
        {
            int count = (int)Instance.onGoingSkillInfo[SkillInfo.AliveEffectCount];
            count--;
            if(count == 0) // 이펙트 전부 소멸 시
            {
                Instance.DestroySpawnerObject((GameObject)Instance.onGoingSkillInfo[SkillInfo.SpawnerObject]);                
            }
            else
            {
                Instance.onGoingSkillInfo[SkillInfo.AliveEffectCount] = count;
            }
            yield break;
        }
        Vector2 newpos = (Vector2)gameObject.transform.position + new Vector2(gameObject.transform.localScale.x /2 * direction.x, gameObject.transform.localScale.y /2* direction.y);
        Instantiate(Resources.Load("Prefabs/Skill/FourWayExplosion"), newpos, Quaternion.identity, ((GameObject)Instance.onGoingSkillInfo[SkillInfo.SpawnerObject]).transform); // 새 이펙트 생성
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Wall Tile Map") { // 이펙트가 벽과 충돌: 더이상 이펙트를 만들지 못함.
            canGenerateNextExplosion = false; 
        }
        if (collision.gameObject.CompareTag("Monster")) // 스킬 히트박스 내의 몬스터를 리스트에 담기
        {
            monsters.Add(collision);
        }
    }
}
