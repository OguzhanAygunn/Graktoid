using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] int health;
    private int maxHealth;
    public int MaxHealth {  get { return health; } }

    private void Awake()
    {
        maxHealth = health;
    }

    public void TakeHit(int damage)
    {
        health -= damage;
    }

}
