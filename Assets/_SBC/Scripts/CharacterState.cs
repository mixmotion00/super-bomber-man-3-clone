using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SBC;

public interface ICharacterState
{
    int ExplosionPower { get; set; }
    int BombCount { get; set; }
    public bool IsAlive { get; }
    public void OnHitExplosion(Vector2Int explosionPos);
}

public class CharacterState : MonoBehaviour, ICharacterState
{
    private int _health = 1;
    private int _explosionPower = 1;
    private int _bombCount = 1;
    private ICharacterMovement _charMovement;
    private ICharacterAnimation _charAnimation;

    public int ExplosionPower { get => _explosionPower; set => _explosionPower = value; }
    public int BombCount { get => _bombCount; set => _bombCount = value; }
    public bool IsAlive
    {
        get { return _health > 0; }
    }

    private void Start()
    {
        _charMovement = GetComponent<ICharacterMovement>();
        _charAnimation = GetComponent<ICharacterAnimation>();
        WorldObject.Instance.CharacterStates.Add(this);
    }

    private void Update()
    {
        if (_charAnimation.AnimationIsDone("Die", 0.9f))
        {
            // Game Over
            Destroy(gameObject);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<RewardItem>() != null)
        {
            var reward = collision.gameObject.GetComponent<RewardItem>();
            reward.OnInteracted(this);
        }
    }

    public void OnHitExplosion(Vector2Int explosionPos)
    {
        if (!IsAlive) return;

        // #todo: VecCurrentPos() for player aren't reliable

        // check explosion direction
        //var horSign = Mathf.Sign((float)_charMovement.VecCurrentPos().x - (float)explosionPos.x);
        var vecIntPos = new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
        var horSign = Mathf.Sign((float)vecIntPos.x - (float)explosionPos.x);

        //if (_charMovement.VecCurrentPos() == explosionPos)
        if (VecCurPos(horSign) == explosionPos)
        {
            _health -= 1;
            if (_health <= 0)
            {
                OnDead();
            }
        }
    }

    private Vector2Int VecCurPos(float horSign)
    {
        var vecPos = new Vector2(Mathf.FloorToInt(transform.position.x), Mathf.CeilToInt(transform.position.y));

        //    // Error adjustment when moving left
        //    if (_allFaceDir == FaceDir.West || _allFaceDir == FaceDir.East)
        //    {
        //        //vecPos = new Vector2(Mathf.CeilToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        //        vecPos.x = Mathf.CeilToInt(transform.position.x);
        //    }

        //    // Error adjustment when moving down
        //    if (_allFaceDir == FaceDir.South || _allFaceDir == FaceDir.North)
        //    {
        //        vecPos.y = Mathf.CeilToInt(transform.position.y);
        //    }
        //    if (_allFaceDir == FaceDir.North)
        //    {
        //        vecPos.y = Mathf.FloorToInt(transform.position.y);
        //    }

        //if (horSign < 0)
        //{
        //    vecPos.x = Mathf.FloorToInt(transform.position.x);
        //}
        //else
        //{
        //    vecPos.x = Mathf.FloorToInt(transform.position.x);
        //}

        //vecPos.y = Mathf.CeilToInt(transform.position.y);

        return new Vector2Int((int)vecPos.x, (int)vecPos.y);
    }

    private void OnDead()
    {
        Debug.Log($"OnDead Player");
        _charAnimation.PlayAnimation(AnimationType.Die, FaceDir.South);
        WorldObject.Instance.CharacterStates.Remove(this);
    }
}
