using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum GemState { Fall,Ready,Collected}
public enum GemType { Null, Red, Green, Blue, Purple }
public class GemManager : MonoBehaviour
{
    public static GemManager instance;
    [SerializeField] GameObject gemPrefab;
    [SerializeField] int count;
    [SerializeField] List<GemController> gems;
    private Transform gemsPool;
    private void Awake()
    {
        instance = (!instance) ? this : instance;
        gemsPool = new GameObject("Gem Pool").transform;
    }

    private void Start()
    {
        Pool();
    }


    private void Pool()
    {
        while (count > 0)
        {
            count--;
            GemController gemController = Instantiate(gemPrefab).GetComponent<GemController>();
            gemController.transform.parent = gemsPool;
            gemController.gameObject.SetActive(false);
            gems.Add(gemController);
        }
    }

    public static GemController GetGem()
    {
        return instance.gems.Find(gem => !gem.gameObject.activeInHierarchy);
    }

    public static async UniTaskVoid SpawnGem(Vector3 spawnPos, GemType type = GemType.Red, float delay = 0)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delay));

        GemController gem = GetGem();
        gem.transform.position = spawnPos;
        gem.gameObject.SetActive(true);
        gem.PlayAnim();
    }

}
