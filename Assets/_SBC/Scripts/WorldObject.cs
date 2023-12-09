using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.Progress;

public class DestructableTile
{
    public SpriteRenderer SR;
    public Vector2 TilePos;
    public Vector2 WorldTilePos;
    public Vector2Int TilePosVInt; //for tilemap reference
}

public class WorldObject : MonoBehaviour
{
    public static WorldObject Instance;

    [SerializeField] private Tilemap _destructableTM;
    [SerializeField] private Tilemap _wallTM;
    [SerializeField] private List<Vector2Int> _allTileBounds = new List<Vector2Int>();
    [SerializeField] private SpriteRenderer srPrefab;
    [SerializeField] private ExplodeEvent _explodeEventPrefab;
    [SerializeField] private RewardItem _rewardItemPrefab;
    private List<DestructableTile> _destructableTiles = new List<DestructableTile>();

    public bool OnCollided(Vector2Int pos)
    {
        var anyTile = _allTileBounds.Any(t => t.x == pos.x && t.y == pos.y);

        var destructableTile = _destructableTiles.
            FirstOrDefault(f => f.TilePos.x == pos.x && f.TilePos.y == pos.y);

        if(destructableTile != null)
        {
            Debug.Log($"Destructable found!, {destructableTile.TilePos}");
            OnDestroyDestructable(destructableTile);
        }

        //Debug.Log($"{pos},{tile}");

        return anyTile;
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GetAllTiles();
    }

    private void OnDestroyDestructable(DestructableTile tile)
    {
        var tileObj = _destructableTM.GetTile((Vector3Int)tile.TilePosVInt);
        if(tileObj != null)
        {
            var explodeEvent = Instantiate(_explodeEventPrefab);
            explodeEvent.transform.position = tile.WorldTilePos;

            var randomItem = Random.Range(0, 2);
            if(randomItem == 0)
            {
                explodeEvent.Init(() => SpawnRewardItem(RewardType.BombCountBonus, tile.WorldTilePos));
            }
            else if(randomItem == 1)
            {
                explodeEvent.Init(() => SpawnRewardItem(RewardType.ExplosionPower, tile.WorldTilePos));
            }
            else
            {
                explodeEvent.Init(null);
            }

            _destructableTM.SetTile((Vector3Int)tile.TilePosVInt, null);
            _allTileBounds.RemoveAll(t => t.x == tile.TilePosVInt.x && t.y == tile.TilePosVInt.y + 1);
            Destroy(tile.SR.gameObject);
        }
    }

    private void SpawnRewardItem(RewardType rewardItemType, Vector2 spawnPos)
    {
        var reward = Instantiate(_rewardItemPrefab);
        reward.transform.position = spawnPos;
        reward.Init(rewardItemType);
    }

    private void GetAllTiles()
    {
        //BoundsInt bounds = _tilemap.cellBounds;

        // Get all tiles within the bounds of the tilemap
        //TileBase[] allTiles = _tilemap.GetTilesBlock(bounds);

        var debugContainer = new GameObject();

        var destructable = GetTilemapBounds(_destructableTM);

        for (int i = 0; i < destructable.Count; i++)
        {
            //add 1 because for unity's tilemap weird tile pos
            var vector2Int = new Vector2Int(destructable[i].x, destructable[i].y);
            destructable[i] = new Vector2Int(destructable[i].x, destructable[i].y + 1);

            //For debugging
            var sr = Instantiate(srPrefab, debugContainer.transform);
            sr.sortingOrder = 10;
            sr.transform.position = new Vector2(destructable[i].x, destructable[i].y);
            var desTile = new DestructableTile();
            desTile.SR = sr;
            desTile.TilePosVInt = vector2Int;
            desTile.TilePos = sr.transform.position;
            var newT = new Vector2(sr.transform.position.x + 0.5f, sr.transform.position.y - 0.5f);
            sr.transform.position = newT;
            sr.color = new Color(0, 1, 0, 0.25f);
            desTile.WorldTilePos = newT;
            _destructableTiles.Add(desTile);
        }

        _allTileBounds.AddRange(destructable);

        var wall = GetTilemapBounds(_wallTM);
        for (int i = 0; i < wall.Count; i++)
        {
            //add 1 because for unity's tilemap weird tile positioning
            wall[i] = new Vector2Int(wall[i].x, wall[i].y + 1);
        }
        _allTileBounds.AddRange(wall);
    }

    private List<Vector2Int> GetTilemapBounds(Tilemap tilemap)
    {
        var tilesPos = new List<Vector2Int>();

        // Ensure that the tilemap is not null
        if (tilemap == null)
        {
            Debug.LogWarning("Tilemap is not assigned.");
        }

        // Iterate through all positions in the tilemap
        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {
            // Check if the tile exists at the current position
            if (tilemap.HasTile(pos))
            {
                tilesPos.Add((Vector2Int)pos);
                Debug.Log($"Pos:{pos.x},{pos.y}");
            }
        }

        tilemap.RefreshAllTiles();

        return tilesPos;
    }
}
