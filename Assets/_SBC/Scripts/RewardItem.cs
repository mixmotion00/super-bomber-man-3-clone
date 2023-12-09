using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RewardType
{
    BombCountBonus,
    ExplosionPower,
}

public class RewardItem : MonoBehaviour
{
    [SerializeField] private RewardType _rewardType;
    [SerializeField] private RewardItemSprite _rewardItemSprite;
    [SerializeField] private SpriteRenderer _sr;

    public void Init(RewardType rewardType)
    {
        switch (rewardType)
        {
            case RewardType.BombCountBonus:
                _sr.sprite = _rewardItemSprite.BombBonus;
                break;
            case RewardType.ExplosionPower:
                _sr.sprite = _rewardItemSprite.ExplosionBonus;
                break;
            default:
                break;
        }

        _rewardType = rewardType;

        WorldObject.Instance.AddRewardItem(this);
    }

    public void OnInteracted(ICharacterState charState)
    {
        switch (_rewardType)
        {
            case RewardType.BombCountBonus:
                charState.BombCount++;
                break;
            case RewardType.ExplosionPower:
                charState.ExplosionPower++;
                break;
            default:
                break;
        }

        WorldObject.Instance.RemoveRewardItem(this, false);

        Destroy(gameObject);
    }
}
