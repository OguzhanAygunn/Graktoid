using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public enum DamageAreaType { Enemy,Player,Both}
public class AreaDamageTrigger : MonoBehaviour
{
    [SerializeField] DamageAreaType type = DamageAreaType.Enemy;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] float radius;
    [SerializeField] float triggerDelay;
    [SerializeField] List<EnemyController> enemies;

    private void Start()
    {
    }

    [Button(size: ButtonSizes.Large)]
    public async UniTaskVoid Trigger(float delay = 0)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delay));

        enemies.Clear();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, enemyLayer);


        if(colliders.Length > 0)
        {
            colliders.ForEach(coll => enemies.Add(coll.GetComponentInParent<EnemyController>()));
            enemies.ForEach(enemy => enemy.TakeHit(damage: 25));

        }
    }
}
