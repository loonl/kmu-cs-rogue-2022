using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    private List<Dictionary<string, object>> monsterData; // 몬스터 데이터
    public List<Monster> SetupMonsters = new List<Monster>(); // 생성할 몬스터들
    public List<Monster> monsters = new List<Monster>(); // 생존한 몬스터들
    public List<Monster> deadMonsters = new List<Monster>(); // 죽은 몬스터들

    private int roomIndex; // 방 번호
    private Vector3 roomPosition; // 방 위치
    private float horizontalRange;
    private float verticalRange;

    // 스포너 초기화
    public void Set(int roomIndex, Vector3 roomPosition, float horizontalRange, float verticalRange, List<Dictionary<string, object>> monsterData)
    {
        this.roomIndex = roomIndex;
        this.roomPosition = roomPosition;
        this.horizontalRange = horizontalRange;
        this.verticalRange = verticalRange;
        this.monsterData = monsterData;
    }

    // 몬스터 스폰
    public void Spawn()
    {
        for (int i = 0; i < SetupMonsters.Count; i++)
        {
            Monster monster = Instantiate(SetupMonsters[i]);
            monster.monsterData = monsterData;
            monster.transform.SetParent(gameObject.transform);
            Vector3 diff = new Vector3(
                Random.Range(- 0.8f * horizontalRange, 0.8f * horizontalRange),
                Random.Range(-0.8f * verticalRange, 0.8f * verticalRange),
                0f
            );
            monster.transform.position = roomPosition + diff;

            monsters.Add(monster);
        }
    }

    // 방에 남은 몬스터가 있는지 확인
    public void CheckRemainEnemy()
    {
        if (monsters.Count < 1)
        {
            DungeonSystem.Instance.Rooms[roomIndex].Clear();
            SoundManager.Instance.SoundPlay(SoundType.DoorOpen);
        }
    }
}