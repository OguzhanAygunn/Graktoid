using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }


    public void SetVisiblity(bool active, float duration)
    {
        float alphaVal = active ? 1f : 0f;
        spriteRenderer.DOFade(alphaVal, duration).SetEase(Ease.Linear);
    }
}
