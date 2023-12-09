using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacterBombing
{
    void PlaceBomb(Vector2 position);
}

public class CharacterBombing : MonoBehaviour, ICharacterBombing
{
    private ICharacterState _charState;
    [SerializeField] private BombBehaviour _bombBehaviourPrefab;

    private void Start()
    {
        _charState = GetComponent<ICharacterState>();
    }

    public void PlaceBomb(Vector2 position)
    {
        if (_charState.BombCount <= 0) return;

        var vecPos = new Vector2(Mathf.FloorToInt(position.x), Mathf.RoundToInt(position.y));

        var bomb = Instantiate(_bombBehaviourPrefab);
        bomb.transform.position = vecPos;
        bomb.Init(vecPos, ref _charState);
    }
}
