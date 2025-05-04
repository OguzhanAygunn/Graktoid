using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SpriteMultipleEffect : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] Material material;
    [SerializeField] int count;
    [SerializeField] SpriteRenderer spriteRenderer;
    Vector3 targetScale;
    List<SpriteRenderer> renderers = new List<SpriteRenderer>();



    private void Awake()
    {
        targetScale = spriteRenderer.transform.lossyScale;
    }

    private void Pool()
    {
        int index = 0;
        while(index < count)
        {
            index++;
            SpriteRenderer newRenderer = Instantiate(prefab).GetComponent<SpriteRenderer>();
            newRenderer.sprite = spriteRenderer.sprite;
            newRenderer.DOFade(0, 0);
            newRenderer.transform.localScale = Vector3.zero;
            newRenderer.sprite = null;
            newRenderer.gameObject.SetActive(false);
            renderers.Add(newRenderer);
        }
    }

    [Button(size:ButtonSizes.Large)]
    public async void PlayAnim(Vector3 pos,Transform parent)
    {
        float alphaPlusVal = 0.04f;
        float alpha = 0.4f;

        Vector3 startScale = targetScale * 1.2f;
        Vector3 topScale = targetScale * 6;
        Vector3 scale;
        int index = 1;

        foreach(SpriteRenderer render in renderers)
        {
            scale = Vector3.Slerp(startScale, topScale, (float)index / (float)count);
            render.transform.parent = parent;
            render.transform.localPosition = Vector3.zero;
            
            render.gameObject.SetActive(true);
            render.sprite = spriteRenderer.sprite;
            render.material = material;
            render.transform.DOScale(scale, 1f).OnComplete( () =>
            {
                render.transform.DOScale(scale * 1.2F, 15);
            });
            render.DOFade(alpha, 0.2f);
            alpha -= alphaPlusVal;
            index++;

            
        }

        await UniTask.Delay(TimeSpan.FromSeconds(1f));

        foreach(SpriteRenderer render in renderers)
        {
            render.DOFade(0, 1F);
        }
    }
}
