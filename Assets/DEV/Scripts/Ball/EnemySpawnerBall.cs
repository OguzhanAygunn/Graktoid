using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerBall : MonoBehaviour
{
    [Title("Main")]
    [SerializeField] bool active;
    [SerializeField] EnemyType type;
    public EnemyType EnemyType { get { return type; } }
    [Space(6)]
    [SerializeField] float jumpPower;
    [SerializeField] float jumpDuration;
    [Space(6)]
    [SerializeField] ParticleSystem trailParticle;
    [SerializeField] ParticleSystem expParticle;
    [SerializeField] AreaDamageTrigger damageTrigger;
    [Space(6)]
    [SerializeField] Transform spawnPos;
    [SerializeField] Transform target;


    private void Awake()
    {
        expParticle.Stop(withChildren: true);
    }


    [Button(size: ButtonSizes.Large)]
    public void PlayJump(Vector3 targetPos)
    {
        trailParticle.Play(withChildren: true);
        expParticle.Stop(withChildren: true);


        active = true;
        transform.DOJump(targetPos, jumpPower, 1, jumpDuration).SetEase(Ease.Linear).OnComplete( () =>
        {
            expParticle.Play(withChildren: true);
            trailParticle.Stop(withChildren: true);

            CameraController.PlayShake("SmallExplosion");

            EnemyController enemy = EnemyManager.GetEnemy(type: type);
            Vector3 spawnPos = transform.position;
            spawnPos.z = -1f;
            enemy.transform.position = spawnPos;
            enemy.gameObject.SetActive(true);
            enemy.ResetEnemy();

        });
    }




}
