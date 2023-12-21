using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyBehaviour
{
    public int Health { get; }
    public bool IsAlive { get; }
    public void OnHitExplosion(Vector2Int explosionPos);
}

public class EnemyBehaviour : MonoBehaviour, IEnemyBehaviour
{
    // Private
    [SerializeField] private Animator _animator;
    [SerializeField] private string _walkAnimStr;
    [SerializeField] private string _dieAnimStr;
    private int _health = 1;

    // Public
    public int Health { get { return _health; } }
    public bool IsAlive
    {
        get { return _health > 0; }
    }
    public IEnemyMovement EnemyMovement { get; private set; }

    private void Start()
    {
        EnemyMovement = GetComponent<IEnemyMovement>();
        WorldObject.Instance.EnemyBehaviours.Add(this);

        _animator.Play(_walkAnimStr);
    }

    private void Update()
    {
        // Destroy enemy game object once finish the animation
        var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

        if(stateInfo.IsName(_dieAnimStr) &&
            stateInfo.normalizedTime >= 1)
        {
            Destroy(gameObject);
        }
    }

    public void OnHitExplosion(Vector2Int explosionPos)
    {
        if (!IsAlive) return;

        if(EnemyMovement.VecCurrentPos() == explosionPos)
        {
            Debug.Log($"OnHitExplosion");
            _health -= 1;
            if(_health <= 0)
            {
                OnDead();
            }
        }
    }

    public void OnDead()
    {
        _animator.Play(_dieAnimStr);
    }
}
