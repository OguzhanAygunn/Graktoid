using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherShadowController : MonoBehaviour
{
    [Title("Main")]
    [SerializeField] bool active = true;
    [SerializeField] bool visibility;
    [SerializeField] Transform target;
    [SerializeField] float speed;

    [Title("Scale Controller")]
    [SerializeField] Vector3 minScale;
    [SerializeField] Vector3 maxScale;
    [SerializeField] float maxDistance;


    private SpriteRenderer sRenderer;
    [SerializeField] private Vector3 defaultPos;
    [SerializeField] private Vector3 defaultScale;
    private void Awake()
    {
        minScale = transform.localScale;
        sRenderer = GetComponent<SpriteRenderer>();

        defaultPos = transform.localPosition;
        defaultScale = transform.localScale;
    }

    private void FixedUpdate()
    {
        if (!active)
            return;
        Movement();
        ScaleController();
    }


    private void ScaleController()
    {
        Vector3 posA = transform.localPosition;
        Vector3 posB = target.localPosition;

        posA.z = posB.z;

        float distance = Vector3.Distance(posA, posB);
        float scaleSlerpVal = distance / maxDistance;

        Vector3 scale = Vector3.Slerp(minScale, maxScale, scaleSlerpVal);
        transform.localScale = scale;
    }

    private void Movement()
    {
        Vector3 firstPos = transform.localPosition;
        Vector3 pos = transform.localPosition;
        Vector3 targetPos = target.localPosition;

        pos = Vector3.Lerp(pos, targetPos, speed * Time.fixedDeltaTime);
        pos.y = firstPos.y;
        transform.localPosition = pos;
    }


    public void SetVisibility(bool active, float duration = 1, float delay = 0)
    {
        visibility = active;

        float targetAlpha = visibility ? active ? 0.5f : 0.1f : 0.0f;

        sRenderer.DOFade(targetAlpha, duration).SetDelay(delay);
    }

    public void SetActive(bool active)
    {
        this.active = active;
    }

    public void ResetShadow()
    {
        transform.localPosition = defaultPos;
        transform.localScale = defaultScale;
    }
}
