using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact : MonoBehaviour
{
    private CircleCollider2D _collider;

    private void Start()
    {
        _collider = this.GetComponent<CircleCollider2D>();
    }

    public virtual void InteractEvent()
    {
        // 상호작용 이벤트
        Debug.Log("Interact event not define");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            GameManager.Instance.Player.AddInteractEvent(this);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            GameManager.Instance.Player.RemoveInteractEvent();
        }
    }
}
