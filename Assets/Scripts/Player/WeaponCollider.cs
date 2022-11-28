using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public class WeaponCollider : MonoBehaviour
{
    Player player;
    ArcCollider2D arc;
    Transform effectTransform; // for changing attack effect size or flipping
    public PolygonCollider2D poly;
    public Animator effectanim;
    public List<Collider2D> monsters = new List<Collider2D>();


    private void Awake()
    {
        Init();
    }
    //void Start()
    //{
    //    Init();
    //}

    protected virtual void Init()
    {
        player = transform.GetComponentInParent<Player>();
        arc = GetComponent<ArcCollider2D>();
        poly = GetComponent<PolygonCollider2D>();
        poly.enabled = false;
        effectanim = player.transform.GetChild(0).GetChild(2).GetComponent<Animator>();
        effectTransform = player.transform.GetChild(0).GetChild(2).GetComponent<Transform>();
        
    }

    public void SetAttackRange(float value = 1)
    {
        arc.radius = value;
        poly.points = arc.getPoints(poly.offset);
    }

    public void Attack(string effectname)
    {
        effectanim.SetTrigger(effectname);
        StartCoroutine(EnableCollider());
    }


    public IEnumerator EnableCollider()
    {
        poly.enabled = true;
        yield return GameManager.Instance.Setwfs(10);
        poly.enabled = false;
    }

    public void SetUpEffect(string effectname = "NormalSlash2", Item item = null, float range = 1f)
    {
        if (item != null)
        {   
            effectname = item.effectName;
            range = item.stat.range;
            player.wpnColl.SetAttackRange(range); // 범위 설정  
        }
        switch (effectname)
        {
            case "NormalSlash":
                effectTransform.localScale = new Vector2(range * 2.5f, range * 2.5f);
                effectTransform.eulerAngles = new Vector3(0, 0, 64f); // rotation
                effectTransform.localPosition = new Vector3(range * 0.08f, range * 0.8f, 0);
                break;
            case "NormalSlash2":
                effectTransform.localScale = new Vector2(range * 1.5f, range * 3f);
                effectTransform.eulerAngles = new Vector3(0, 0, 0); // rotation
                effectTransform.localPosition = new Vector3(0.2f, -0.2f, 0);
                break;
            case "NormalSlash3":
                effectTransform.localScale = new Vector2(range * 1.5f, -range * 2.5f);
                effectTransform.eulerAngles = new Vector3(0, 0, 0); // rotation
                effectTransform.localPosition = new Vector3(0.04f, -0.09f, 0);
                break;
            case "FireSlash":
            case "ElectricSlash":
                effectTransform.localScale = new Vector2(range * 1.5f, -range * 2.5f);
                effectTransform.eulerAngles = new Vector3(0, 0, 0); // rotation
                effectTransform.localPosition = new Vector3(0.08f, -0.09f, 0);
                break;
            case "ElectricSlash2":
            case "FireSlash2":
                effectTransform.localScale = new Vector2(range * 2f, range * 2.5f);
                effectTransform.eulerAngles = new Vector3(0, 0, 30); // rotation
                effectTransform.localPosition = new Vector3(0.08f, 0.28f, 0);
                break;
            case "FireSlash3":
            case "ElectricSlash3":
                effectTransform.localScale = new Vector2(range * 1.5f, range * 3.2f);
                effectTransform.eulerAngles = new Vector3(0, 0, 0); // rotation
                effectTransform.localPosition = new Vector3(0, -0.2f, 0);
                break;
            case "ElectricClaw":
            case "NormalClaw":
            case "FireClaw":
                effectTransform.localScale = new Vector2(range * 1.5f, range * 2f);
                effectTransform.eulerAngles = new Vector3(0, 0, 0); // rotation
                effectTransform.localPosition = new Vector3(-0.21f, 0.07f, 0);
                break;
            default:
                effectTransform.localScale = new Vector2(range * 1.5f, range * 3);
                effectTransform.eulerAngles = new Vector3(0, 0, 0);
                effectTransform.position = player.transform.position;
                break;
        }
        if (player.transform.localScale.x < 0)
        {
            effectTransform.eulerAngles = new Vector3(0, 0, -effectTransform.eulerAngles.z); // 플레이어가 오른쪽 보고있으면 의도한대로 rotation이 저장이 안되서 수정해주는 코드
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Monster")) // 공격 범위 내의 몬스터를 리스트에 담기
        {
            monsters.Add(collision);
        }
    }
    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (monsters.Contains(collision)) // monsters 리스트에 없다면 이는 몬스터가 아님.
        {
            Monster target = collision.gameObject.GetComponent<Monster>();
            if (!target.isInvulnerable) target.OnDamage(player.stat.damage, player.stat.knockBackForce, (target.transform.position - transform.position).normalized, GameManager.Instance.Setwfs(10)); // 대미지 주기
        }
        if (collision.gameObject.CompareTag("MapObject"))
        {
            collision.gameObject.SendMessage("OnDamage");
        }
    }
    //private void OnTriggerStay2D(Collider2D collision)
    //{
    //    Monster target;
    //    target = collision.GetComponent<Monster>();
    //    if (target != null  && !monsters.Contains(target) && player.curState == PlayerState.Attacking)
    //    {
    //        monsters.Add(target);
    //        // execute ondamage function when monster is in range
    //        Monster attackTarget = collision.gameObject.GetComponent<Monster>();
            
    //        // playerUnit.stat.knockBackForce => 기존에 5f였음
    //        attackTarget.OnDamage(player.stat.damage, player.stat.knockBackForce, (attackTarget.transform.position - transform.position).normalized, GameManager.Instance.Setwfs(10)); 
    //    }

    //    if (collision.gameObject.CompareTag("MapObject"))
    //    {
    //        collision.gameObject.SendMessage("OnDamage");
    //    }
    //}

}
