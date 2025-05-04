using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBomb : MonoBehaviour
{
    [Title("Main")]
    [SerializeField] PlayerBombState state = PlayerBombState.Fall;
    [SerializeField] Transform spriteTrs;
    [SerializeField] Transform shadowTrs;
    [SerializeField] OtherShadowController shadowController;

    [Space(6)]

    [Title("Spawn Anim")]
    [SerializeField] float spawnAnimJump;
    [SerializeField] float spawnAnimJumpDuration;

    [Space(6)]

    [Title("Explosion")]
    [SerializeField] SpriteRenderer bombRenderer;
    [SerializeField] string propID;
    [SerializeField] float expEffectDuration;
    [SerializeField] float propPlusVal;
    [SerializeField] ParticleSystem expParticle;
    [SerializeField] AreaDamageTrigger damageTrigger;


    [Space(6)]
    [Title("Radar")]
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] float radius;

    private Vector3 defaultPos;
    private Vector3 defaultScale;
    private Vector3 defaultRotate;

    private readonly float expTime = 5;


    private void Awake()
    {
        defaultPos = spriteTrs.localPosition;
        defaultRotate = spriteTrs.localEulerAngles;
        defaultScale = spriteTrs.localScale;
        expParticle.Stop(withChildren: true);
    }

    private void Start()
    {
    }

    private void OnEnable()
    {
        expParticle.Stop(withChildren: true);
    }

    private void Update()
    {
        DistanceController();
    }

    private void DistanceController()
    {
        if (state != PlayerBombState.Idle)
            return;

        Collider2D coll = Physics2D.OverlapCircle(spriteTrs.position, radius: radius, layerMask: enemyLayer);

        if (coll)
        {
            Explosion().Forget();
        }
    }
    public async UniTaskVoid PlaySpawnAnim()
    {
        if (state is PlayerBombState.Explode)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1f));
            ResetBomb();
        }

        transform.position = PlayerController.instance.transform.position;
            

        state = PlayerBombState.Fall;
        expParticle.Stop(withChildren: true);
        shadowController.SetVisibility(active: false, duration: 0.0f, delay: 0f);
        shadowController.SetVisibility(active: true, duration: 1f, delay: 0.75f);

        Vector3 startPos = spriteTrs.localPosition;
        Vector3 startScale = spriteTrs.localScale;

        Vector3 targetRotate = spriteTrs.localEulerAngles;
        targetRotate.z += 360 * 2;

        float scaleDuration = 0.2f;
        float rotateDuration = spawnAnimJumpDuration;

        spriteTrs.localScale = Vector3.zero;

        spriteTrs.DOLocalJump(startPos, spawnAnimJump, 1, spawnAnimJumpDuration).SetEase(Ease.Linear).OnComplete(() => SpawnSizeAnim());
        spriteTrs.DOScale(startScale, scaleDuration);
        spriteTrs.DOLocalRotate(targetRotate, rotateDuration, RotateMode.FastBeyond360);

        await UniTask.Delay(TimeSpan.FromSeconds(expTime));

        Explosion().Forget();
    }


    private void SpawnSizeAnim()
    {
        shadowController.SetActive(active: false);
        Vector3 startScale = spriteTrs.localScale;
        Vector3 targetScale = startScale;
        targetScale *= 0.75f;
        float duration = 0.08f;

        shadowTrs.SetParent(spriteTrs);

        spriteTrs.transform.DOScale(targetScale, duration).OnComplete(() =>
        {
            targetScale = startScale * 1.15f;
            spriteTrs.transform.DOScale(targetScale, duration).SetEase(Ease.Linear).OnComplete(() =>
            {

                targetScale = startScale;
                spriteTrs.transform.DOScale(targetScale, duration).SetEase(Ease.Linear).OnComplete(() =>
                {
                    shadowTrs.SetParent(transform);
                    shadowController.SetActive(active: true);
                    IdleAnim().Forget();
                    state = PlayerBombState.Idle;
                });
            });
        });


    }


    private async UniTaskVoid IdleAnim()
    {
        float duration = UnityEngine.Random.Range(0.7f, 0.8f);

        Vector3 startPos = spriteTrs.localPosition;
        float jumpPow = 0.5f;
        await UniTask.Delay(TimeSpan.FromSeconds(0));


        while (true)
        {
            spriteTrs.DOLocalJump(startPos, jumpPow, 1, duration).SetEase(Ease.Linear);
            ShaderEffect();
            await UniTask.Delay(TimeSpan.FromSeconds(duration));

            if (state is PlayerBombState.Explode)
                return;
        }

    }


    private void ShaderEffect()
    {
        float propVal = bombRenderer.material.GetFloat(propID);
        propVal += propPlusVal;
        bombRenderer.material.DOFloat(propVal, propID, expEffectDuration).SetEase(Ease.Linear).OnComplete(() =>
        {
            bombRenderer.material.DOFloat(0, propID, expEffectDuration / 2).SetEase(Ease.Linear);
        });
    }


    [Button(size: ButtonSizes.Large)]
    public async UniTaskVoid Explosion(float delay = 0)
    {

        if (state == PlayerBombState.Explode)
            return;

        await UniTask.Delay(TimeSpan.FromSeconds(delay));

        PlayerBombSpawner.RemoveActiveBomb(this);

        state = PlayerBombState.Explode;
        bombRenderer.DOFade(0, 0);
        expParticle.Play(withChildren: true);
        shadowController.SetVisibility(false);

        damageTrigger.Trigger(delay: 0.1f).Forget();
        CameraController.PlayShake("PlayerHit");
        SFXManager.PlaySFX("FireBall Exp");
        await UniTask.Delay(TimeSpan.FromSeconds(1f));
        gameObject.SetActive(value: false);
        
    }

    private void ResetBomb()
    {
        spriteTrs.localPosition = defaultPos;
        shadowController.ResetShadow();
        bombRenderer.DOFade(1, 0);

    }
}
