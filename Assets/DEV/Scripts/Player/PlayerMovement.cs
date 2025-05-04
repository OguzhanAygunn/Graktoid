using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Title("General")]
    [SerializeField] bool active;
    [SerializeField] SpriteRenderer spriteRenderer;
    public bool Active
    {
        get { return active; }
        set { active = value; }
    }
    [SerializeField] FloatingJoystick joystick;
    [SerializeField] Transform spriteTrs;
    [SerializeField] Transform spriteParentTrs;

    [Space(6)]

    [Title("Speed")]
    [SerializeField] float maxSpeed;
    [SerializeField] float speed;
    [SerializeField] float speedAcceleration;

    [Space(6)]

    [Title("Push")]
    [SerializeField] bool pushActive;
    [SerializeField] float pushDuration;
    [SerializeField] float pushSpeed;
    [SerializeField] Vector2 pushPoint;

    [Space(6)]

    [Title("Jump")]
    [SerializeField] float jumpPower;
    [SerializeField] float jumpDuration;
    [SerializeField] AnimationCurve jumpCurve;
    [SerializeField] ParticleSystem jumpParticle;
    [SerializeField] AreaDamageTrigger jumpDamageTrigger;

    [Space(6)]
    [Title("Effects")]
    [SerializeField] ParticleSystem dustParticle;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 joyPos;
    private PlayerController controller;
    private Vector3 spriteDefaultScale;
    private Vector3 spriteParentDefaultScale;
    private Vector3 defaultScale;
    private Collider2D coll;
    private StateHandler stateHandler;
    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponentInChildren<Collider2D>();
        animator = GetComponentInChildren<Animator>();
        spriteDefaultScale = spriteTrs.localScale;
        spriteParentDefaultScale = spriteParentTrs.localScale;
        defaultScale = transform.localScale;
        if (dustParticle)
            dustParticle.Play(withChildren: true);
        stateHandler = GetComponent<StateHandler>();

        jumpParticle.Stop(withChildren: true);
    }

    private void Start()
    {

    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    private void FixedUpdate()
    {
        if (stateHandler.isFreeze)
            return;

        PushMove();
        InputsUpdate();
        Move();
        Flip();
        AnimatorController();
    }

    private void InputsUpdate()
    {
        joyPos = new Vector2(joystick.Horizontal, joystick.Vertical);

        float targetSpeed = joyPos.magnitude > 0 ? maxSpeed : 0;
        speed = Mathf.MoveTowards(speed, targetSpeed, speedAcceleration * Time.fixedDeltaTime);

    }

    private void Move()
    {
        float _speed = isJump ? speed * 1.5f : speed;

        Vector2 pos = rb.position;
        Vector2 plusPos = joyPos * _speed * Time.fixedDeltaTime;
        //plusPos *= pushActive ? -1 : 1;
        pos += plusPos;
        rb.MovePosition(pos);
        WeaponsController.PosUpdate();
    }

    private void PushMove()
    {
        if (pushDuration == 0 || !pushActive)
            return;

        pushDuration = Mathf.MoveTowards(pushDuration, 0, Time.fixedDeltaTime);

        Vector2 dir = rb.position - pushPoint;
        dir *= pushSpeed * Time.fixedDeltaTime;
        dir += rb.position;

        rb.MovePosition(dir);

        if(pushDuration == 0)
        {
            pushActive = false;
            speed = 0;
        }
    }

    public void PushActive(Transform enemy, float pushDur = 0.25f)
    {
        pushActive = true;
        pushPoint = enemy.transform.position;
        pushDuration = pushDur;
    }

    private void Flip()
    {
        if (!Input.GetMouseButton(0))
            return;

        Vector3 scale = spriteDefaultScale;
        scale.x = joystick.Horizontal < 0 ? -spriteDefaultScale.x : spriteDefaultScale.x;
        spriteTrs.localScale = scale;
    }

    private void AnimatorController()
    {
        float targetBlend = joyPos.magnitude > 0 ? 1 : 0;

        animator.SetFloat("Speed", targetBlend);

    }


    bool isJump = false;
    [Button(size: ButtonSizes.Large)]
    public async void Jump()
    {
        if (isJump)
            return;

        SFXManager.PlaySFX("Jump Start");
        Vector3 startScale = spriteParentTrs.localScale;

        Vector3 targetScale = startScale;
        targetScale.x = startScale.x * 0.45f;
        targetScale.y = startScale.y * 1.4f;

        float startDuration = 0.2f;
        
        spriteParentTrs.DOScaleX(targetScale.x, startDuration/ 2).SetEase(Ease.Linear);
        spriteParentTrs.DOScaleY(targetScale.y, startDuration).SetEase(Ease.Linear);

        spriteRenderer.material.DOFloat(1, "_ShiftingFade", 0.2f).SetEase(Ease.Linear);
        isJump = true;
        coll.enabled = false;
        spriteTrs.DOLocalJump(Vector3.zero, jumpPower, 1, jumpDuration).SetEase(jumpCurve).OnComplete(() =>
        {
            coll.enabled = true;
            isJump = false;
            SizeEffect();
            CameraController.PlayShake("SmallExplosion");

            jumpParticle.transform.position = transform.position;
            jumpParticle.transform.parent = null;
            jumpParticle.Play(withChildren: true);
            jumpDamageTrigger.Trigger(delay: 0.05f).Forget();
            
            spriteRenderer.material.DOFloat(0, "_ShiftingFade", 0.1f).SetEase(Ease.Linear);
            SFXManager.PlaySFX("Jump Exp");
        });


        await UniTask.Delay(TimeSpan.FromSeconds(startDuration));

        startDuration = 0.2f;
        spriteParentTrs.DOScaleX(startScale.x, startDuration - 0.05f).SetEase(Ease.Unset);
        spriteParentTrs.DOScaleY(startScale.y, startDuration).SetEase(Ease.Unset);

        await UniTask.Delay(TimeSpan.FromSeconds(startDuration));

    }


    bool sizeEffect = false;
    private void SizeEffect()
    {
        if (sizeEffect)
            return;

        Vector3 scale = spriteParentDefaultScale;
        scale *= 0.25f;

        spriteParentTrs.DOScale(scale, 0.05f).SetEase(Ease.Linear).OnComplete(() =>
        {
            scale = spriteParentDefaultScale;
            spriteParentTrs.DOScale(scale, 0.35f).SetEase(Ease.OutElastic).OnComplete(() =>
            {
                sizeEffect = false;
            });
        });
    }
}
