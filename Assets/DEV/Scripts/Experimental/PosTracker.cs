using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosTracker : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Vector3 offset;

    private void Awake()
    {
        if (offset == Vector3.zero)
            offset = transform.position - target.position;
    }

    public void Init()
    {
        if (!target)
            target = transform.parent;


        if (offset == Vector3.zero)
            offset = transform.position - target.position;

        transform.parent = null;

        transform.eulerAngles = Vector3.right * 45;

    }


    private void FixedUpdate()
    {
        Movement();
    }


    private void Movement()
    {
        transform.position = target.position + offset;
    }
}
