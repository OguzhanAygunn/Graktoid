using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerBallManager : MonoBehaviour
{
    public static EnemySpawnerBallManager instance;
    [SerializeField] List<SpawnerBallInfo> infos;

    [SerializeField] List<EnemySpawnerBall> balls;

    private Transform enemyBallParent;
    private void Awake()
    {
        instance = (!instance) ? this : instance;
        Pool();

        enemyBallParent = new GameObject("Enemy Balls").transform;
    }

    
    private void Pool()
    {
        foreach(SpawnerBallInfo info in infos)
        {
            int index = 1;

            while(index <= info.count)
            {
                index++;
                EnemySpawnerBall ball = Instantiate(info.prefab).GetComponent<EnemySpawnerBall>();
                ball.transform.SetParent(enemyBallParent);
                ball.gameObject.SetActive(false);
                balls.Add(ball);
            }
        }
    }

    public static EnemySpawnerBall GetBall(EnemyType type)
    {
        return instance.balls.Find(ball => ball.EnemyType == type && !ball.gameObject.activeInHierarchy);
    }
}


[System.Serializable]
public class SpawnerBallInfo
{
    public EnemyType type;
    public GameObject prefab;
    public int count;
}
