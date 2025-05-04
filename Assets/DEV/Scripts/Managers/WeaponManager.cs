using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager instance;
    [Title("Pool")]
    [SerializeField] GameObject prefab;
    [SerializeField] int count;
    [SerializeField] List<Weapon> allWeapons;

    [Title("Start")]
    [SerializeField] int startWeaponCount;
    [SerializeField] int weaponsLevel;


    private WeaponsController weaponsController;
    private void Awake()
    {
        instance = (!instance) ? this : instance;
        
    }

    private void Start()
    {
        weaponsController = WeaponsController.instance;
        Pool();
    }


    private void Pool()
    {
        while(count > 0)
        {
            count--;
            Weapon weapon = Instantiate(prefab).GetComponent<Weapon>();
            //weapon.SetWeapon(startWeaponLevels);
            allWeapons.Add(weapon);
            weapon.transform.parent = weaponsController.transform;
            weapon.gameObject.SetActive(false);
        }
    }

    public static Weapon GetWeapon()
    {
        return instance.allWeapons.Find(weapon => weapon.gameObject.activeInHierarchy);
    }

    public static List<Weapon> GetWeapons(int count)
    {
        List<Weapon> weapons = new List<Weapon>();
        foreach(Weapon w in instance.allWeapons)
        {
            if (!w.gameObject.activeInHierarchy)
            {
                weapons.Add(w);
                count--;

                if (count == 0)
                    break;
            }
        }

        return weapons;
    }
}
