using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Management.Instrumentation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDescriptionPanelUI : MonoBehaviour
{
    public static CardDescriptionPanelUI instance;

    [Title("Main")]
    [SerializeField] bool visibility;

    [Space(6)]

    [SerializeField] CanvasGroup group;
    [SerializeField] CardUI card;

    [Space(6)]

    [SerializeField] Image icon1;
    [SerializeField] Image icon2;

    [Space(6)]

    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI desText;


    private Vector3 defaultScale;
    private void Awake()
    {
        instance = (!instance) ? this : instance;
        defaultScale = transform.localScale;
        SetVisibility(active: false, force: true);
    }

    public static void AssignCard(CardUI newCard)
    {
        //Time.timeScale = 1;
        instance.card = newCard;
        CardInfo cardInfo = CardManager.GetCardInfo(instance.card.ID.ToString());

        instance.icon1.sprite = cardInfo.icon1;
        instance.icon2.sprite = cardInfo.icon2;

        instance.titleText.text = cardInfo.titleText + " Upgrade";
        instance.desText.SetText(cardInfo.descriptionText);
        if (!instance.visibility)
        {
            instance.SetVisibility(active: true, force: false);
        }
        else
        {
            instance.SizeEffect();
        }

        instance.desText.GetAsyncFixedUpdateTrigger();

    }

    public void SetVisibility(bool active, bool force)
    {
        visibility = active;
        float duration = force ? 0 : 0.15f;

        if (visibility)
        {
            transform.DOScale(defaultScale, duration).SetEase(Ease.Linear);
        }
        else
        {
            group.DOFade(0, duration).SetEase(Ease.Linear).OnComplete(() =>
            {
                transform.localScale = Vector3.zero;
                group.alpha = 1;
            });
        }
    }

    public void DeActive()
    {
        float duration = 0.1f;
        transform.DOScale(defaultScale * 1.2f, duration/3).SetEase(Ease.Linear).OnComplete(() =>
        {
            transform.DOScale(0, duration).SetEase(Ease.Linear);
        });
    }

    public void SizeEffect()
    {
        Vector3 startScale = transform.localScale / 1.15f;
        Vector3 endScale = transform.localScale;


        transform.DOScale(startScale, 0.06f).SetEase(Ease.Linear).OnComplete(() =>
        {
            transform.DOScale(endScale, 0.06f).SetEase(Ease.Linear);
        });
    }

}