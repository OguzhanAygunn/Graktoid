using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerBallShooter : MonoBehaviour
{

    [SerializeField] bool active;
    [SerializeField] EnemyType enemyType;
    [SerializeField] Transform target;
    [SerializeField] Transform spawnPos;
    [SerializeField] float counter;
    [SerializeField] float duration;
    [SerializeField] float ballCount;
    [SerializeField] float delay;
    
    private void Start()
    {
        target = PlayerController.instance.transform;
    }

    private void Update()
    {
        TimerController();
    }

    private void TimerController()
    {
        if (!active)
            return;

        counter = Mathf.MoveTowards(counter, duration, Time.deltaTime);

        if(counter == duration)
        {
            SpawnBalls(type:enemyType).Forget();
        }
    }

    private async UniTaskVoid SpawnBalls(EnemyType type)
    {
        counter = 0;
        int index = 0;

        while(index < ballCount)
        {
            index++;

            EnemySpawnerBall ball = EnemySpawnerBallManager.GetBall(type: type);
            ball.transform.position = spawnPos.position;
            ball.gameObject.SetActive(true);

            Vector3 targetPos = GetTargetBallPos();
            ball.PlayJump(targetPos);

            await UniTask.Delay(TimeSpan.FromSeconds(delay));
        }
    }

    public Vector3 GetTargetBallPos()
    {
        Vector3 pos = target.position;

        pos.x += UnityEngine.Random.Range(-4.5f, 4.5f);
        pos.y += UnityEngine.Random.Range(-4.5f, 4.5f);

        return pos;
    }

    public void SetActive(bool active)
    {
        this.active = active;
    }

}
