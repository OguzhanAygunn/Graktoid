using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Title("Main")]
    [SerializeField] GameObject failPanel;
    [SerializeField] GameObject winPanel;

    private void Awake()
    {
        instance = (!instance) ? this : instance;
    }

    public async UniTaskVoid CompleteLevel(bool isWin,float delay = 0)
    {
        if (delay > 0)
            await UniTask.Delay(TimeSpan.FromSeconds(delay));

        if (isWin)
        {
            Win();
        }
        else
        {
            Fail();
        }
    }

    public void Win()
    {
        winPanel.SetActive(true);
    }

    public void Fail()
    {
        failPanel.SetActive(true);
    }
}
