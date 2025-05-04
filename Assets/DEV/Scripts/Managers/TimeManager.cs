using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance;

    [Title("Main")]
    [SerializeField] int fps; 
    [SerializeField] TextMeshProUGUI counterTmpro;
    [SerializeField] int gameTime;
    [SerializeField] int countdownAlert = 10; 
    [SerializeField] int gameMusicDeActiveCount = 10;
    private Color tmproColor;
    private Vector3 tmproScale;
    [SerializeField] Color alertColor;

    private void Start()
    {
        Application.targetFrameRate = fps;
        tmproColor =counterTmpro.color;
        tmproScale = counterTmpro.transform.localScale;
        TimeUpdate();
        Timer();
    }


    private async void Timer()
    {

        while (gameTime > 0)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            gameTime--;
            TimeUpdate();
        }
    }


    [Button(size: ButtonSizes.Large)]
    [SerializeField] void AlertEffect()
    {
        SFXManager.PlaySFX("Countdown Alert");
        counterTmpro.DOColor(alertColor, 0.1f).SetEase(Ease.Linear).OnComplete( () => {
            counterTmpro.DOColor(tmproColor, 0.5f).SetDelay(0.1f);
        });

        counterTmpro.transform.DOScale(tmproScale * 1.2f, 0.1f).SetEase(Ease.Linear).OnComplete(() =>{
            counterTmpro.transform.DOScale(tmproScale, 0.5f).SetDelay(0.1f);
        });
    }

    private void TimeUpdate()
    {
        if (GameManager.FreezeGame)
            return;

        int minute = gameTime / 60;
        int second = gameTime % 60;

        counterTmpro.text = "0" + minute + ":" + (second < 10 ? "0" : "") + second;

        float total = (minute * 60) + (second);

        if(total <= countdownAlert)
        {
            AlertEffect();
        }

        if(total == gameMusicDeActiveCount)
        {
            GameMusicManager.SetVolume(endVolume: 0, 0.3f);
        }


        if(total == 0)
        {
            GameManager.SetGameFinish(active: true).Forget();
            counterTmpro.DOFade(0, 0.5f).SetDelay(0.25f).SetEase(Ease.Linear);
        }

    }


    public static async void SetTimeScale(float scaleVal,float duration,float delay)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delay));
        DOTween.To(() => Time.timeScale, x => Time.timeScale = x, scaleVal, duration).SetEase(Ease.Linear);
    }
}
