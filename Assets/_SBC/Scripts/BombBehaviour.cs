using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BombBehaviour : MonoBehaviour
{
    ICharacterState _charState;
    [SerializeField] private BombExplosion _bombExplosionPrefab;
    [SerializeField] private BoxCollider2D _boxCol2D;
    [SerializeField] private float _explosionPwr = 1.0f;
    [SerializeField] private float _animationDelay = 1.0f;

    private float _timeToExploded = 2.0f;
    private float _currentTime = 0.0f;
    private bool _doneInit = false;
    private ChainSourceFlag _chainSourceFlag = ChainSourceFlag.None; // direction of the chain's source (if any)

    public void Init(Vector2 original, ref ICharacterState characterState)
    {
        _explosionPwr = characterState.ExplosionPower;
        _charState = characterState;
        _charState.BombCount--;

        _doneInit = true;
    }

    public void ChainExplode(ChainSourceFlag chainSourceFlag)
    {
        _chainSourceFlag = chainSourceFlag;
        _currentTime = _timeToExploded * 0.95f; //is a delay for chain activation (respecting how original works)
    }

    // Update is called once per frame
    void Update()
    {
        if (!_doneInit) return;

        _currentTime += 1 * Time.deltaTime;
        if(_currentTime >= _timeToExploded)
        {
            var explosion = Instantiate(_bombExplosionPrefab);
            explosion.transform.position = transform.position;
            explosion.Init((int)_explosionPwr, _chainSourceFlag);
            _charState.BombCount++;
            WorldObject.Instance.RemoveBomb(this);
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            _boxCol2D.isTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            _boxCol2D.isTrigger = false;
        }
    }
}
