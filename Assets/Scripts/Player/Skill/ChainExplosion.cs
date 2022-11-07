using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


public class ChainExplosion : BaseSkill
{
    bool canGenerateNextExplosion = true; // 다음 이펙트 생성 가능 여부
    bool firstgenerated = false; // 처음 생성된 이펙트?
    Vector2 direction;
    [SerializeField] float NextExplosionGenerateTimeF = 0.15f; // 다음 폭발 생성 딜레이, 이펙트 애니메이션 길이보다 짧아야함!

    protected override void Start()
    {
        init();
        SetDirection();
        ExecuteSkill();
    }
    protected override void init()
    {
        base.init();
        knockbackPower = 3f;
    }

    private void SetDirection()
    {
        if (SkillManager.Instance.onGoingSkillInfo.Count != 0) // 이펙트 정보 딕셔너리에 정보가 있음 - 처음 생성된 이펙트가 아님
        {
            direction = (Vector2)SkillManager.Instance.onGoingSkillInfo[SkillManager.SkillInfo.Direction];
            return;
        }
        firstgenerated = true;
        Vector2 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        direction = (mousepos - (Vector2)player.transform.position).normalized; // 마우스 위치를 가리키는 방향 벡터
        Debug.Log(direction.magnitude);
        SkillManager.Instance.onGoingSkillInfo.Add(SkillManager.SkillInfo.Direction, direction); // 처음 생성된 이펙트 - 스킬 정보에 방향 저장

        SetPosition();
    }
    protected override void SetPosition()
    {
        gameObject.transform.position = player.transform.position + new Vector3(gameObject.transform.localScale.x / 2 * direction.x, gameObject.transform.localScale.y / 2 * direction.y, 0); 
    }

    protected override IEnumerator SkillAction()
    {   
        if(firstgenerated) yield return new WaitForSeconds(0.2f);
        
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
            SkillManager.Instance.onGoingSkillInfo.Clear(); // 스킬 정보 초기화
            yield break;
        }
        Vector2 newpos = gameObject.transform.position + new Vector3(gameObject.transform.localScale.x /2 * direction.x, gameObject.transform.localScale.y /2* direction.y, 0);
        Instantiate(Resources.Load("Prefabs/Skill/ChainExplosion"), newpos, Quaternion.identity); // 새 이펙트 생성
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
