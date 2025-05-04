using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderEffectController : MonoBehaviour
{
    [Title("Take Hit")]
    [SerializeField] SpriteRenderer takeHitRenderer;
    [SerializeField] Material takeHitmaterial;
    [SerializeField] string takeHitPropID;
    [SerializeField] float takeHitDuration;
    [SerializeField] float takeHitDelay;

    [Title("Body Shine")]
    [SerializeField] SpriteRenderer shineRenderer;
    [SerializeField] Material shineMaterial;
    [SerializeField] string shinePropID;
    [SerializeField] float shineDuration;
    [SerializeField] float shineDelay;

    private void Awake()
    {
        if (takeHitRenderer)
            takeHitmaterial = takeHitRenderer.material;
        if (shineRenderer)
            shineMaterial = shineRenderer?.material;
    }

    public void TakeHit()
    {
        takeHitmaterial.DOFloat(1, takeHitPropID, takeHitDuration).OnComplete(() =>
        {
            takeHitmaterial.DOFloat(0, takeHitPropID, takeHitDuration).SetDelay(takeHitDelay);
        });
    }

    [Button(size: ButtonSizes.Large)]
    public void Shine()
    {
        shineMaterial.DOFloat(1, shinePropID, shineDuration).OnComplete(() =>
        {
            shineMaterial.DOFloat(0, shinePropID, shineDuration).SetDelay(shineDelay);
        });
    }
}
