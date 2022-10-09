using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCollider : MonoBehaviour
{
    Player playerUnit;
    ArcCollider2D arc;
    public PolygonCollider2D poly;
    public List<Monster> monsters;

    // Start is called before the first frame update
    void Start()
    {
        playerUnit = transform.GetComponentInParent<Player>();
        arc = GetComponent<ArcCollider2D>();
        poly = GetComponent<PolygonCollider2D>();
        poly.enabled = false;
    }

    public void SetAttackRange(float value)
    {
        arc.radius = value;
    } 

    private void OnTriggerStay2D(Collider2D collision)
    {
        Monster target;
        target = collision.GetComponent<Monster>();
        if (target != null  && !monsters.Contains(target) && playerUnit.isAttacking)
        {
            monsters.Add(target);
            // execute ondamage function when monster is in range
            Monster attackTarget = collision.gameObject.GetComponent<Monster>();

            // !! �˹� ����ġ ���
            attackTarget.OnDamage(playerUnit.stat.damage, 5f, (collision.gameObject.transform.position - transform.position).normalized); 
        }

        if (collision.gameObject.tag == "MapObject")
        {
            collision.gameObject.SendMessage("OnDamage");
        }
    }
}
