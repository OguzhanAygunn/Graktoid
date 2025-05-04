using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemController : MonoBehaviour
{
    [Title("Main")]
    [SerializeField] GemType gemType;
    [SerializeField] GemState gemState;
    [SerializeField] Transform spriteTrs;
    [SerializeField] SpriteRenderer gemRenderer;
    [SerializeField] SpriteMultipleEffect gemMultipleEffect;
    [SerializeField] CircleCollider2D fakeCollider;
    public Transform SpriteTrs
    {
        get { return spriteTrs; }
    }

    [Space(6)]

    [Title("Jump")]
    [SerializeField] float jumpPower;
    [SerializeField] float jumpDuration;
    [SerializeField] Vector2 jumpOffset;

    [Space(6)]
    [Title("Coll")]
    [SerializeField] float triggerDistance; 


    private Vector3 spriteDefaultPos;
    private Vector3 spriteDefaultScale;
    private Vector3 spriteDefaultRot;
    private Vector3 targetSize;
    private Transform playerPos;


    private void Awake()
    {
        spriteDefaultPos   = spriteTrs.transform.position;
        spriteDefaultRot   = spriteTrs.transform.eulerAngles;
        spriteDefaultScale = spriteTrs.transform.localScale;
        SetActiveFakeCircle(active: false);
    }

    private void Start()
    {
        playerPos = PlayerController.instance.transform;
    }

    private void FixedUpdate()
    {
        DistanceController();
    }

    public void SetActiveFakeCircle(bool active)
    {
        return;
        fakeCollider.enabled = active;
    }

    [Button(size: ButtonSizes.Large)]
    public  async void PlayAnim()
    {

        fakeCollider.enabled = false;
        SetActiveFakeCircle(active: true);
        gemRenderer.sortingOrder = 3;
        FXManager.PlayFX("Spawn Gem - Yellow", spriteTrs.transform.position, 1.2f).Forget();


        gemState = GemState.Fall;

        //Play
        Vector3 endScale = spriteTrs.transform.localScale;
        spriteTrs.transform.localScale = Vector3.zero;
        spriteTrs.transform.DOScale(endScale, 0.1f);


        Vector3 endPos = spriteTrs.transform.position;
        endPos.x += UnityEngine.Random.Range(-jumpOffset.x, jumpOffset.x);
        endPos.y += UnityEngine.Random.Range(-jumpOffset.y, jumpOffset.y);




        Vector3 targetRot = Vector3.forward * 360;
        int flipCount = 2;
        targetRot *= flipCount;

        spriteTrs.transform.DORotate(targetRot, jumpDuration - 0.05f, RotateMode.FastBeyond360);


        spriteTrs.transform.DOJump(endPos, jumpPower, 1, jumpDuration).SetEase(Ease.Linear).OnComplete( () =>
        {
            fakeCollider.enabled = true;

            gemRenderer.sortingOrder = 0;
            spriteTrs.transform.DOJump(endPos, 0.4f, 1, 0.4f).SetEase(Ease.Linear).OnComplete(() =>
            {
                spriteTrs.transform.DOJump(endPos, 0.2f, 1, 0.3f).SetEase(Ease.Linear).OnComplete( () =>
                { 
                    spriteTrs.transform.DOJump(endPos, 0.1f, 1, 0.3f).SetEase(Ease.Linear).OnComplete( () =>
                    {
                        spriteTrs.transform.DOJump(endPos, 0.05f, 1, 0.2f).SetEase(Ease.Linear).OnComplete( () =>
                        {
                            SizeEffect();
                        });
                    });
                });
            });
        });

        await UniTask.Delay(TimeSpan.FromSeconds(2f));
        gemState = GemState.Ready;

        
    }


    private void DistanceController()
    {
        if (gemState != GemState.Ready)
            return;

        float distance = Vector3.Distance(spriteTrs.position, playerPos.position);

        if(distance < triggerDistance)
        {
            Collected();
        }
    }


    private void Collected()
    {
        FXManager.PlayFX("Spawn Gem - Yellow", spriteTrs.position, 1.2f).Forget();
        gemState = GemState.Collected;
        //gemMultipleEffect.PlayAnim(pos: playerPos.position, parent: playerPos);
        PlayerController.instance.CollectGem(this);
        Destroy(gameObject);
    }


    private void SizeEffect()
    {
        Vector3 startPos = spriteTrs.localPosition;
        Vector3 endPos = startPos + Vector3.up * 0.25f;

        spriteTrs.DOLocalMove(endPos, 0.4f).SetEase(Ease.Linear).OnComplete(() =>
        {
            spriteTrs.DOLocalMove(startPos, 0.4f).SetEase(Ease.Linear).OnComplete(() =>
            {
                SizeEffect();
            });
        });
    }
}
