using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RewardItemSprite", menuName = "Assets/Reward Item Sprite")]
public class RewardItemSprite : ScriptableObject
{
    [SerializeField] private Sprite _bombBonus;
    [SerializeField] private Sprite _explosionBonus;

    public Sprite BombBonus { get => _bombBonus; }
    public Sprite ExplosionBonus { get => _explosionBonus; }
}
