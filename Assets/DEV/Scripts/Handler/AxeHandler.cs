using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeHandler : MonoBehaviour
{
    public static AxeHandler instance;

    [SerializeField] List<AxeInfo> axes;
    private void Awake()
    {
        instance = (!instance) ? this : instance;
    }
}

[System.Serializable]
public class AxeInfo
{
    public string id;
    public int level;
    public Sprite Sprite;
}
