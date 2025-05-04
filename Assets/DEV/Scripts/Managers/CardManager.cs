using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    public static CardManager instance;
    [Title("Main")]
    [SerializeField] List<CardSet> cardSets;
    [SerializeField] List<CardInfo> cardInfos;
    [SerializeField] CardUI selectedCard;

    [Space(6)]

    [Title("Cards")]
    [SerializeField] List<CardUI> cards;
    [SerializeField] List<CardUI> activeCards;
    [SerializeField] float cardPerDelay;
    [SerializeField] CanvasGroup group;
    [SerializeField] CanvasGroup titleGroup;

    [Space(6)]

    [Title("GUI")]
    [SerializeField] Transform acceptButton;
    [SerializeField] Image acceptImage;
    [SerializeField] TextMeshProUGUI acceptText;

    private Vector3 defaultPos;
    private void Awake()
    {
        instance = (!instance) ? this : instance;
        SetActiveCards(active: false, force: true, delay: 0, useThePerDelay: false).Forget();
    }


    public static void AssignSelectedCard(CardUI newCard)
    {
        if (instance.selectedCard)
            instance.selectedCard.DeActiveOnSelected();

        instance.selectedCard = newCard;
    }

    public static CardInfo GetCardInfo(string id)
    {
        return instance.cardInfos.Find(info => info.id == id);
    }

    [Button(size: ButtonSizes.Large)]
    public static async UniTaskVoid SetActiveCards(bool active = true, bool force = false, float delay = 0, bool useThePerDelay = true, float mainDelay = 0)
    {

        if (active)
        {
            CameraController.SetActiveZoom(active: true).Forget();
            SkillsButton.SetVisibility(active: false).Forget();
            TimeManager.SetTimeScale(scaleVal: 0.4f, duration: 0.3f, delay: 0.25f);
        }

        await UniTask.Delay(TimeSpan.FromSeconds(mainDelay));

        float duration = force ? 0.25f : 0;
        float alphaVal = active ? 0.4f : 0;
        instance.group.interactable = active;
        instance.group.blocksRaycasts = active;
        instance.group.DOFade(alphaVal, duration);

        foreach (CardUI card in instance.cards)
        {
            card.SetActive(active: active, force: force, delay: delay);
            if (useThePerDelay)
                await UniTask.Delay(TimeSpan.FromSeconds(instance.cardPerDelay));
        }

        alphaVal = active ? 1 : 0;
        instance.titleGroup.DOFade(alphaVal, 0.2f);


        if (active)
        {

            return;
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
            int index = 1;
            instance.activeCards[index].ActiveOnSelected();
        }
        else
        {
        }

    }


    public static async UniTaskVoid DeActiveCards()
    {
        CameraController.SetActiveZoom(active: false).Forget();

        await UniTask.Delay(TimeSpan.FromSeconds(0f));

        SkillsButton.SetVisibility(active: true).Forget();

        instance.group.DOFade(0, 0.15f);
        instance.titleGroup.DOFade(0, 0.15f);

        instance.group.interactable = false;
        instance.group.blocksRaycasts = false;

        GameManager.SetFreezeGame(active: false);
        TimeManager.SetTimeScale(scaleVal: 1, duration: 1f, delay: 0);
        List<CardUI> cards = new List<CardUI>();
        instance.activeCards.ForEach(card => cards.Add(card));
        HandController.SetActiveVisibility(active: false, force: false);
        //cards.Remove(instance.selectedCard);
        CardUI mainCard = instance.selectedCard;


        CardDescriptionPanelUI.instance.DeActive();

        float delay = 0;

        cards.ForEach(card =>
        {
            card.SetActive(active: false, force: false, delay: 0);
            delay += 1f;
        });
    }


    public void AllCardsDeActive()
    {
        foreach (CardUI card in cards)
        {
            card.DeActiveOnSelected();
        }
    }

    public static void AddActiveCard(CardUI card)
    {
        if (instance.activeCards.Contains(card))
            return;

        instance.activeCards.Add(card);
    }

    public static void RemoveActiveCard(CardUI card)
    {
        if (!instance.activeCards.Contains(card))
            return;

        instance.activeCards.Remove(card);
    }

    public void AcceptButton()
    {
        SFXManager.PlaySFX("Accept Card");
        Vector3 startScale = acceptButton.transform.localScale;
        float duration = 0.025f;
        acceptButton.transform.DOScale(startScale * 0.85f, duration / 2);
        acceptButton.transform.DOScale(startScale * 1.1f, duration).SetEase(Ease.Linear).OnComplete(() =>
        {
            acceptButton.transform.DOScale(startScale, duration).SetEase(Ease.Linear);
        });

        DeActiveCards().Forget();
        GetCardInfo(selectedCard.ID.ToString()).ActiveEvents();
    }
}

[System.Serializable]
public class CardInfo
{
    public string id;
    [Title("Icons")]
    public Sprite icon1;
    public Sprite icon2;
    [Title("Texts")]
    public string titleText;
    [TextArea]
    public string descriptionText;
    [Title("Others")]
    public string colorID;
    public UnityEvent events;


    public void ActiveEvents()
    {
        events.Invoke();
    }

}

[System.Serializable]
public class CardSet
{
    public int level;
    public List<string> ids;
}
