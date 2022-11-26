using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffEffect : MonoBehaviour
{
    Animator animator;

    private void Awake()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    private void OnEnable()
    {
        animator.SetTrigger(gameObject.name);
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
