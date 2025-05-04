using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateHandler : MonoBehaviour
{
    public bool isFreeze;
    [SerializeField] Animator animator;
    [SerializeField] Rigidbody2D rb;

    public async UniTaskVoid SetFreeze(bool active,float delay=0)
    {

        if (delay > 0)
            await UniTask.Delay(TimeSpan.FromSeconds(delay));

        isFreeze = active;

        if (animator)
            animator.enabled = !isFreeze;

        if (rb)
            rb.isKinematic = isFreeze;
    }

}
