using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ExplosionSpriteFlag
{
    Base,
    UpMid,
    UpEnd,
    DownMid,
    DownEnd,
    LeftMid,
    LeftEnd,
    RightMid,
    RightEnd
}

public enum ChainSourceFlag
{
    None,
    FromTop,
    FromBottom,
    FromLeft,
    FromRight
}

public class SpriteCache
{
    public SpriteRenderer Sr;
    public ExplosionSpriteFlag Flag;

    public SpriteCache(SpriteRenderer sr, ExplosionSpriteFlag flag)
    {
        Sr = sr;
        Flag = flag;
    }
}

public class BombExplosion : MonoBehaviour
{
    [SerializeField] private int _topSpread = 0;
    [SerializeField] private int _bottomSpread = 0;
    [SerializeField] private int _leftSpread = 0;
    [SerializeField] private int _rightSpread = 0;

    [SerializeField] private SpriteRenderer _srPrefab;
    [SerializeField] private List<ExplosionSprite> _explosionSprites = new List<ExplosionSprite>();
    private List<SpriteCache> _cacheSprites = new List<SpriteCache>();

    private float _currentTime = 0.0f;
    private float _delayTime = 0.2f;
    private int _animationIndex = 0;
    private int _maxIndex { get { return _explosionSprites.Count; } }

    [SerializeField] private bool _doneInit = false;

    public void Init(int power, ChainSourceFlag chainSourceFlag)
    {
        if (power <= 0) power = 1; //min 1

        _topSpread = power;
        _bottomSpread = power;
        _leftSpread = power;
        _rightSpread = power;

        CreateExplosionSpread(chainSourceFlag);
        _doneInit = true;
    }

    private void Update()
    {
        if(_animationIndex >= _maxIndex)
        {
            foreach (var sprite in _cacheSprites)
            {
                Destroy(sprite.Sr.gameObject);
            }

            Destroy(gameObject);

            return;
        }

        if (!_doneInit) return;
        PlayExplosionSequences();
    }

    [ContextMenu("CreateExplosionSpread")]
    private void CreateExplosionSpread(ChainSourceFlag chainSourceFlag)
    {
        Vector2 origin = transform.position;

        var spreadPos = origin;
        //base
        InstantiateSpread(ref spreadPos, Vector2.zero, ExplosionSpriteFlag.Base);
        var oriSpreadPosInt = new Vector2Int((int)spreadPos.x, (int)spreadPos.y);
        WorldObject.Instance.OnExplosionCollided(oriSpreadPosInt);

        bool collided = false;
        //top
        for (int i = 0; i < _topSpread; i++)
        {
            if (collided) continue;

            if (chainSourceFlag == ChainSourceFlag.FromBottom) continue;

            //check if any collider
            Vector2Int spreadPosInt = new Vector2Int((int)spreadPos.x, (int)spreadPos.y) + Vector2Int.up;
            if (WorldObject.Instance.OnCollidedBomb(spreadPosInt, Vector2.zero))
            {
                Debug.Log($"Collided bomb to the top");
                collided = true;
                //Activate chain explosion
                WorldObject.Instance.OnCollidedBomb(spreadPosInt, Vector2.zero).ChainExplode(ChainSourceFlag.FromTop);
            }

            if (WorldObject.Instance.OnExplosionCollided(spreadPosInt))
            {
                Debug.Log($"Collided top: {spreadPosInt}");
                collided = true;
            }
            if (!collided)
            {
                if(i == _topSpread - 1)
                {
                    // non-blocked, play till end of explosion's tail
                    InstantiateSpread(ref spreadPos, Vector2.up, ExplosionSpriteFlag.UpEnd);
                }
                else
                {
                    InstantiateSpread(ref spreadPos, Vector2.up, ExplosionSpriteFlag.UpMid);
                }
            }
        }
        spreadPos = origin;
        collided = false;
        //down
        for (int i = 0; i < _bottomSpread; i++)
        {
            if (collided) continue;

            if (chainSourceFlag == ChainSourceFlag.FromTop) continue;

            Vector2Int spreadPosInt = new Vector2Int((int)spreadPos.x, (int)spreadPos.y) + Vector2Int.down;
            if (WorldObject.Instance.OnCollidedBomb(spreadPosInt, Vector2.zero))
            {
                Debug.Log($"Collided bomb to the down");
                collided = true;
                WorldObject.Instance.OnCollidedBomb(spreadPosInt, Vector2.zero).ChainExplode(ChainSourceFlag.FromBottom);
            }
            if (WorldObject.Instance.OnExplosionCollided(spreadPosInt))
            {
                Debug.Log($"Collided down: {spreadPosInt}");
                collided = true;
            }
            if (!collided)
            {
                if (i == _topSpread - 1)
                {
                    // non-blocked, play till end of explosion's tail
                    InstantiateSpread(ref spreadPos, Vector2.down, ExplosionSpriteFlag.DownEnd);
                }
                else
                {
                    InstantiateSpread(ref spreadPos, Vector2.down, ExplosionSpriteFlag.DownMid);
                }
            }
        }
        spreadPos = origin;
        collided = false;
        //left
        for (int i = 0; i < _leftSpread; i++)
        {
            if (collided) continue;

            if (chainSourceFlag == ChainSourceFlag.FromRight) continue;

            Vector2Int spreadPosInt = new Vector2Int((int)spreadPos.x, (int)spreadPos.y) + Vector2Int.left;
            if (WorldObject.Instance.OnCollidedBomb(spreadPosInt, Vector2.zero))
            {
                Debug.Log($"Collided bomb to the left");
                collided = true;
                WorldObject.Instance.OnCollidedBomb(spreadPosInt, Vector2.zero).ChainExplode(ChainSourceFlag.FromLeft);
            }
            if (WorldObject.Instance.OnExplosionCollided(spreadPosInt))
            {
                Debug.Log($"Collided left: {spreadPosInt}");
                collided = true;
            }
            if (!collided)
            {
                if (i == _topSpread - 1)
                {
                    // non-blocked, play till end of explosion's tail
                    InstantiateSpread(ref spreadPos, Vector2.left, ExplosionSpriteFlag.LeftEnd);
                }
                else
                {
                    InstantiateSpread(ref spreadPos, Vector2.left, ExplosionSpriteFlag.LeftMid);
                }
            }
        }
        spreadPos = origin;
        collided = false;
        //right
        for (int i = 0; i < _rightSpread; i++)
        {
            if (collided) continue;

            if (chainSourceFlag == ChainSourceFlag.FromLeft) continue;

            Vector2Int spreadPosInt = new Vector2Int((int)spreadPos.x, (int)spreadPos.y) + Vector2Int.right;
            if (WorldObject.Instance.OnCollidedBomb(spreadPosInt, Vector2.zero))
            {
                Debug.Log($"Collided bomb to the right");
                collided = true;
                WorldObject.Instance.OnCollidedBomb(spreadPosInt, Vector2.zero).ChainExplode(ChainSourceFlag.FromRight);
            }
            if (WorldObject.Instance.OnExplosionCollided(spreadPosInt))
            {
                Debug.Log($"Collided right: {spreadPosInt}");
                collided = true;
            }
            if (!collided)
            {
                if (i == _topSpread - 1)
                {
                    // non-blocked, play till end of explosion's tail
                    InstantiateSpread(ref spreadPos, Vector2.right, ExplosionSpriteFlag.RightEnd);
                }
                else
                {
                    InstantiateSpread(ref spreadPos, Vector2.right, ExplosionSpriteFlag.RightMid);
                }
            }
            
        }

        PlayExplosionSprite();
    }

    private void InstantiateSpread(ref Vector2 spreadPos, Vector2 dir, ExplosionSpriteFlag flag)
    {
        spreadPos += dir;
        var newSpread = Instantiate(_srPrefab);
        newSpread.transform.position = new Vector2(spreadPos.x + 0.5f, spreadPos.y - 0.5f);

        SpriteCache spriteCache = new SpriteCache(newSpread, flag);
        _cacheSprites.Add(spriteCache);
    }

    private void PlayExplosionSequences()
    {
        _currentTime += 1 * Time.deltaTime;

        if(_currentTime >= _delayTime)
        {
            PlayExplosionSprite();
            _animationIndex++;
            _currentTime = 0;
        }
    }

    private void PlayExplosionSprite()
    {
        foreach (var sr in _cacheSprites)
        {
            switch (sr.Flag)
            {
                case ExplosionSpriteFlag.Base:
                    sr.Sr.sprite = _explosionSprites[_animationIndex].Base;
                    break;
                case ExplosionSpriteFlag.UpMid:
                    sr.Sr.sprite = _explosionSprites[_animationIndex].UpMid;
                    break;
                case ExplosionSpriteFlag.UpEnd:
                    sr.Sr.sprite = _explosionSprites[_animationIndex].UpEnd;
                    break;
                case ExplosionSpriteFlag.DownMid:
                    sr.Sr.sprite = _explosionSprites[_animationIndex].DownMid;
                    break;
                case ExplosionSpriteFlag.DownEnd:
                    sr.Sr.sprite = _explosionSprites[_animationIndex].DownEnd;
                    break;
                case ExplosionSpriteFlag.LeftMid:
                    sr.Sr.sprite = _explosionSprites[_animationIndex].LeftMid;
                    break;
                case ExplosionSpriteFlag.LeftEnd:
                    sr.Sr.sprite = _explosionSprites[_animationIndex].LeftEnd;
                    break;
                case ExplosionSpriteFlag.RightMid:
                    sr.Sr.sprite = _explosionSprites[_animationIndex].RightMid;
                    break;
                case ExplosionSpriteFlag.RightEnd:
                    sr.Sr.sprite = _explosionSprites[_animationIndex].RightEnd;
                    break;
                default:
                    Debug.LogError($"Undefined Flag");
                    break;
            }
        }
    }
}
