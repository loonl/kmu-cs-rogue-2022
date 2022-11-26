using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffEffect : MonoBehaviour
{
    Animator animator;
    BoxCollider2D boxCollider;

    private void Awake()
    {
        animator = gameObject.GetComponent<Animator>();
        boxCollider = gameObject.GetComponent<BoxCollider2D>();
    }

    private void OnEnable()
    {
        animator.SetTrigger(gameObject.name);
        if (gameObject.name.Contains("Skill"))
        {
            gameObject.transform.localScale = Vector3.one * 1.5f;
            boxCollider.size = new Vector2(0.8f, 0.4f);
            boxCollider.offset = new Vector2(0, 0.3f);
        }
        else
        {
            gameObject.transform.localScale = Vector3.one;
            boxCollider.size = new Vector2(0.3f, 0.3f);
            boxCollider.offset = new Vector2(0, 0.15f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(gameObject.name.Contains("Skill"))
        {
            // �浹�� ����� ����
            if (collision.gameObject.tag == "Monster")
            {
                Monster target = collision.gameObject.GetComponent<Monster>();

                // ������ �ֱ�
                target.OnDamage(GameManager.Instance.Player.stat.damage, GameManager.Instance.Player.stat.knockBackForce,
                    (target.transform.position - transform.position).normalized, GameManager.Instance.Setwfs(10));
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // �浹�� ����� �� ������Ʈ
        if (collision.gameObject.tag == "MapObject")
            collision.gameObject.SendMessage("OnDamage");
    }
}
