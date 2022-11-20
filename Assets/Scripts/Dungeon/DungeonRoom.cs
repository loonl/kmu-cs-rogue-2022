using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonRoom : MonoBehaviour
{
    public GameObject TileMapParent;

    [SerializeField]
    private Tilemap _groundLayer;
    [SerializeField]
    private Tilemap _wallLayer;
    [SerializeField]
    private Tilemap _objectLayer;
    [SerializeField]
    private Tilemap _closeDoorLayer;
    [SerializeField]
    private Tilemap _openDoorLayer;
    [SerializeField]
    public GameObject ObjectParent;
    [SerializeField]
    public GameObject ObstacleParent;

    public Tilemap GroundLayer { get { return _groundLayer; } }
    public Tilemap WallLayer { get { return _wallLayer; } }
    public Tilemap ObjectLayer { get { return _objectLayer; } }
    public Tilemap CloseDoorLayer { get { return _closeDoorLayer; } }
    public Tilemap OpenDoorLayer { get { return _openDoorLayer; } }

    public Portal[] Portals = new Portal[] { null, null, null, null };

    private MonsterSpawner _spawner = null;

    //전투방인지 아닌지 체크를 위한 용도
    public bool IsClear = false;

    public void Enter(ushort outDirect, GameObject player)
    {
        // 플레이어 입장
        Portals[(outDirect + 2) % 4].Enter(player);
        
        if (!IsClear)
        {
            _spawner.Spawn();
            SoundManager.Instance.SoundPlay(SoundType.DoorClose);
        }
    }

    public void Clear()
    {
        // 방 클리어
        IsClear = true;
        foreach (Portal portal in Portals)
        {
            if (portal != null)
            {
                portal.Activate();
            }
        }

        CloseDoorLayer.gameObject.SetActive(false);
        OpenDoorLayer.gameObject.SetActive(true);
    }

    public void SetSpawner(MonsterSpawner spawner, int roomIndex, List<Dictionary<string, object>> monsterData)
    {
        _spawner = spawner;
        float horizontalRange = (float)(this.WallLayer.size.x - 2) * 0.5f;
        float verticalRange = (float)(this.WallLayer.size.y - 2) * 0.5f;

        _spawner.Set(roomIndex, this.transform.position, horizontalRange, verticalRange, monsterData);
    }

    //테스트용 코드
    public void KillAll()
    {
        int cnt = _spawner.aliveMonsters.Count;
        if (_spawner != null)
            for(int i = 0; i < cnt; i++)
                _spawner.aliveMonsters[0].Die();
    }
}
