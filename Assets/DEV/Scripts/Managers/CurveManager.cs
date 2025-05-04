using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveManager : MonoBehaviour
{
    public static CurveManager instance;
    [SerializeField] List<CurveInfo> curves;
    private void Awake()
    {
        instance = (!instance) ? this : instance;
    }


    public static AnimationCurve GetCurve(string id)
    {
        return instance.curves.Find(curve => curve.id == id).curve;
    }
}


[System.Serializable]
public class CurveInfo
{
    public string id;
    public AnimationCurve curve;
}
