using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    public static WeaponHandler instance;

    [SerializeField] List<WeaponInfo> weapons;
    private void Awake()
    {
        instance = (!instance) ? this : instance;
    }

    public static WeaponInfo GetWeaponInfo(int level)
    {
        return instance.weapons.Find(info => info.level == level);
    }

}

[System.Serializable]
public class WeaponInfo
{
    public string id;
    public int level;
    public Sprite sprite;
    public bool inverseRotate;
}
