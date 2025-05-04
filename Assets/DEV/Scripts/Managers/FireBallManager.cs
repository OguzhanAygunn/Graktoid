using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallManager : MonoBehaviour
{
    public static FireBallManager instance;

    [Title("Pool")]
    [SerializeField] GameObject fireBall;
    [SerializeField] List<FireBallEnemy> fireBalls;
    [SerializeField] int poolCount;


    private Transform fireBallParent;
    private void Awake()
    {
        instance = (!instance) ? this : instance;
    }

    private void Start()
    {
        fireBallParent = new GameObject("FireBall Parent").transform;
        Pool();
    }


    private void Pool()
    {
        int index = 0;

        while (index < poolCount)
        {
            index++;
            FireBallEnemy ball = Instantiate(fireBall).GetComponent<FireBallEnemy>();
            ball.transform.parent = fireBallParent;
            ball.Init();
            ball.gameObject.SetActive(false);
            fireBalls.Add(ball);
        }
    }


    public static FireBallEnemy GetFireBall()
    {
        return instance.fireBalls.Find(fireBall => !fireBall.gameObject.activeInHierarchy);
    }
}
