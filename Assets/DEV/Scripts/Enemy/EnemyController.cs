using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Title("Main")]
    [SerializeField] EnemyType enemyType;
    public EnemyType EnemyType { get { return enemyType; } set { enemyType = value; } }
    [SerializeField] Transform spriteParent;
    [SerializeField] Transform spriteTrs;

    [Space(6)]

    [Title("Health")]

    [SerializeField] bool isAlive;
    public bool IsAlive { get { return isAlive; } }
    [SerializeField] int health;
    [SerializeField] int defenceVal;
    [HideInInspector] public int maxHealth;
    public int Health
    {
        get { return health; }
    }

    [Space(6)]

    [Title("Movement")]
    [SerializeField] bool moveActive;
    [SerializeField] Transform target;
    [SerializeField] float maxSpeed;
    [SerializeField] float speedAcceleration;
    [SerializeField] float moveSpeed;

    [Space(6)]

    [Title("Push")]
    [SerializeField] bool pushActive;
    [SerializeField] float pushSpeed = 3;

    [Space(6)]

    [Title("Take Hit")]
    [SerializeField] bool the_script;
    [ShowIf(nameof(the_script))][SerializeField] SizeController sizeController;
    [SerializeField] bool takeHitEffectAvailable = true;
    [SerializeField] bool takeHit;
    [SerializeField] float takeHitSpeed;
    [SerializeField] string takeHitEffectID = "Take Hit - Blue";
    float takeHitDuration;

    [Space(6)]

    [Title("Others")]
    [SerializeField] bool spawnGem;
    [SerializeField] FireBallEnemyShooter fireBallShooter;
    [SerializeField] EnemyBoss bossSc;

    [Space(6)]

    [Title("Jumper")]
    [SerializeField] bool isJump;
    [SerializeField] Transform jumpTrs;

    private Rigidbody2D rb;
    private Vector3 spriteParentScale;
    private Vector3 spriteScale;
    private PlayerController playerController;
    private Transform playerTrs;
    private ShaderEffectController effectController;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Collider2D coll;
    private ShadowController shadowController;
    private StateHandler stateHandler;
    public StateHandler StateHandler
    {
        get { return stateHandler; }
    }


    private Vector3 defaultScale;
    [HideInInspector]public bool isInitialized;
    private void Awake()
    {
        maxHealth = health;
    }
    private void Start()
    {
        Init();
    }

    public void Init()
    {
        if (isInitialized)
            return;

        
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        playerController = PlayerController.instance;
        effectController = GetComponent<ShaderEffectController>();
        animator = GetComponentInChildren<Animator>();
        coll = GetComponentInChildren<Collider2D>();
        shadowController = GetComponentInChildren<ShadowController>();
        stateHandler = GetComponent<StateHandler>();
        
        playerTrs = playerController.transform;
        target = playerTrs;
        spriteParentScale = spriteParent.transform.localScale;
        spriteScale = spriteTrs.localScale;
        defaultScale = transform.localScale;

        isInitialized = true;
    }


    private void FixedUpdate()
    {
        if (stateHandler.isFreeze)
            return;


        SpeedController();
        Move();
        FlipController();
        AnimatorController();
    }

    private void Move()
    {
        float _moveSpeed = moveSpeed;


        if (pushActive)
        {
            if (takeHitDuration > 0)
                _moveSpeed = maxSpeed * -pushSpeed;
        }


        if (!isAlive)
            _moveSpeed = maxSpeed * -3f;


        Vector3 pos = Vector3.MoveTowards(rb.position, target.position, _moveSpeed * Time.deltaTime);

        takeHitDuration = Mathf.MoveTowards(takeHitDuration, 0, Time.fixedDeltaTime);

        rb.MovePosition(pos);
    }

    private void SpeedController()
    {
        if (takeHitDuration > 0)
             return;

        float targetSpeed = moveActive ? maxSpeed : (pushActive ? 0 : maxSpeed);
        moveSpeed = Mathf.MoveTowards(moveSpeed, targetSpeed, speedAcceleration * Time.deltaTime);
    }

    private void FlipController()
    {
        if (!isAlive)
            return;

        Vector3 scale = spriteParentScale;
        scale.x = transform.position.x > playerTrs.position.x ? -scale.x : scale.x;
        spriteParent.localScale = scale;
    }

    public async void TakeHit(Weapon weapon)
    {
        takeHit = true;

        FXManager.PlayFX(takeHitEffectID, GetTakeHitEffectSpawnPos(), 0.5f).Forget();

        takeHitDuration = 0.5f;

        if(pushActive)
            moveSpeed = 0;

        int damage = weapon.Damage - defenceVal;
        health -= damage;

        if (fireBallShooter)
            fireBallShooter.FasterModeActiveUpdate();

        if (health <= 0)
        {
            Kill();
            return;
        }

        if (takeHitEffectAvailable)
        {
            if (the_script)
                sizeController.SetActive(active: true);
            else
                animator.SetTrigger("PopUp-A");
        }


        SFXManager.PlaySFX("Hit - A");
        effectController.TakeHit();
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
    }

    public async void TakeHit(int damage = 25)
    {
        takeHit = true;

        FXManager.PlayFX(takeHitEffectID, GetTakeHitEffectSpawnPos(), 0.5f).Forget();

        takeHitDuration = 0.5f;
        moveSpeed = 0;

        
        health -= damage;

        if (fireBallShooter)
            fireBallShooter.FasterModeActiveUpdate();

        if (health <= 0)
        {
            Kill();
            return;
        }

        if (takeHitEffectAvailable)
        {
            if (the_script)
                sizeController.SetActive(active: true);
            else
                animator.SetTrigger("PopUp-A");
        }


        SFXManager.PlaySFX("Hit - A");
        effectController.TakeHit();
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
    }

    public void Kill()
    {
        isAlive = false;
        coll.enabled = false;



        if (spawnGem)
            GemManager.SpawnGem(spawnPos: transform.position, type: GemType.Red, 0.1f).Forget();


        if (bossSc)
            bossSc.Kill();

        Vector3 targetRotate = Vector3.forward * 360 * 1;
        transform.DOLocalRotate(targetRotate, 1f, RotateMode.FastBeyond360);
        shadowController.SetVisiblity(active: false, duration: 0.1f);
        transform.DOScale(Vector3.zero, 0.5f).SetDelay(0.4f);
        spriteRenderer.DOFade(0, 1f).OnComplete(() =>
        {
            EnemyManager.RemoveEnemy(this);
            gameObject.SetActive(false);
        });

        EnemyManager.IncreaseKillCount();

    }

    public Vector3 GetTakeHitEffectSpawnPos()
    {
        float x = UnityEngine.Random.Range(-0.3f, 0.3f);
        float y = UnityEngine.Random.Range(+0.0f, 0.8f);
        float z = transform.position.z;

        Vector3 pos = transform.position + new Vector3(x, y, z);

        return pos;
    }

    private void AnimatorController()
    {
        float blendVal = !moveActive || !isAlive ? 0 : 1;
        animator.SetFloat("Speed", blendVal);
    }

    public void ResetEnemy()
    {
        health = maxHealth;
        isAlive = true;
        spriteRenderer.DOFade(1, 0);
        
        shadowController?.SetVisiblity(active: true, duration: 0);
        transform.localScale = defaultScale;
        transform.localEulerAngles = Vector3.zero;
        coll.enabled = true;
    }
    
    public void CallSpawnEffect()
    {
        SpawnEffect().Forget();
    }

    [Button(size:ButtonSizes.Large)]
    public async UniTaskVoid SpawnEffect(float duration = 0.4f)
    {
        if (!isInitialized)
            Init();

        shadowController?.SetVisiblity(active: false, duration: 0.0f);
        EnemyManager.AddEnemy(this);
        moveActive = false;
        coll.enabled = false;

        FXManager.PlayFX("Enemy Spawn", transform.position, 3f).Forget();
        if(animator)
            animator.enabled = false;

        shadowController?.SetVisiblity(active: true, duration: 0.4f);

        transform.localScale = Vector3.zero;

        transform.DOScaleX(defaultScale.x, duration + duration).SetEase(Ease.OutElastic);
        transform.DOScaleY(defaultScale.y, duration).SetEase(Ease.Unset);
        transform.DOScaleZ(defaultScale.z, 0.1f);

        transform.localEulerAngles = Vector3.zero;

        await UniTask.Delay(TimeSpan.FromSeconds(duration * 2));
        moveActive = true;
        //shadowController.SetVisiblity(active: true, duration: 0.5f);
        
        coll.enabled = true;
        
        if (animator)
            animator.enabled = true;

        moveActive = true;
    }

    bool sizeEffect = false;
    private void SizeEffect()
    {
        if (sizeEffect)
            return;

        sizeEffect = true;
        Vector3 scale = spriteScale;
        scale *= 1.2f;

        AnimationCurve curve = CurveManager.GetCurve("Take Hit Enemy");

        spriteTrs.DOScale(scale, 0.05f).SetEase(Ease.Linear).OnComplete(() =>
        {
            spriteTrs.DOScale(spriteScale, 0.5f).SetEase(Ease.OutElastic).OnComplete(() =>
            {
                sizeEffect = false;
            });
        });
    }

    public void PushActive(float pushTime = 0.25f)
    {
        takeHitDuration = pushTime;
    }
}
