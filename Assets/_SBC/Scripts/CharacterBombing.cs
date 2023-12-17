using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Progress;

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

        var allExistingBomb = WorldObject.Instance.CacheBombs;

        Debug.Log($"allExistingBomb.Count: {allExistingBomb.Count}");

        foreach (var item in allExistingBomb)
        {
            Debug.Log($"Bombpos: {item.transform.position}, desired_pos: {position}");
        }

        var vecPos = new Vector2(Mathf.FloorToInt(position.x), Mathf.RoundToInt(position.y));

        //prevent placing bomb at the same vec world tile location
        if (allExistingBomb.Any(b => (Vector2)b.transform.position == vecPos)) return;

        var bomb = Instantiate(_bombBehaviourPrefab);
        bomb.transform.position = vecPos;
        bomb.Init(vecPos, ref _charState);

        WorldObject.Instance.AddBomb(bomb);
    }

    //public bool BombAlreadyExistHere(Vector2 pos)
    //{
    //    var allExistingBomb = WorldObject.Instance.CacheBombs;
    //    bool exist = false;
    //    allExistingBomb.Any(b => (b.transform.x + 0.5f) == position)
    //    foreach (var item in allExistingBomb)
    //    {

    //    }
    //}
}
