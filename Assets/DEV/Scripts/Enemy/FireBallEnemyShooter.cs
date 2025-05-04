using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallEnemyShooter : MonoBehaviour
{
    [Title("Main")]
    [SerializeField] bool active;
    [SerializeField] bool shootable;
    [SerializeField] bool fasterMode;
    [SerializeField] float counter;
    [SerializeField] float duration;
    [SerializeField] Transform spawnPos;
    [SerializeField] ParticleSystem particle;
    [SerializeField] float activeDistance; 
    Transform playerPos;
    Transform enemyPos;

    EnemyController enemyController;

    private void Start()
    {
        enemyController = GetComponent<EnemyController>();
        enemyPos = transform;
        playerPos = PlayerController.instance.transform;
        particle.Play(withChildren: true);
        SetActive(active: true, delay: 1f).Forget();
    }

    private void Update()
    {
        ShootableUpdate();
        TimerController();
    }

    private void ShootableUpdate()
    {
        float distance = Vector3.Distance(enemyPos.position, playerPos.position);
        shootable = distance <= activeDistance;
    }

    private void TimerController()
    {
        if (!enemyController.IsAlive)
            return;

        if (!shootable)
            return;

        if (GameManager.FreezeGame)
            return;

        if (!active)
            return;

        float dur = fasterMode ? duration / 3 : duration;

        counter = Mathf.MoveTowards(counter, dur, Time.deltaTime);


        if (counter >= dur)
        {
            ShootFireBall();
        }
    }

    private void ShootFireBall()
    {
        counter = 0;
        FireBallEnemy fireBall = FireBallManager.GetFireBall();
        fireBall.gameObject.SetActive(true);
        fireBall.transform.position = spawnPos.position;
        fireBall.Fire();
    }

    public async UniTaskVoid SetActive(bool active, float delay)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(value: delay));

        this.active = active;
    }

    public void FasterModeActiveUpdate()
    {
        fasterMode = enemyController.Health < enemyController.maxHealth / 2;
    }
}
