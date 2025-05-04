using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpManager : MonoBehaviour
{
    public static ExpManager instance;

    [Title("Main")]
    [SerializeField] int expCount;
    [SerializeField] List<LevelExpInfo> levelExpInfos;
    [SerializeField] Slider slider;
    private LevelExpInfo curretInfo;
    public LevelExpInfo CurrentInfo { get { return curretInfo; } }
    private void Awake()
    {
        instance = (!instance) ? this : instance;

        //UpdateCurrentInfo(level: 2);
        //ExpUpdateUI();
    }


    [Button(size: ButtonSizes.Large)]
    public static void IncreaseExp(int increaseVal = 0)
    {
        instance.expCount += increaseVal;
        ExpUpdateUI();
    }

    public static void ExpUpdateUI()
    {
        instance.slider.value = (float)instance.expCount / (float)instance.curretInfo.xp;
    }


    public static void UpdateCurrentInfo(int level)
    {
        instance.curretInfo = GetLevelExpInfo(level: level);
    }

    public static LevelExpInfo GetLevelExpInfo(int level)
    {
        return instance.levelExpInfos.Find(expInfo => expInfo.level == level);
    }
}



[System.Serializable]
public class LevelExpInfo
{
    public int level;
    public int xp;
}
