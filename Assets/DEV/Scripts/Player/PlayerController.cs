using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public PlayerMovement Movement { get { return movement; } }
    public PlayerHealth Health { get { return health; } }
    public StateHandler StateHandler { get { return stateHandler; } }
    [SerializeField] PlayerMovement movement;
    [SerializeField] PlayerHealth health;
    [SerializeField] StateHandler stateHandler;
    private ShaderEffectController shaderEffectController;

    private void Awake()
    {
        instance = (!instance) ? this : instance;
        shaderEffectController = GetComponent<ShaderEffectController>();
        movement = GetComponent<PlayerMovement>();

    }

    public void Init()
    {
        shaderEffectController = GetComponent<ShaderEffectController>();
        movement = GetComponent<PlayerMovement>();
    }

    public void CollectGem(GemController gemController)
    {
        shaderEffectController.Shine();
        SizeEffect();

        GameManager.SetFreezeGame(active: true, delay: 0.55f);
        CardManager.SetActiveCards(mainDelay: 0.4f).Forget();
        HandController.SetActiveVisibility(active: true, force: false);
        SFXManager.PlaySFX("Collect Gem");
    }

    public void SizeEffect()
    {
        Vector3 startScale = transform.localScale;
        Vector3 endScale = startScale * 1.3f;

        transform.DOScale(endScale, 0.25f).OnComplete(() =>
        {
            transform.DOScale(startScale, 0.25f).SetDelay(0.05f);
        });
    }

    [Button(size: ButtonSizes.Large)]
    public void TakeHit(Transform enemy, int damage = 10,Vector3 effectSpawnPoint = default(Vector3))
    {
        //CameraController.PlayShake("PlayerHit");
        shaderEffectController.TakeHit();
        Movement.PushActive(enemy);

        FXManager.PlayFX("Take Hit - Blue", effectSpawnPoint, 2f).Forget();    
    }

}
