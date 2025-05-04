using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;
    [SerializeField] List<SFXInfo> sfxs;
    [SerializeField] AudioSource source;
    private void Awake()
    {
        instance = (!instance) ? this : instance;
        sfxs.ForEach(sfx => { sfx.Init(); });
    }


    public static void PlaySFX(string id)
    {
        SFXInfo info = GetSFX(id: id);
        if (info == null)
            return;

        AudioClip clip = info.clip;
        if (info.playable)
        {
            instance.source.PlayOneShot(clip, info.volume);
            info.IncreaseCount();
        }
            
    }

    public static SFXInfo GetSFX(string id)
    {
        return instance.sfxs.Find(s => s.id == id);
    }
}


[System.Serializable]
public class SFXInfo
{
    [Title("Main")]
    public bool playable;
    public string id;
    public AudioClip clip;
    [Range(0, 1)] public float volume = 1;

    [Space(6)]

    [Title("Count")]
    public int count;
    public int maxCount;
    public float delay;

    public void Init()
    {
        playable = true;
    }

    public async void IncreaseCount()
    {
        count++;
        UpdatePlayable();

        await UniTask.Delay(TimeSpan.FromSeconds(delay));
        DecreaseCount();
    }

    public void DecreaseCount()
    {
        count--;
        UpdatePlayable();
    }

    public void UpdatePlayable()
    {
        playable = count <= maxCount;
    }
}
