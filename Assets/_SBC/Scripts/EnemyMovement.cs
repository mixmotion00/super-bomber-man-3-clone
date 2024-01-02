using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public interface IEnemyMovement
{
    Vector2Int VecCurrentPos();
}

public class EnemyMovement : MonoBehaviour, IEnemyMovement
{
    // Enemy state
    private Vector2 _moveDir = Vector2.up;
    private Vector2 _nextMoveTile = Vector2.zero;
    private float _moveSpeed = 2;

    // Cast grid adjustment
    private Vector2Int upAdj = new Vector2Int(0, 1);
    private Vector2Int downAdj = new Vector2Int(0, -1);
    private Vector2Int leftAdj = new Vector2Int(-1, 0);
    private Vector2Int rightAdj = new Vector2Int(1, 0);

    // Manual References
    [SerializeField] private SpriteRenderer _srDebug;
    [SerializeField] private Transform _debugContainer;

    // Auto References
    private IEnemyBehaviour _enemyBehaviour;

    // Debug
    private bool _showDebug = false; // Only works for 1 enemy during test

    // Properties
    private List<Vector2Int> _allTilesBounds
    {
        // Enemy should also collided with bomb
        get
        {
            if (!WorldObject.Instance) return new List<Vector2Int>();

            var updatedBounds = new List<Vector2Int>();
            var allBounds = WorldObject.Instance.AllTileBounds;
            var cacheBombs = WorldObject.Instance.CacheBombs;

            updatedBounds.AddRange(allBounds);
            cacheBombs.ForEach(bomb =>
            updatedBounds.Add(new Vector2Int((int)bomb.transform.position.x, (int)bomb.transform.position.y)));
            
            return updatedBounds;
        }
    }

    // Mono
    private void Start()
    {
        _enemyBehaviour = GetComponent<IEnemyBehaviour>();
        _nextMoveTile = VecCurrentPos() + _moveDir;
    }

    private void Update()
    {
        if (!_enemyBehaviour.IsAlive) return;
        MovePerTile();
    }

    // Public
    public Vector2Int VecCurrentPos()
    {
        var vecPos = new Vector2(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));

        // Error adjustment when moving left
        if (_moveDir == Vector2.left)
        {
            //vecPos = new Vector2(Mathf.CeilToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
            vecPos.x = Mathf.CeilToInt(transform.position.x);
        }
        // Error adjustment when moving down
        if (_moveDir == Vector2.down)
        {
            vecPos.y = Mathf.CeilToInt(transform.position.y);
        }

        return new Vector2Int((int)vecPos.x, (int)vecPos.y);
    }

    // Private
    private void MovePerTile()
    {
        if(VecCurrentPos() != _nextMoveTile)
        {
            transform.position = Vector2.MoveTowards(transform.position, _nextMoveTile, Time.deltaTime * _moveSpeed);
        }
        else
        {
            // Finished moving to next tile

            ResetDebugCollider();

            //Debug.Log($"Finished. {VecCurrentPos()}, {_nextMoveTile}");
            // Check if can move move

            // Debug
            DebugPos(VecCurrentPos(), upAdj.x, upAdj.y, Color.red);
            DebugPos(VecCurrentPos(), downAdj.x, downAdj.y, Color.yellow);
            DebugPos(VecCurrentPos(), leftAdj.x, leftAdj.y, Color.blue);
            DebugPos(VecCurrentPos(), rightAdj.x, rightAdj.y, Color.green);

            bool needNewDir = RandomDirOnCollided(out Vector2 newMoveDir);

            if (needNewDir)
                _moveDir = newMoveDir;

            //Debug.Break();

            _nextMoveTile = VecCurrentPos() + _moveDir;
        }
    }

    private bool RandomDirOnCollided(out Vector2 randDir)
    {
        //if moving up, should expect collided top only

        List<Vector2Int> allowedDir = new List<Vector2Int>
        {Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        //var allTileBounds = WorldObject.Instance.AllTileBounds;

        var roundPosVer = VecCurrentPos();
        roundPosVer.x += upAdj.x;
        roundPosVer.y += upAdj.y;
        var topCol = _allTilesBounds.Any(tile => roundPosVer == tile);

        roundPosVer = VecCurrentPos();
        roundPosVer.x += downAdj.x;
        roundPosVer.y += downAdj.y;
        var downCol = _allTilesBounds.Any(tile => (roundPosVer) == tile);

        var roundPosHor = VecCurrentPos();
        roundPosHor.x += leftAdj.x;
        roundPosHor.y += leftAdj.y;
        var leftCol = _allTilesBounds.Any(tile => roundPosHor == tile);

        roundPosHor = VecCurrentPos();
        roundPosHor.x += rightAdj.x;
        roundPosHor.y += rightAdj.y;
        var rightCol = _allTilesBounds.Any(tile => tile == roundPosHor);// == tile);

        randDir = Vector2.zero;

        if (_moveDir == Vector2.up)
        {
            if (topCol)
            {
                if (topCol)
                {
                    allowedDir.Remove(Vector2Int.up);
                }
                if (downCol)
                {
                    allowedDir.Remove(Vector2Int.down);
                }
                if (leftCol)
                {
                    allowedDir.Remove(Vector2Int.left);
                }
                if (rightCol)
                {
                    allowedDir.Remove(Vector2Int.right);
                }

                var randPick = Random.Range(0, allowedDir.Count);

                randDir = allowedDir[randPick];
                //Debug.Break();
                if (allowedDir.Any()) return true;
            }
        }
        else if (_moveDir == Vector2.down)
        {
            if(downCol)
            {
                if (downCol)
                {
                    allowedDir.Remove(Vector2Int.down);
                }
                if (topCol)
                {
                    allowedDir.Remove(Vector2Int.up);
                }
                if (leftCol)
                {
                    allowedDir.Remove(Vector2Int.left);
                }
                if (rightCol)
                {
                    allowedDir.Remove(Vector2Int.right);
                }

                randDir = allowedDir[Random.Range(0, allowedDir.Count)];
                if (allowedDir.Any()) return true;
            }
        }
        else if (_moveDir == Vector2.left)
        {
            if(leftCol)
            {
                if (downCol)
                {
                    allowedDir.Remove(Vector2Int.down);
                }
                if (topCol)
                {
                    allowedDir.Remove(Vector2Int.up);
                }
                if (leftCol)
                {
                    allowedDir.Remove(Vector2Int.left);
                }
                if (rightCol)
                {
                    allowedDir.Remove(Vector2Int.right);
                }

                randDir = allowedDir[Random.Range(0, allowedDir.Count)];
                if (allowedDir.Any()) return true;
            }
        }
        else if (_moveDir == Vector2.right)
        {
            if(rightCol)
            {
                if (topCol)
                {
                    allowedDir.Remove(Vector2Int.up);
                }
                if (downCol)
                {
                    allowedDir.Remove(Vector2Int.down);
                }
                if (leftCol)
                {
                    allowedDir.Remove(Vector2Int.left);
                }
                if (rightCol)
                {
                    allowedDir.Remove(Vector2Int.right);
                }

                //Debug.Break();
                randDir = allowedDir[Random.Range(0, allowedDir.Count)];
                if (allowedDir.Any()) return true;
            }
        }

        return false;
    }

    private void DebugPos(Vector2Int checkPos, int xInc, int yInc, Color color)
    {
        //var allTileBounds = WorldObject.Instance.AllTileBounds;

        var newRoundPos = checkPos;
        newRoundPos.x += xInc;
        newRoundPos.y += yInc;
        var check = _allTilesBounds.FirstOrDefault(tile => newRoundPos == tile);

        if (!_allTilesBounds.Any(tile => newRoundPos == tile)) // if no tile found, just don't display debug
            return;

        if(_showDebug)
            DebugCollided(check, color);
    }

    private void DebugCollided(Vector2Int colliderPos, Color color)
    {
        var sr = Instantiate(_srDebug, _debugContainer);
        color.a = 0.5f;
        sr.color = color;
        sr.transform.position = new Vector2(colliderPos.x + 0.5f, colliderPos.y - 0.5f);
    }

    private void ResetDebugCollider()
    {
        foreach (Transform item in _debugContainer)
        {
            Destroy(item.gameObject);
        }
    }
}
