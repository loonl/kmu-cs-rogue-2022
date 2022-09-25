using UnityEngine;

public class FastZombie : Monster
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (dead)
        {
            return;
        }

        // 목표가 가까워지면 이동속도 증가
        Player attackTarget = other.gameObject.GetComponent<Player>();
        if (player == attackTarget)
        {
            speed += 0.75f;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (dead)
        {
            return;
        }

        // 목표 멀어지면 이동속도 감소
        Player attackTarget = other.gameObject.GetComponent<Player>();
        if (player == attackTarget)
        {
            speed -= 0.75f;
        }
    }
}
