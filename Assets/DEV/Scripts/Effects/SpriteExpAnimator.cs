using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpriteExpAnimator : MonoBehaviour
{
    [Title("Main")]
    [SerializeField] bool animActive;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] List<Sprite> sprites;
    [SerializeField] ExpAnimType animType;
    [SerializeField] float frameSec;


    [Title("One Shoot")]
    [ShowIf("@this.animType == ExpAnimType.OneShot")][SerializeField] bool fadeOut;
    [ShowIf("@this.animType == ExpAnimType.OneShot && this.fadeOut == true")] [SerializeField]float fadeOutTime;

    private void Awake()
    {
        
    }


    [Button(size: ButtonSizes.Large)] 
    public async UniTaskVoid PlayAnim()
    {
        spriteRenderer.DOFade(1, 0);
        if(animType is ExpAnimType.OneShot)
        {
            foreach(Sprite sprite in sprites)
            {
                spriteRenderer.sprite = sprite;
                await UniTask.Delay(TimeSpan.FromSeconds(frameSec));
            }

            await UniTask.Delay(TimeSpan.FromSeconds(frameSec));
            spriteRenderer.DOFade(0, 0.35f);
        }
        else if(animType is ExpAnimType.Loop)
        {

        }
    }

}



[System.Serializable]
public class SpriteAnimInfo
{
    public int index;
    public Sprite sprite;
    public UnityEvent func;
}
