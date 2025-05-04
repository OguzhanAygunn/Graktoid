using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXManager : MonoBehaviour
{
    public static FXManager instance;
    [SerializeField] List<FXPoolInfo> pools;
    [SerializeField] List<FX> fxs;

    private Transform fxParent;
    private void Awake()
    {
        instance = this;

        fxParent = new GameObject("FX Parent").transform;

        Application.targetFrameRate = -1;
    }

    private void Start()
    {
        Pool();
    }

    private void Pool()
    {
        int index = 0;
        foreach (FXPoolInfo info in pools)
        {
            index = 0;

            while (index < info.count)
            {
                GameObject particle = Instantiate(info.fxObj);
                particle.transform.parent = fxParent;
                particle.gameObject.SetActive(false);

                FX fx = new FX()
                {
                    id = info.id,
                    effect = particle,
                };

                fxs.Add(fx);

                index++;
            }
        }
    }


    public static async UniTaskVoid PlayFX(string id,Vector3 pos, float desDelay)
    {

        ParticleSystem particle = GetFX(id: id);
        if (!particle)
            return;
        particle.transform.position = pos;
        particle.gameObject.SetActive(true);
        particle.Clear();
        particle.Play(withChildren: true);


        await UniTask.Delay(TimeSpan.FromSeconds(desDelay));

        particle.gameObject.SetActive(false);
    }

    public static ParticleSystem GetFX(string id)
    {
        return instance.fxs.Find(fx => fx.id == id && !fx.effect.activeInHierarchy)?.effect.GetComponent<ParticleSystem>();
    }
}

[System.Serializable]
public class FXPoolInfo
{
    public string id;
    public GameObject fxObj;
    public int count;
}


[System.Serializable]
public class FX
{
    public string id;
    public GameObject effect;
}
