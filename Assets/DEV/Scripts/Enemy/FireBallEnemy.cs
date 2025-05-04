using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallEnemy : MonoBehaviour
{
    private Transform target;
    [Title("Fire")]
    [SerializeField] bool isFire;
    [SerializeField] float jumpPower;
    [SerializeField] float duration;
    [SerializeField] AnimationCurve curve;
    [SerializeField] LayerMask playerLayer;

    [Space(6)]

    [Title("Effects")]
    [SerializeField] ParticleSystem fireParticle;
    [SerializeField] ParticleSystem expParticle;
    [SerializeField] LavaController lavaController;
    public void Init()
    {
        target = PlayerController.instance.transform;

        fireParticle.Stop(withChildren: true);
        expParticle.Stop(withChildren: true);
        lavaController.Init();
    }


    public void Fire()
    {
        
        fireParticle.Play(withChildren: true);
        expParticle.Stop(withChildren: true);

        isFire = true;
        Vector3 targetPos = target.position;
        targetPos += new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0);
        transform.DOJump(targetPos, jumpPower, 1, duration).SetEase(Ease.Linear).OnComplete(async () => 
        { 
            isFire = false;
            lavaController.Init();
            expParticle.Play(withChildren: true);
            fireParticle.Stop(withChildren: true);
            CameraController.PlayShake("PlayerHit");
            //lavaController.PlayAnim().Forget();
            SFXManager.PlaySFX("FireBall Exp");

            PlayerCollController();
            await UniTask.Delay(TimeSpan.FromSeconds(3.5f));
            gameObject.SetActive(value: false);
        });
    }


    private void PlayerCollController()
    {
        Collider2D coll = Physics2D.OverlapCircle(transform.position, 1f, playerLayer);

        if(coll != null)
        {
            PlayerController.instance.TakeHit(transform, 10, transform.position);
        }
    }

}
