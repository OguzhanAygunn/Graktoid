using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{

    [Title("Main")]
    [SerializeField] int id;
    public int ID { get { return id; } }
    [SerializeField] bool active;
    [SerializeField] bool visibility;
    [SerializeField] bool onSelected;

    [Space(4)]

    [Title("GUI")]
    [SerializeField] Image mainBackground;
    [SerializeField] Image background;
    [SerializeField] TextMeshProUGUI titleTmpro;
    [SerializeField] string titleText;
    [SerializeField] string description;

    [Space(4)]

    [SerializeField] Image icon1;
    [SerializeField] Image icon2;

    [Title("Transforms")]
    [SerializeField] Vector3 deActivePos;


    private Vector3 defaultPos;
    private Vector3 defaultRot;
    private Vector3 defaultScale;

    private Color mainOutlineColor;
    private Color targetMainOutlineColor;

    private void OnValidate()
    {


    }

    private void Awake()
    {
        defaultPos = transform.localPosition;
        defaultRot = transform.localEulerAngles;
        defaultScale = transform.localScale;
    }

    private void Start()
    {
        mainOutlineColor = ColorManager.GetColorInfo("Green - Card Outline").cardColor;
        targetMainOutlineColor = mainOutlineColor;
    }

    private void FixedUpdate()
    {
        MainBackgroundColorController();
    }

    private void MainBackgroundColorController()
    {
        if (!active)
            return;

        mainBackground.color = Vector4.MoveTowards(mainBackground.color, mainOutlineColor, 8 * Time.deltaTime);

        if (mainBackground.color == mainOutlineColor)
        {
            mainOutlineColor = mainOutlineColor == targetMainOutlineColor ? Color.white : targetMainOutlineColor;
        }
    }

    public void UpdateUI(string id)
    {
        CardInfo cardInfo = CardManager.GetCardInfo(id: id);

        titleTmpro.text = cardInfo.titleText;

        if (cardInfo.icon1)
            icon1.sprite = cardInfo.icon1;
        if (cardInfo.icon2)
            icon2.sprite = cardInfo.icon2;

        ColorInfo colorInfo = ColorManager.GetColorInfo(id: cardInfo.colorID);
        background.color = colorInfo.cardColor;
    }

    public async void SetActive(bool active, bool force, float delay)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delay));

        if (active)
        {
            CardManager.AddActiveCard(this);
            UpdateUI(id: id.ToString());
            float duration = force ? 0 : 0.35f;
            Vector3 targetRot = defaultRot;
            targetRot.z += 360 * 2;
            targetRot.z += 20;

            transform.localScale = Vector3.zero;
            transform.DOLocalMove(defaultPos, duration);
            transform.DOLocalRotate(targetRot, duration, RotateMode.FastBeyond360).OnComplete(() =>
            {
                targetRot.z -= 20;
                transform.DOLocalRotate(targetRot, duration / 2f);
            });
            transform.DOScale(defaultScale, duration / 1.5f);

        }
        else
        {
            CardManager.RemoveActiveCard(this);

            float duration = force ? 0 : 0.4f;
            deActivePos.x = defaultPos.x;
            transform.DOLocalMove(deActivePos, duration);
        }
    }

    public void ActiveOnSelected()
    {
        SetActiveOnSelected(active: true, force: false, delay: 0).Forget();
    }

    public void DeActiveOnSelected()
    {
        SetActiveOnSelected(active: false, force: false, delay: 0).Forget();
    }

    [Button(size: ButtonSizes.Large)]
    public async UniTaskVoid SetActiveOnSelected(bool active, bool force, float delay)
    {
        if (this.active == active)
            return;


        if (active)
        {
            CardManager.AssignSelectedCard(this);
            SFXManager.PlaySFX("Click Card");
        }
            

        this.active = active;

        await UniTask.Delay(TimeSpan.FromSeconds(delay));

        float duration = force ? 0 : 0.06f;

        Vector3 startScale = active ? defaultScale * 1.25f : defaultScale * 1.2f;
        Vector3 endScale = active ? defaultScale * 1.1f : defaultScale;
        Color mainColor = active ? ColorManager.GetColorInfo("Green - Card Outline").cardColor : Color.white;

        if (!active)
            mainBackground.color = Color.white;
        if (active)
        {
            DOTween.To(() => titleTmpro.characterSpacing, x => titleTmpro.characterSpacing = x, 20, .1f).OnComplete(() =>
            {
                DOTween.To(() => titleTmpro.characterSpacing, x => titleTmpro.characterSpacing = x, 0, .1f);
            });


            icon1.transform.DOScale(startScale * 1.125f, duration * 1.2f).SetEase(Ease.Linear).OnComplete(() =>
            {
                icon1.transform.DOScale(Vector3.one, duration).SetEase(Ease.Linear);
            });

            icon2.transform.DOScale(startScale * 1.125f, duration * 1.3f).SetEase(Ease.Linear).OnComplete(() =>
            {
                icon2.transform.DOScale(Vector3.one, duration).SetEase(Ease.Linear);
            });

            CardDescriptionPanelUI.AssignCard(this);
        }

        transform.DOScale(startScale, duration).SetEase(Ease.Linear).OnComplete(() =>
        {
            transform.DOScale(endScale, duration * 2).SetEase(Ease.Linear);
        });
    }
}
