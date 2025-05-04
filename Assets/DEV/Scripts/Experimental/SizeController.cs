using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeController : MonoBehaviour
{
    [Title("Main")]
    [SerializeField] Transform trs;
    [SerializeField] bool active;

    [Space(6)]

    [Title("Sizer")]
    [SerializeField] Vector3 downScale;
    [SerializeField] Vector3 upScale;
    [SerializeField] float speed;

    private void Awake()
    {
        downScale = trs.transform.localScale;
    }

    private void Update()
    {
        SizeUpdate();
    }

    private void SizeUpdate()
    {
        Vector3 scale  = trs.transform.localScale;
        Vector3 targetScale = active ? upScale : downScale;

        scale = Vector3.MoveTowards(scale, targetScale, speed * Time.deltaTime);

        trs.transform.localScale = scale;

        if (scale == targetScale && active)
            active = false;
    }

    public void SetActive(bool active)
    {
        this.active = active;
    }
}
