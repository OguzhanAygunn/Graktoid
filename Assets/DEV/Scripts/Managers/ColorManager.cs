using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ColorManager : MonoBehaviour
{
    public static ColorManager instance;
    [SerializeField] List<ColorInfo> colorInfos;

    private void Awake()
    {
        instance = (!instance) ? this : instance;
    }

    public static ColorInfo GetColorInfo(string id)
    {
        return instance.colorInfos.Find(info => info.id == id);
    }

}

[System.Serializable]

public class ColorInfo
{
    public string id;
    public Color cardColor;
}
