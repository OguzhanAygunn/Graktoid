using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillsButton : MonoBehaviour
{
    public static SkillsButton instance;
    
    [Title("Main")]
    [SerializeField] bool visibile;
    [SerializeField] CanvasGroup group;

    private void Awake()
    {
        instance = (!instance) ? this : instance;
    }

    private void Start()
    {
        SetVisibility(active: true, force: true, delay: 0).Forget();
    }


    [Button(size: ButtonSizes.Large)]
    public static async UniTaskVoid SetVisibility(bool active=true, bool force=false, float delay=0)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delay));
        float duration = force ? 0 : 0.2f;
        instance.visibile = active;

        instance.group.interactable = instance.visibile;
        instance.group.blocksRaycasts = instance.visibile;

        float targetAlphaVal = active ? 1f : 0f;
        instance.group.DOFade(targetAlphaVal, duration).SetEase(Ease.Linear);
    }

}
