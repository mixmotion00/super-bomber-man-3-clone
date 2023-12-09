using UnityEngine;

[CreateAssetMenu(fileName = "ExplosionSprite", menuName = "Assets/Explosion Sprite")]
public class ExplosionSprite : ScriptableObject
{
    [SerializeField] private Sprite _base;
    [SerializeField] private Sprite _upMid;
    [SerializeField] private Sprite _upEnd;
    [SerializeField] private Sprite _downMid;
    [SerializeField] private Sprite _downEnd;
    [SerializeField] private Sprite _rightMid;
    [SerializeField] private Sprite _rightEnd;
    [SerializeField] private Sprite _leftMid;
    [SerializeField] private Sprite _leftEnd;

    public Sprite Base { get => _base; }
    public Sprite UpMid { get => _upMid; }
    public Sprite UpEnd { get => _upEnd; }
    public Sprite DownMid { get => _downMid; }
    public Sprite DownEnd { get => _downEnd; }
    public Sprite RightMid { get => _rightMid; }
    public Sprite RightEnd { get => _rightEnd; }
    public Sprite LeftMid { get => _leftMid; }
    public Sprite LeftEnd { get => _leftEnd; }
}