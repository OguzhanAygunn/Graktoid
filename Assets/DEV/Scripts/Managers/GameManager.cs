using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool freezeGame;
    public bool gameFinish;
    public static bool FreezeGame
    {
        get
        {
            return instance.freezeGame;
        }
        set
        {
            instance.freezeGame = value;
        }
    }
    public static bool GameFinish { get { return instance.gameFinish; } }
    private void Awake()
    {
        instance = (!instance) ? this : instance;

    }

    private void Start()
    {
        Screen.SetResolution(Screen.currentResolution.width / 2, Screen.currentResolution.height / 2, true);
    }


    public static async void SetFreezeGame(bool active, float delay = 0)
    {
        FreezeGame = active;
        await UniTask.Delay(TimeSpan.FromSeconds(value: delay));
        StateHandlersManager.SetActiveFreeze(active: active);

    }

    public static async UniTaskVoid SetGameFinish(bool active,float delay = 0)
    {
        if (delay > 0)
            await UniTask.Delay(TimeSpan.FromSeconds(delay));

        instance.gameFinish = active;


        if (GameFinish)
        {
            EnemyManager.KillAllEnemies();
            PlayerBombSpawner.AllExplosionActiveBombs();
        }

        instance.CompleteLevel(isWin: active, delay: 1f).Forget();
    }


    public async UniTaskVoid CompleteLevel(bool isWin,float delay)
    {

        WeaponsController.instance.DeActive(delay:0.1f).Forget();

        if (delay > 0)
            await UniTask.Delay(TimeSpan.FromSeconds(delay));

        UIManager.instance.CompleteLevel(isWin: isWin, delay: 0).Forget();
    }

    public static void RestartLevel()
    {
        int levelIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(levelIndex);
    }
}
