using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowGenerate : MonoBehaviour
{
    private Player player;
    [SerializeField]
    private Transform parent;
    
    [SerializeField]
    private List<GameObject> arrowPrefabs;
    
    // test - 오브젝트 풀링
    private List<Queue<GameObject>> arrowPool;
    private int index;

    private void Start()
    {
        player = gameObject.GetComponent<Player>();
        parent = GameObject.Find("ArrowPrefabs").transform;
        
        ArrowPoolInit();
    }
    
    /*
     * 초기 설정 코드
     */

    // 오브젝트 풀링 첫 작업
    private void ArrowPoolInit()
    {
        arrowPool = new List<Queue<GameObject>>();
        for (int i = 0; i < arrowPrefabs.Count; i++)
        {
            Queue<GameObject> temp = new Queue<GameObject>();
            
            // 화살 2발씩 미리 로딩
            for (int j = 0; j < 2; j++)
            {
                GameObject obj = Instantiate(arrowPrefabs[i]);
                obj.SetActive(false);
                obj.transform.SetParent(parent);
                temp.Enqueue(obj);
            }
            
            arrowPool.Add(temp);
        }
    }
    
    /*
     * 다른 스크립트에서 호출되는 함수
     */
    
    // Arrow 공격
    public void Attack(string effectName)
    {
        // 사용할 화살 이펙트 prefab index 결정
        switch (effectName)
        {
            case "NormalArrow":
                index = 0;
                break;
        }
        
        GameObject arrowPrefab = arrowPool[index].Dequeue();
        arrowPrefab.SetActive(true);
        
        // 화살 스폰 위치 조정
        arrowPrefab.transform.position = player.transform.position;

        // 화살 발사
        // TODO - 자동 에임 보정 기능
        if (player.moveInfo != Vector3.zero)
            arrowPrefab.GetComponent<Rigidbody2D>().velocity = player.moveInfo * 15f;
        else
            arrowPrefab.GetComponent<Rigidbody2D>().velocity = new Vector2(-player.transform.localScale.x, 0) / 7 * 150f;
    }
    
    // 사용이 끝나고 다시 Pool 안에 반환
    public void ReturnObject(GameObject obj)
    {
        obj.gameObject.SetActive(false);
        arrowPool[index].Enqueue(obj);
    }
    
}
