using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyMovement : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Vector2 _moveDir = Vector2.right;
    private float _moveSpeed = 50;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if(RandomDirOnCollided() != Vector2.zero)
        {
            _moveDir = RandomDirOnCollided();
        }
    }

    private void FixedUpdate()
    {
        _rb.velocity = _moveDir * _moveSpeed * Time.fixedDeltaTime;
    }

    private Vector2 RandomDirOnCollided()
    {
        List<Vector2Int> allowedDir = new List<Vector2Int>();
        //var allTileBounds = WorldObject.Instance.AllTileBounds;

        foreach (var tileBound in WorldObject.Instance.AllTileBounds)
        {
            if ((VecCurrentPos()) == tileBound && _moveDir != Vector2.right)
            {
                //move right
                allowedDir.Add(Vector2Int.right);
            }
            //check right bound
            if ((VecCurrentPos() + Vector2Int.right) == tileBound && _moveDir != Vector2.left)
            {
                //move left
                allowedDir.Add(Vector2Int.left);
            }

            ////check upper bound
            //if((VecCurrentPos() + Vector2Int.up) == tileBound && _moveDir != Vector2.down)
            //{
            //    //move down
            //    allowedDir.Add(Vector2Int.down);
            //}
            ////check down bound
            //else if ((VecCurrentPos() + Vector2Int.down) == tileBound && _moveDir != Vector2.up)
            //{
            //    //move up
            //    allowedDir.Add(Vector2Int.up);
            //}
            ////check left bound
            //if ((VecCurrentPos() + Vector2Int.left) == tileBound && _moveDir != Vector2.right)
            //{
            //    //move right
            //    allowedDir.Add(Vector2Int.right);
            //}
            ////check right bound
            //else if ((VecCurrentPos() + Vector2Int.right) == tileBound && _moveDir != Vector2.left)
            //{
            //    //move left
            //    allowedDir.Add(Vector2Int.left);
            //}
        }

        if (allowedDir.Count > 0)
        {
            var rand = allowedDir[Random.Range(0, allowedDir.Count)];
            return rand;
        }
        else
        {
            return Vector2.zero;
        }
    }

    private Vector2Int VecCurrentPos()
    {
        var vecPos = new Vector2(Mathf.FloorToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        return new Vector2Int((int)vecPos.x, (int)vecPos.y);
    }
}
