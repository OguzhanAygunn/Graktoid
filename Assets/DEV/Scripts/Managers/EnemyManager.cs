using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;


public enum EnemyType { Skeleton,SkeletonBoss,Goblin}
public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;

    [Title("Pool")]
    [SerializeField] List<EnemyInfo> infos;
    [SerializeField] List<EnemyController> activeEnemies;
    [SerializeField] List<EnemyController> allEnemies;

    [Space(6)]

    [Title("Events")]
    [SerializeField] int winKillCount; 
    [SerializeField] int killCount;
    [SerializeField] List<EnemyEventInfo> eventInfos;

    private Transform enemiesParent;
    private void Awake()
    {
        instance = (!instance) ? this : instance;
        enemiesParent = new GameObject("Enemies Parent").transform;

        Pool();
    }

    [Button(size: ButtonSizes.Large)]
    public void ActiveEnemiesUpdate()
    {
        activeEnemies = FindObjectsOfType<EnemyController>().ToList();
    }

    public static void AddEnemy(EnemyController enemy)
    {
        if (instance.activeEnemies.Contains(enemy))
            return;

        instance.activeEnemies.Add(enemy);
        StateHandlersManager.AddStateHandler(enemy.StateHandler);
    }

    public static void RemoveEnemy(EnemyController enemy)
    {
        if (!instance.activeEnemies.Contains(enemy))
            return;

        instance.activeEnemies.Remove(enemy);
        StateHandlersManager.RemoveStateHandler(enemy.StateHandler);
    }

    public static void KillAllEnemies()
    {
        instance.activeEnemies.ForEach(enemy => enemy.Kill());
    }

    [Button(size: ButtonSizes.Large)]
    public void AllPush(EnemyController enemySc)
    {
        activeEnemies.ForEach(enemy => {
            if (enemy != enemySc)
                enemy.PushActive(pushTime: 0.5f);
        });
    }

    private void Pool()
    {

        foreach(EnemyInfo info in infos)
        {
            GameObject prefab = info.enemyPrefab;
            int count = info.count;

            while (count > 0)
            {
                EnemyController enemy = Instantiate(prefab).GetComponent<EnemyController>();
                allEnemies.Add(enemy);
                enemy.transform.parent = enemiesParent;
                enemy.gameObject.SetActive(false);
                count--;
            }
        }
    }


    public static EnemyController GetEnemy(EnemyType type)
    {
        return instance.allEnemies.Find(enemy => enemy.EnemyType == type && !enemy.gameObject.activeInHierarchy);
    }

   public static void IncreaseKillCount(int increaseVal = 1)
    {
        instance.killCount += increaseVal;

        EnemyEventInfo eventInfo = instance.eventInfos.Find(info => info.killCount == instance.killCount);
        if(eventInfo != null)
            eventInfo.PlayEvents();

        if(instance.killCount == instance.winKillCount)
        {
            //GameManager.SetGameFinis(active: true).Forget();
        }
    }
}


[System.Serializable]
public class EnemyInfo
{
    public EnemyType enemyType;
    public GameObject enemyPrefab;
    public int count;
}

[System.Serializable]
public class EnemyEventInfo
{
    public int killCount;
    public UnityEvent unityEvent;

    public void PlayEvents()
    {
        unityEvent?.Invoke();
    }
}
