using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FunkyCode;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;
using System;
using DG.Tweening;

public class LavaController : MonoBehaviour
{
    [Title("Reqs")]
    [SerializeField] SpriteRenderer sRenderer;
    [Space(6)]

    [Title("Colors")]
    [SerializeField][ColorUsage(true, true)] Color startColor;
    [SerializeField][ColorUsage(true, true)] Color endColor;
    [SerializeField] float duration;
    [SerializeField] float delay;


    private Vector3 defaultPos;
    private Vector3 defaultRot;


    public void Init()
    {
        sRenderer.DOFade(0, 0f);
        defaultPos = transform.localPosition;
        defaultRot = transform.localEulerAngles;
    }

    public void ResetTrs()
    {
        transform.localPosition = defaultPos;
        transform.localEulerAngles = defaultRot;
    }

    [Button(size: ButtonSizes.Large)]
    public async UniTaskVoid PlayAnim()
    {
        sRenderer.DOFade(1, 0.5f);

        await UniTask.Delay(TimeSpan.FromSeconds(delay + 0.5f));

        sRenderer.DOFade(0, 0.5f);
    }
}
