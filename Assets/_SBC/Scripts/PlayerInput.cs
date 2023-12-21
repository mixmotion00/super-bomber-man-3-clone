using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBC
{
    public class PlayerInput : MonoBehaviour
    {
        private Vector2 _directionInput;
        private ICharacterMovement _charMovement;
        private ICharacterBombing _charBombing;
        private ICharacterState _charState;

        [SerializeField] private bool _actionInput = false;
        private bool _interactReady = true;
        private float _interactInpDelay = .1f;
        private float _currentInpDelay = 0;

        // Start is called before the first frame update
        void Start()
        {
            _charMovement = GetComponent<ICharacterMovement>();
            _charBombing = GetComponent<ICharacterBombing>();
            _charState = GetComponent<ICharacterState>();
        }

        // Update is called once per frame
        void Update()
        {
            if(_currentInpDelay > 0.0f)
            {
                _currentInpDelay -= Time.deltaTime;
            }
            if (_currentInpDelay <= 0.0f)
            {
                _interactReady = true;
            }

            _directionInput = Vector2.zero;
            _actionInput = false;

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                _directionInput.x = -1;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                _directionInput.x = 1;
            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                _directionInput.y = 1;
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                _directionInput.y = -1;
            }

            if(Input.GetKeyDown(KeyCode.E)) 
            {
                //_actionInput = true;
                if (_interactReady)
                {
                    _charBombing.PlaceBomb(transform.position);
                    _currentInpDelay = _interactInpDelay;
                    _interactReady = false;
                }
            }

        }

        private void FixedUpdate()
        {
            if(_charState.IsAlive)
                _charMovement.Move(_directionInput);
            //if(_actionInput) 
            //{
            //    //Place bomb
            //    _charBombing.PlaceBomb(transform.position);
            //}
        }
    }
}
