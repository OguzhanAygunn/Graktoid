using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ExpAnimType { OneShot,Loop}
public class SpriteExpAnimatorManager : MonoBehaviour
{
    public static SpriteExpAnimatorManager instance;

    private void Awake()
    {
        instance = (!instance) ? this : instance;
    }
}
