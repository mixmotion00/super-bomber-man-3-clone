using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBC
{
    public interface ICharacterVisual
    {
        void FlipSR(bool flip);
    }

    public class CharacterVisual : MonoBehaviour, ICharacterVisual
    {
        [SerializeField] private SpriteRenderer _baseSR;

        public void FlipSR(bool flip)
        {
            _baseSR.flipX = flip;
        }
    }
}


