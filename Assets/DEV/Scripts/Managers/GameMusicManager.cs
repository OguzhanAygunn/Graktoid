using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMusicManager : MonoBehaviour
{
    public static GameMusicManager instance;
    [SerializeField] AudioSource source;


    private void Awake()
    {
        instance = (!instance) ? this : instance;
    }

    public static async void SetVolume(float endVolume=1,float duration = 0.5f,float delay = 0)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delay));
        DOTween.To(() => instance.source.volume, x => instance.source.volume = x, endVolume, duration).SetEase(Ease.Linear);
    }
}
