using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Player player;
    private void Start()
    {
        player = GameManager.Instance.Player;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 충돌한 대상이 몬스터
        if (collision.gameObject.tag == "Monster")
        {
            Monster target = collision.gameObject.GetComponent<Monster>();

            // 데미지 주기
            target.OnDamage(player.stat.damage, player.stat.knockBackForce,
                (target.transform.position - transform.position).normalized, GameManager.Instance.Setwfs(10));
        }

        // 충돌한 대상이 맵 오브젝트
        else if (collision.gameObject.tag == "MapObject")
            collision.gameObject.SendMessage("OnDamage");

        GameManager.Instance.Player.gameObject.GetComponent<ArrowGenerate>().ReturnObject(gameObject);
    }
}
