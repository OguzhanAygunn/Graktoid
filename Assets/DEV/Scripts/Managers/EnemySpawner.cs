using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Title("Main")]
    [SerializeField] List<Transform> spawnPoses;

    [Space(6)]

    [Title("Spawner")]
    [SerializeField] EnemyType enemyType; 
    [SerializeField] bool spawnerActive;
    [SerializeField] int enemyCount;
    [SerializeField] float duration;
    [SerializeField] float counter;
    [SerializeField] LayerMask controlLayers;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        SpawnerController();
    }

    private void SpawnerController()
    {
        if (!spawnerActive)
            return;

        counter = Mathf.MoveTowards(counter, duration, Time.deltaTime);

        if(counter == duration)
        {
            SpawnEnemies(count: enemyCount, type: enemyType).Forget();
        }
    }

    public void SetSpawnerActive(bool active)
    {
        spawnerActive = active;
    }


    [Button(size: ButtonSizes.Large)]
    public async UniTaskVoid SpawnEnemies(int count,EnemyType type)
    {
        if (GameManager.GameFinish)
            return;

        counter = 0;

        int index = 0;

        List<Transform> poses = new List<Transform>();

        spawnPoses.ForEach(p => { poses.Add(p); });

        while (index < count)
        {
            index++;

            int randomIndex = UnityEngine.Random.Range(0, poses.Count);
            Vector3 pos = poses[randomIndex].position;
            poses.RemoveAt(randomIndex);

            Collider2D collider = Physics2D.OverlapCircle(pos, 1.25f ,controlLayers);

            if (collider)
                return;


            EnemyController enemyController = EnemyManager.GetEnemy(type: type);
            enemyController.transform.position = pos;
            enemyController.gameObject.SetActive(true);
            enemyController.Init();
            enemyController.ResetEnemy();
            enemyController.SpawnEffect().Forget();

            await UniTask.Delay(TimeSpan.FromSeconds(0.05f));

            
        }
    }

    
}
