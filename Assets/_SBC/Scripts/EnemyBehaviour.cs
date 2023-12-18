using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyBehaviour
{
    public int Health { get; }
    public void OnHitExplosion(Vector2Int explosionPos);
}

public class EnemyBehaviour : MonoBehaviour, IEnemyBehaviour
{
    // Private
    private int _health;

    // Public
    public int Health { get { return _health; } }
    public IEnemyMovement EnemyMovement { get; private set; }

    private void Start()
    {
        EnemyMovement = GetComponent<IEnemyMovement>();
        WorldObject.Instance.EnemyBehaviours.Add(this);

    }

    public void OnHitExplosion(Vector2Int explosionPos)
    {
        if(EnemyMovement.VecCurrentPos() == explosionPos)
        {
            Debug.Log($"OnHitExplosion");
        }
    }
}
