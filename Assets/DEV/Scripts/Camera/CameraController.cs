using Com.LuisPedroFonseca.ProCamera2D;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;
    [SerializeField] private Camera cam;
    [SerializeField] ProCamera2DShake shaker;


    [SerializeField] List<CameraFOVInfo> fovs;
    private void Awake()
    {
        cam = GetComponent<Camera>();
        instance = (!instance) ? this : instance;
    }


    public static void PlayShake(string id)
    {
        instance.shaker.Shake(id);
    }

    public static async UniTaskVoid SetActiveZoom(bool active, float delay = 0)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delay));

        if (active)
            ActiveZoom();
        else
            DeActiveZoom().Forget();
    }

    public static void ActiveZoom()
    {
        PlayZoom("zoom").Forget();
    }

    public static async UniTaskVoid DeActiveZoom()
    {
        CameraFOVInfo info = GetFOVInfo("before default");

        instance.cam.DOOrthoSize(info.fov, info.duration).SetEase(info.ease);
        await UniTask.Delay(TimeSpan.FromSeconds(info.duration));

        info = GetFOVInfo("default");
        instance.cam.DOOrthoSize(info.fov, info.duration).SetEase(info.ease);
    }

    [Button(size: ButtonSizes.Large)]
    public static async UniTaskVoid PlayZoom(string id,float delay = 0)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delay));
        CameraFOVInfo info = GetFOVInfo(id: id);
        instance.cam.DOOrthoSize(info.fov, info.duration).SetEase(info.ease);
        //DOTween.To(() => instance.cam.orthographicSize, x => instance.cam.orthographicSize = x, info.fov, info.duration).SetEase(info.ease);
    }

    public static CameraFOVInfo GetFOVInfo(string id)
    {
        return instance.fovs.Find(f => f.id == id);
    }


}


[System.Serializable]
public class CameraFOVInfo
{
    public string id;
    public float fov;
    public float duration;
    public Ease ease;
}
