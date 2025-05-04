using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyBoss : MonoBehaviour
{

    [Title("Events")]
    [SerializeField] UnityEvent killEvents;

    public void Kill()
    {
        CameraController.PlayShake("SmallExplosion");
        killEvents.Invoke();
    }
}
