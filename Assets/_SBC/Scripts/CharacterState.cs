using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacterState
{
    int ExplosionPower { get; set; }
    int BombCount { get; set; }
}

public class CharacterState : MonoBehaviour, ICharacterState
{
    private int _explosionPower = 1;
    private int _bombCount = 1;

    public int ExplosionPower { get => _explosionPower; set => _explosionPower = value; }
    public int BombCount { get => _bombCount; set => _bombCount = value; }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<RewardItem>() != null)
        {
            var reward = collision.gameObject.GetComponent<RewardItem>();
            reward.OnInteracted(this);
        }
    }
}
