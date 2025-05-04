using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum PushType { InOrder, Random }
public class WeaponsController : MonoBehaviour
{
    public static WeaponsController instance;


    [Title("Main")]
    [SerializeField] List<Weapon> activeWeapons;
    [SerializeField] Transform weaponSpawnPoint;
    [SerializeField] Transform weaponSpawnParentPoint;
    [SerializeField] Transform followTrs;
    [SerializeField] Vector3 offset;
    [SerializeField] float rotateSpeed;
    [SerializeField] int activeWeaponCount;


    [Space(6)]

    [Title("Active")]
    [SerializeField] int weaponLevel;
    [SerializeField] int activeCount;
    WeaponManager weaponManager;

    [Space(6)]

    [Title("Push")]
    [SerializeField] bool pushActive;
    [SerializeField] PushType pushType;
    [SerializeField] int pushWeaponCount;
    [SerializeField] float pushDelay;
    private StateHandler stateHandler;
    private void Awake()
    {
        instance = (!instance) ? this : instance;
    }
    private void Start()
    {
        weaponManager = WeaponManager.instance;
        stateHandler = GetComponent<StateHandler>();
        ActiveWeapons(count: activeWeaponCount).Forget();

    }

    private void LateUpdate()
    {
    }

    private void FixedUpdate()
    {
        if (stateHandler.isFreeze)
            return;

        Rotate();
    }

    public static void PosUpdate()
    {
        instance.transform.localPosition = instance.followTrs.position + instance.offset;
    }

    private void Rotate()
    {
        transform.Rotate(Vector3.forward * -rotateSpeed * Time.fixedDeltaTime);
    }
    [Button(size: ButtonSizes.Large)]
    public async UniTaskVoid ActiveWeapons(int count)
    {

        if(activeWeapons.Count > 0)
        {
            foreach(Weapon weapon in activeWeapons)
            {
                weapon.transform.DOLocalMove(Vector3.zero, 0.7f).OnComplete(() => weapon.gameObject.SetActive(value: false));
            }

            activeWeapons.Clear();
        }

        await UniTask.Delay(TimeSpan.FromSeconds(0.8f));

        List<Weapon> weapons = WeaponManager.GetWeapons(count: count);

        weapons.ForEach(weapon => { weapon.gameObject.SetActive(true); });


        float rotateVal = 360f / count;
        Vector3 pos = Vector3.zero;
        Vector3 lookPos = Vector3.zero;
        Vector3 lookRot = Vector3.zero;
        int index = 1;
        //weaponSpawnParentPoint.transform.localEulerAngles = Vector3.zero;


        foreach (Weapon weapon in weapons)
        {
            weapon.SetWeapon(weaponLevel);
            weapon.transform.position = transform.position;
            weaponSpawnParentPoint.localEulerAngles += Vector3.forward * rotateVal;

            weaponSpawnPoint.parent = transform;
            pos = weaponSpawnPoint.transform.localPosition;
            weaponSpawnPoint.parent = weaponSpawnParentPoint;
            lookPos = weaponSpawnPoint.transform.position;
            lookRot = weaponSpawnParentPoint.localEulerAngles;

            weapon.gameObject.SetActive(true);
            weapon.ToPos(pos);
            weapon.ToLook(lookRot);
            activeWeapons.Add(weapon);
            index++;
        }

    }

    public void CallActiveWeapons(int count)
    {
        ActiveWeapons(count: count).Forget();
    }


    [Button(size: ButtonSizes.Large)]
    public async UniTaskVoid Push(float delay = 0)
    {

        await UniTask.Delay(TimeSpan.FromSeconds(value: delay));

        List<Weapon> pushWeapons = activeWeapons;

        if (pushType == PushType.InOrder)
        {
            //nothing -_-
        }
        else
        {
            pushWeapons = pushWeapons.OrderBy(weapon => weapon.OrderIndex).ToList();
            pushWeapons.ForEach(weapon => weapon.OrderIndexRandomize());
        }


        int weaponIndex = 0;

        foreach (Weapon weapon in pushWeapons)
        {
            weapon.Push().Forget();
            weaponIndex++;

            if (weaponIndex == pushWeaponCount)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(pushDelay));
                weaponIndex = 0;
            }
        }

        Push().Forget();

    }

    [Button(size: ButtonSizes.Large)]
    public void IncreaseAttackSpeed(float increaseVal)
    {
        rotateSpeed += rotateSpeed * (increaseVal / 100);
    }


    public void SetWeaponLevel(int newLevel)
    {
        instance.weaponLevel = newLevel;
    }

    public async UniTaskVoid DeActive(float delay = 0)
    {

        if (delay > 0)
            await UniTask.Delay(TimeSpan.FromSeconds(delay));

        float speed = rotateSpeed * 1.5f;
        
        while(rotateSpeed != 0)
        {
            rotateSpeed = Mathf.MoveTowards(rotateSpeed, 0, Time.fixedDeltaTime * speed);
            await UniTask.Delay(TimeSpan.FromSeconds(Time.fixedDeltaTime));
        }


    }

}
