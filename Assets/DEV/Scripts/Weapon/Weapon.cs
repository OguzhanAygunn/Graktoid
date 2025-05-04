using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Title("Main")]
    [SerializeField] int minDamage;
    [SerializeField] int maxDamage;
    [SerializeField] int level;
    [SerializeField] Transform effectSpawnPos;
    [SerializeField] int orderIndex;
    public int OrderIndex
    {
        get { return orderIndex; }
        set { orderIndex = value; }
    }
    [Space(6)]

    [Title("Push")]
    [SerializeField] bool push;
    [SerializeField] Vector3 defaultPos;
    [SerializeField] Transform pushPos;
    [SerializeField] float pushDuration = 0.5f;
    [SerializeField] float pushDelay = 0.1f;
    private Vector3 pushDefaultPos;
    private Vector3 defaultScale;


    public Transform EffectSpawnPos
    {
        get
        {
            return effectSpawnPos;
        }
    }

    public int Damage
    {
        get { return UnityEngine.Random.Range(minDamage, maxDamage); }
    }


    private SpriteRenderer spriteRenderer;
    private WeaponsController weaponController;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        OrderIndexRandomize();
    }

    private void Start()
    {
        pushDefaultPos = pushPos.localPosition;
        weaponController = WeaponsController.instance;
        defaultScale = transform.localScale;
    }


    public void ToPos(Vector3 targetPos)
    {
        transform.DOLocalMove(targetPos, 1f).OnComplete(() =>
        {
            defaultPos = transform.localPosition;
        });
    }

    public void ToLook(Vector3 lookPos)
    {
        transform.DOLocalRotate(lookPos, 0.5f);
    }


    public void SetWeapon(int newLevel)
    {
        level = newLevel;
        WeaponInfo weaponInfo = WeaponHandler.GetWeaponInfo(level);

        spriteRenderer.sprite = weaponInfo.sprite;
    }

    public void OrderIndexRandomize()
    {
        orderIndex = UnityEngine.Random.Range(0, 100);
    }

    [Button(size: ButtonSizes.Large)]
    public async UniTaskVoid Push()
    {

        if (push)
            return;

        push = true;
        pushPos.parent = weaponController.transform;

        transform.DOScale(defaultScale * 1.3f, 0.05f).SetEase(Ease.Linear);
        transform.DOLocalMove(pushPos.localPosition, pushDuration).SetEase(Ease.Linear).OnComplete(() =>
        {

            transform.DOScale(defaultScale, 0.4f).SetEase(Ease.Linear).SetDelay(0.3f);
            transform.DOLocalMove(defaultPos, pushDuration * 1.4f).SetEase(Ease.Unset).SetDelay(pushDelay).OnComplete(() =>
            {

                pushPos.parent = transform;
                push = false;
            });
        });
        await UniTask.Delay(TimeSpan.FromSeconds(1f));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            EnemyController enemyController = other.GetComponentInParent<EnemyController>();
            enemyController.TakeHit(weapon: this);
        }
    }
}
