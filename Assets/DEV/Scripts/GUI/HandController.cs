using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    public static HandController instance;

    [Title("Main")]
    [SerializeField] bool visibility;
    [SerializeField] CanvasGroup group;
    [SerializeField] Transform hand;
    [SerializeField] float speed;

    [Title("Scales")]
    [SerializeField] bool scaleEffectActive;
    [SerializeField] Vector3 defaultScale;
    [SerializeField] Vector3 pressScale;
    [SerializeField] float scaleSpeed;
    [SerializeField] bool onPress;
    private void Awake()
    {
        instance = (!instance) ? this : instance;
        SetActiveVisibility(active: false, force: true);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            onPress = true;
        else if (Input.GetMouseButtonUp(0))
            onPress = false;


        Follow();
        ScaleController();
    }

    private void Follow()
    {
        hand.position = Vector2.Lerp(hand.position, Input.mousePosition, speed * Time.unscaledDeltaTime);
    }

    private void ScaleController()
    {
        if (!scaleEffectActive)
            return;

        Vector3 scale = hand.localScale;
        Vector3 targetScale = onPress ? pressScale : defaultScale;
        scale = Vector3.Lerp(scale, targetScale, scaleSpeed * Time.deltaTime);
        hand.localScale = scale;
    }

    [Button(size: ButtonSizes.Large)]
    public static void SetActiveVisibility(bool active,bool force)
    {
        instance.scaleEffectActive = false;
        float duration = force ? 0 : 0.25f;

        if (active)
        {
            Vector3 startScale = Vector3.one * 1.15f;
            Vector3 endScale = Vector3.one;
            instance.hand.DOScale(startScale, duration).SetEase(Ease.Linear).OnComplete( () =>
            {
                instance.hand.DOScale(endScale, duration).SetEase(Ease.Linear).OnComplete(() =>
                {
                    instance.visibility = active;
                    instance.scaleEffectActive = true;
                });
            });
        }
        else
        {
            Vector3 startScale = Vector3.one * 1.15f;
            Vector3 endScale = Vector3.zero;
            instance.hand.DOScale(startScale, duration).SetEase(Ease.Linear).OnComplete(() =>
            {
                instance.hand.DOScale(endScale, duration).SetEase(Ease.Linear).OnComplete(() =>
                {
                    instance.visibility = active;
                });
            });
        }


    }
}
