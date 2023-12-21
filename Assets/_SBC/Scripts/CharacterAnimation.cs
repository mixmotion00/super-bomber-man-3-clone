using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBC
{
    public enum AnimationType
    {
        Idle,
        Walk,
        Die,
        Won
    }

    public interface ICharacterAnimation
    {
        bool AnimationIsDone(string animation, float completeProgress = 1.0f);
        void PlayAnimation(AnimationType type, FaceDir faceDir);
        bool AnimationIsPlaying(string animation);
    }

    public class CharacterAnimation : MonoBehaviour, ICharacterAnimation
    {
        [SerializeField] private List<Animator> _animators = new List<Animator>();

        public void PlayAnimation(AnimationType animationType, FaceDir faceDir)
        {
            string animStr = "";

            switch (animationType)
            {
                case AnimationType.Idle:
                    animStr = "Idle";
                    break;
                case AnimationType.Walk:
                    animStr = "Walk";
                    break;
                case AnimationType.Die:
                    animStr = "Die";
                    break;
                case AnimationType.Won:
                    animStr = "Won";
                    break;
            }

            if(animationType != AnimationType.Die)
            {
                foreach (Animator anim in _animators)
                {
                    if (faceDir == FaceDir.West || faceDir == FaceDir.East)
                        anim.Play($"{animStr}-Side");

                    else anim.Play($"{animStr}-{faceDir}");
                }
            }
            else
            {
                _animators.ForEach(a => a.Play(animStr));
            }
        }

        public bool AnimationIsDone(string animation, float completeProgress = 1.0f)
        {
            if (_animators[0].GetCurrentAnimatorStateInfo(0).IsName(animation)
                && _animators[0].GetCurrentAnimatorStateInfo(0).normalizedTime >= completeProgress)
            {
                return true;
            }

            return false;
        }

        public bool AnimationIsPlaying(string animation)
        {
            // Get the current state information from the default layer (layer 0)
            AnimatorStateInfo stateInfo = _animators[0].GetCurrentAnimatorStateInfo(0);
            return stateInfo.IsName(animation);
        }

    }

}

