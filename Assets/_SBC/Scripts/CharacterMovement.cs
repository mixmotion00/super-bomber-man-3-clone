using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SBC
{
    public interface ICharacterMovement 
    {
        bool AnyMovement { get; }
        FaceDir HorizontalFaceDir { get; }
        FaceDir AllFaceDir { get; }
        void Move(Vector2 direction);
        //Vector2Int VecCurrentPos();
    }

    public enum FaceDir : int
    {
        North = 0,
        South = 180,
        East = 270,
        West = 90
    }

    [RequireComponent(typeof(Rigidbody2D))]
    public class CharacterMovement : MonoBehaviour, ICharacterMovement
    {
        private float _moveSpeed = 190;
        private FaceDir _allFaceDir = FaceDir.East; // Default facing right dir
        private FaceDir _horizontalFaceDir = FaceDir.East;
        private Rigidbody2D _rb;

        //Interface references
        private ICharacterAnimation _characterAnimation;
        private ICharacterVisual _characterVisual;
        private ICharacterState _characterState;

        public bool AnyMovement 
        {
            get 
            {
                return _rb.velocity.x != 0.0f || _rb.velocity.y != 0.0f;
            }
        }

        public FaceDir HorizontalFaceDir { get => _horizontalFaceDir; }
        public FaceDir AllFaceDir { get => _allFaceDir; }

        // Start is called before the first frame update
        void Start()
        {
            _characterAnimation = GetComponent<CharacterAnimation>();
            _characterVisual = GetComponent<CharacterVisual>();
            _characterState = GetComponent<ICharacterState>();
            _rb = GetComponent<Rigidbody2D>();
        }

        public void Move(Vector2 direction) 
        {
            if (!_characterState.IsAlive)
            {
                _moveSpeed = 0;
                _rb.velocity = Vector2.zero;
                return;
            }

            direction.x = _moveSpeed * direction.x;
            direction.y = _moveSpeed * direction.y;

            Vector2 velocity = new Vector2(direction.x, direction.y) * Time.smoothDeltaTime;

            _rb.velocity = velocity;

            DetermineFaceDir(velocity);

            if (_rb.velocity.x == 0.0f && _rb.velocity.y == 0.0f)
            {
                _characterAnimation.PlayAnimation(AnimationType.Idle, _allFaceDir);
            }
            else
            {
                _characterAnimation.PlayAnimation(AnimationType.Walk, _allFaceDir);
            }
        }

        private void DetermineFaceDir(Vector2 velocity) 
        {
            bool anyChanges = false;

            if(velocity.x > 0) 
            {
                _allFaceDir = FaceDir.East;
                _horizontalFaceDir = FaceDir.East;
                _characterVisual.FlipSR(false);
                anyChanges = true;
            }
            if(velocity.x < 0) 
            {
                _allFaceDir = FaceDir.West;
                _horizontalFaceDir = FaceDir.West;
                _characterVisual.FlipSR(true);
                anyChanges = true;
            }
            if(velocity.y > 0) 
            {
                _allFaceDir = FaceDir.North;
                anyChanges = true;
            }
            if(velocity.y < 0) 
            {
                _allFaceDir = FaceDir.South;
                anyChanges = true;
            }

            //if (anyChanges)
            //    _debugFaceDir.OnChangeDir(_allFaceDir);
                //EventBusManager.Instance.EventBus.Emmit<IDebugFaceDir>(e => e.OnChangeDir(_faceDir));
        }

        //public Vector2Int VecCurrentPos()
        //{
        //    var vecPos = new Vector2(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));

        //    // Error adjustment when moving left
        //    if (_allFaceDir == FaceDir.West || _allFaceDir == FaceDir.East)
        //    {
        //        //vecPos = new Vector2(Mathf.CeilToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        //        vecPos.x = Mathf.CeilToInt(transform.position.x);
        //    }

        //    // Error adjustment when moving down
        //    if (_allFaceDir == FaceDir.South || _allFaceDir == FaceDir.North)
        //    {
        //        vecPos.y = Mathf.CeilToInt(transform.position.y);
        //    }
        //    if (_allFaceDir == FaceDir.North)
        //    {
        //        vecPos.y = Mathf.FloorToInt(transform.position.y);
        //    }

        //    return new Vector2Int((int)vecPos.x, (int)vecPos.y);
        //}
    }
}