using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    PlayerController controller;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Vector3 effectSpawnPoint = collision.contacts[0].point;

            EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();
            controller.TakeHit(enemy: enemy.transform, damage: 10, effectSpawnPoint);
        }
    }
}
