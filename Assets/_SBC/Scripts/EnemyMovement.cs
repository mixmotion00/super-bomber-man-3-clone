using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyMovement : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Vector2Int _collidedUp, _collidedDown, _collidedLeft, _collidedRight;
    private bool _readyNextCol = false;
    private Vector2 _moveDir = Vector2.right;
    private Vector2 _nextMoveTile = Vector2.zero;
    private float _moveSpeed = 2;
    [SerializeField] private SpriteRenderer _srDebug;
    [SerializeField] private Transform _debugContainer;

    private Vector2Int upAdj = new Vector2Int(0, 1);
    private Vector2Int downAdj = new Vector2Int(0, -1);
    private Vector2Int leftAdj = new Vector2Int(-1, 0);
    private Vector2Int rightAdj = new Vector2Int(1, 0);

    [SerializeField] private bool start = false;
    //[SerializeField] private bool continueMove = true;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _nextMoveTile = VecCurrentPos() + new Vector2(1, 0);
    }

    private void Update()
    {
        //var anyDir = RandomDirOnCollided(out Vector2 randDir);

        //if (anyDir)
        //{
        //    Debug.Log($"Update() randomdir={randDir}");
        //    _moveDir = randDir;
        //}
        if (!start) return;
        MovePerTile();
    }

    private void FixedUpdate()
    {
        //_rb.velocity = _moveDir * _moveSpeed * Time.fixedDeltaTime;
    }

    private void MovePerTile()
    {
        if(VecCurrentPos() != _nextMoveTile)
        {
            transform.position = Vector2.MoveTowards(transform.position, _nextMoveTile, Time.deltaTime * _moveSpeed);
        }
        else
        {
            ResetDebugCollider();

            Debug.Log($"Finished. {VecCurrentPos()}, {_nextMoveTile}");
            // Check if can move move
            DebugPos(VecCurrentPos(), upAdj.x, upAdj.y, Color.red);
            DebugPos(VecCurrentPos(), downAdj.x, downAdj.y, Color.yellow);
            DebugPos(VecCurrentPos(), leftAdj.x, leftAdj.y, Color.blue);
            DebugPos(VecCurrentPos(), rightAdj.x, rightAdj.y, Color.green);

            Debug.Break();

            _nextMoveTile = VecCurrentPos() + Vector2Int.right;
        }

        // Once finished move to next tile
        // Check moveable dir
        // Get next tile based on random dir
    }

    private void GetNextTile(Vector2Int dir)
    {
        _nextMoveTile = VecCurrentPos() + dir;
    }

    private bool RandomDirOnCollided(out Vector2 randDir)
    {
        //if moving up, should expect collided top only

        List<Vector2Int> allowedDir = new List<Vector2Int>
        {Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        var allTileBounds = WorldObject.Instance.AllTileBounds;

        var roundPosVer = VecCurrentPos();
        roundPosVer.x += upAdj.x;
        roundPosVer.y += upAdj.y;
        var topCol = allTileBounds.Any(tile => roundPosVer == tile);

        roundPosVer = VecCurrentPos();
        roundPosVer.x += downAdj.x;
        roundPosVer.y += downAdj.y;
        var downCol = allTileBounds.Any(tile => (roundPosVer) == tile);

        var roundPosHor = VecCurrentPos();
        roundPosHor.x += leftAdj.x;
        roundPosHor.y += leftAdj.y;
        var leftCol = allTileBounds.Any(tile => roundPosHor == tile);

        roundPosHor = VecCurrentPos();
        roundPosHor.x += rightAdj.x;
        roundPosHor.y += rightAdj.y;
        var rightCol = allTileBounds.Any(tile => tile == roundPosHor);// == tile);

        randDir = Vector2.zero;

        if (_moveDir == Vector2.up)
        {
            if (topCol)
            {
                if (topCol)
                {
                    DebugPos(VecCurrentPos(), upAdj.x, upAdj.y, Color.red);
                    allowedDir.Remove(Vector2Int.up);
                }
                if (downCol)
                {
                    DebugPos(VecCurrentPos(), downAdj.x, downAdj.y, Color.yellow);

                    allowedDir.Remove(Vector2Int.down);
                }
                if (leftCol)
                {
                    DebugPos(VecCurrentPos(), leftAdj.x, leftAdj.y, Color.blue);
                    allowedDir.Remove(Vector2Int.left);
                }
                if (rightCol)
                {
                    DebugPos(VecCurrentPos(), rightAdj.x, rightAdj.y, Color.green);
                    allowedDir.Remove(Vector2Int.right);
                }

                //string debugStr = "";
                //allowedDir.ForEach(a => debugStr += $"[{a}],");
                //Debug.Log(debugStr);
                var randPick = Random.Range(0, allowedDir.Count);
                Debug.Log($"RandPick: {randPick}, Count:{allowedDir.Count}");
                if(allowedDir.Count == 0)
                {
                    Debug.Log($"Something is wrong.");
                    Debug.Break();
                }

                randDir = allowedDir[randPick];
                Debug.Break();
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

                string debugStr = "";
                allowedDir.ForEach(a => debugStr += $"[{a}],");
                Debug.Log(debugStr);

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
                    roundPosVer = VecCurrentPos();
                    roundPosVer.x += 1;
                    roundPosVer.y -= 1;
                    var checkDown = allTileBounds.FirstOrDefault(tile => roundPosVer == tile);
                    Debug.Log($"Checkdown: {checkDown}");
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

                string debugStr = "";
                allowedDir.ForEach(a => debugStr += $"[{a}],");
                Debug.Log(debugStr);

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
                    DebugPos(VecCurrentPos(), upAdj.x, upAdj.y, Color.red);
                    allowedDir.Remove(Vector2Int.up);
                }
                if (downCol)
                {
                    DebugPos(VecCurrentPos(), downAdj.x, downAdj.y, Color.yellow);
                    allowedDir.Remove(Vector2Int.down);
                }
                if (leftCol)
                {
                    DebugPos(VecCurrentPos(), leftAdj.x, leftAdj.y, Color.blue);
                    allowedDir.Remove(Vector2Int.left);
                }
                if (rightCol)
                {
                    DebugPos(VecCurrentPos(), rightAdj.x, rightAdj.y, Color.green);
                    allowedDir.Remove(Vector2Int.right);
                }

                //string debugStr = "";
                //allowedDir.ForEach(a => debugStr += $"[{a}],");
                //Debug.Log(debugStr);
                Debug.Break();
                randDir = allowedDir[Random.Range(0, allowedDir.Count)];
                if (allowedDir.Any()) return true;
            }
        }

        return false;
    }

    private void DebugPos(Vector2Int checkPos, int xInc, int yInc, Color color)
    {
        var allTileBounds = WorldObject.Instance.AllTileBounds;

        var newRoundPos = checkPos;
        newRoundPos.x += xInc;
        newRoundPos.y += yInc;
        var check = allTileBounds.FirstOrDefault(tile => newRoundPos == tile);

        if (!allTileBounds.Any(tile => newRoundPos == tile)) // if no tile found, just don't display debug
            return;

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

    private Vector2Int VecCurrentPos()
    {
        var vecPos = new Vector2(Mathf.FloorToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        return new Vector2Int((int)vecPos.x, (int)vecPos.y);
    }
}
