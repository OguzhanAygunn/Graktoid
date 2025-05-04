using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
    [Title("Main")]
    [SerializeField] bool available=true;
    [SerializeField] bool visibility;
    [SerializeField] Vector3 defaultPos;
    [SerializeField] Vector3 visiblePos;
    [SerializeField] float moveSpeed;

    [Space(6)]

    [Title("Counter")]
    [SerializeField] Image counterImage;
    [SerializeField] float duration;
    [SerializeField] float counter;

    RectTransform rectTransform;
    Vector3 defaultScale;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        defaultScale = transform.localScale;
        counterImage.fillAmount = 0;
    }

    private void FixedUpdate()
    {
        Movement();
    }
    private void Movement()
    {
        Vector3 targetPos = visibility ? visiblePos : defaultPos;

        rectTransform.anchoredPosition = Vector3.Lerp(rectTransform.anchoredPosition, targetPos, moveSpeed * Time.fixedDeltaTime);
    }

    public async UniTaskVoid SetVisibility(bool active = true, float delay = 0)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delay));
        visibility = active;
    }

    public void Press()
    {
        if (!available)
            return;

        PressSizeEffect();
        PlayerController.instance.Movement.Jump();
        DeAvailable().Forget();
    }

    private async UniTaskVoid DeAvailable()
    {
        available = false;
        counter = 0;
        while (counter != duration)
        {
            float fillVal = 1 - (counter / duration);
            counterImage.fillAmount = fillVal;
            counter = Mathf.MoveTowards(counter, duration, Time.deltaTime);
            await UniTask.Delay(TimeSpan.FromSeconds(Time.deltaTime));
        }
        available = true;
        SizeEffect();
    }

    [Button(size: ButtonSizes.Large)]
    [SerializeField] void PressSizeEffect()
    {
        transform.DOScale(defaultScale * 0.7f,0.1f).SetEase(Ease.Linear).OnComplete( () =>
        {
            transform.DOScale(defaultScale * 1.2f, 0.1f).SetEase(Ease.Linear).OnComplete(() =>
            {
                transform.DOScale(defaultScale, 0.1f).SetEase(Ease.Linear);
            });
        });
    }
    private void SizeEffect()
    {
        transform.localScale = defaultScale * 0.8f;

        transform.DOScale(defaultScale, 1).SetEase(Ease.OutElastic);
    }
}
