using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeEvent : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    private bool _ready = false;
    private Action _cacheCallback;

    public void Init(Action callback)
    {
        if(callback != null)
            _cacheCallback = callback;
        _ready = true;
    }

    private void Update()
    {
        if (!_ready) return;

        if(_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            DestroyOnFinishAnimation();
        }
    }

    public void DestroyOnFinishAnimation()
    {
        _cacheCallback?.Invoke();
        Destroy(gameObject);
    }
}
