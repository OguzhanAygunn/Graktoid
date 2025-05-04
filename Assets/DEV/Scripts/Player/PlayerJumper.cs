using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumper : MonoBehaviour
{
    [Title("Main")]
    [SerializeField] bool active;
    [SerializeField] float duration;
    [SerializeField] float counter;

    PlayerMovement movement;

    private void Start()
    {
        movement = PlayerController.instance.Movement;
    }

    private void Update()
    {
        CounteController();
    }

    private void CounteController()
    {
        if (active)
            return;

        counter = Mathf.MoveTowards(counter, duration, Time.deltaTime);

        if(counter == duration)
        {
            Jump();
        }
    }


    private void Jump()
    {
        movement.Jump();
        counter = 0;
    }

    public void SetActive(bool active)
    {
        this.active = active;
    }
}
