using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Staff : MonoBehaviour
{
    private Player player;
    List<Monster> monstersList = new List<Monster>();
    float range = 1.5f;
    private Transform parent;

    [SerializeField]
    private GameObject EffectPrefab;
    private Queue<GameObject> EffectPool = new Queue<GameObject>();

    private void Start()
    {
        player = GameManager.Instance.Player;
        Init();
    }

    void Init()
    {
        //���̶�Ű â�� ����ϰ� ���̵��� ������� �������� parent ����
        GameObject go = Instantiate(Resources.Load<GameObject>("Prefabs/Magic/EffectPrefabs"));
        parent = go.transform;
        // ���� 5�߾� �̸� �ε�
        for (int j = 0; j < 5; j++)
        {
            GameObject obj = Instantiate(EffectPrefab);
            obj.transform.SetParent(parent);
            obj.SetActive(false);
            EffectPool.Enqueue(obj);
        }
    }

    // -------------------------------------------------------------
    // Staff ����
    // -------------------------------------------------------------
    public void Attack(string effectName, bool isattack = true)
    {
        GameObject EffectPrefab = EffectPool.Dequeue();
        EffectPrefab.name = effectName;
        EffectPrefab.SetActive(true);

        EffectPrefab.transform.position = NearestShootDirection(isattack);

        StartCoroutine(ReturnObject(EffectPrefab, isattack));
    }


    // -------------------------------------------------------------
    // ����� ������ �ٽ� Pool �ȿ� �ݳ�
    // -------------------------------------------------------------
    public IEnumerator ReturnObject(GameObject obj, bool isattack = true)
    {
        if(isattack)
            yield return GameManager.Instance.Setwfs(100);
        else
            yield return GameManager.Instance.Setwfs(75);
        obj.SetActive(false);
        EffectPool.Enqueue(obj);
    }

    // -------------------------------------------------------------
    // Player�� �ٶ󺸰� �ִ� ���⿡ �ִ� ���͵� �߿��� ���� ����� ���� ��ȯ
    // -------------------------------------------------------------
    private Vector3 NearestShootDirection(bool isattack = true)
    {
        int roomIndex = DungeonSystem.Instance.Currentroom;

        // ��� ���� List���� ���� ���� Ȯ��
        // ���� ���� MonsterSpawner �ҷ���
        if (DungeonSystem.Instance.monsterSpawners.TryGetValue(roomIndex, out MonsterSpawner spawner))
        {
            foreach (Monster monster in spawner.allMonsters)
            {
                // �ܳ� ���� ����1 : ���Ͱ� ��������� ���� ���� �־�� ��
                if (!monster.isDead && Vector3.Distance(monster.transform.position, player.transform.position) < range)
                {
                    // Player�� ������ ���� �ִٸ� 
                    if (player.transform.localScale.x > 0)
                    {
                        // �ܳ� ���� ����2 : ���Ͱ� Player ���ʿ� ����
                        if (monster.transform.position.x < player.transform.position.x)
                            monstersList.Add(monster);
                    }
                    else // Player�� �������� ���� �ִٸ�
                    {
                        // �ܳ� ���� ����2 : ���Ͱ� Player �����ʿ� ����
                        if (monster.transform.position.x > player.transform.position.x)
                            monstersList.Add(monster);
                    }
                }
            }
        }
        
        // �ܳ� ������ Monster�� ���ٸ� Player�� ���� ������ range�Ÿ��� ��ȯ
        if (monstersList.Count == 0)
            return player.transform.position + new Vector3(-player.transform.localScale.x, 0, 0).normalized * range;

        // ������ �����ϴ� Monster�� �߿��� ���� ����� Monster �Ǻ�
        Vector3 returnVal = monstersList[0].transform.position - player.transform.position;
        float mini = returnVal.magnitude;
        int listnum = 0;
        for (int i = 1; i < monstersList.Count; i++)
        {
            Vector3 direction = monstersList[i].transform.position - player.transform.position;
            if (direction.magnitude < mini)
            {
                mini = direction.magnitude;
                returnVal = direction;
                listnum = i;
            }
        }
        if(isattack)
            monstersList[listnum].OnDamage(player.stat.damage, player.stat.knockBackForce,
                (monstersList[listnum].transform.position - transform.position).normalized, GameManager.Instance.Setwfs(10));
        monstersList.Clear();
        //������ ��ġ ��ȯ
        return returnVal + player.transform.position;
    }
}
