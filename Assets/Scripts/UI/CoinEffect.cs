using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinEffect : MonoBehaviour
{
    public TextMeshProUGUI coinText;
    private float aplpha = 1;
    // Start is called before the first frame update
    void Start()
    {
        coinText = this.GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        aplpha -= Time.deltaTime;
        
        coinText.color = new Color(coinText.color.r, coinText.color.g, coinText.color.b, aplpha);
        transform.position += new Vector3(0, 0.1f*Time.deltaTime, 0);
        if (aplpha <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
