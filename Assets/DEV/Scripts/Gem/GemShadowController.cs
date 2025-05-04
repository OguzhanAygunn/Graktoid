using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemShadowController : MonoBehaviour
{
    
    [Title("Main")]
    [SerializeField] GemController controller;
    [SerializeField] Vector3 startScale;
    [SerializeField] Vector3 endScale;
    [SerializeField] float maxDistace;


    private void FixedUpdate()
    {

        //Pos
        Vector3 targetPos = controller.SpriteTrs.position;
        Vector3 pos = transform.position;

        targetPos.y = pos.y;
        transform.position = Vector3.Lerp(pos, targetPos, Time.fixedDeltaTime * 50);

        //Scale
        float distance = Vector2.Distance(transform.position, controller.SpriteTrs.position);
        Vector3 scale = Vector3.Slerp(startScale, endScale, distance / maxDistace);
        transform.localScale = scale;
    }
}
