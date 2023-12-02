using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Handplacement))]
public class Headbob_System : MonoBehaviour
{

    public float EffectIntesity;
    public float EffectIntesityX;
    public float EffectSpeed;

    private Handplacement FollowerInstance;
    private Vector3 OriginalOffset;
    private float SinTime;

    void Start()
    {
        FollowerInstance = GetComponent<Handplacement>();
        OriginalOffset = FollowerInstance.Offset;

    }

    void Update()
    {
        Vector3 inputVector = new Vector3(Input.GetAxis("Vertical"), 0f, Input.GetAxis("Horizontal"));

        if(inputVector.magnitude > 0f)
        {
            SinTime += Time.deltaTime * EffectSpeed;
        }
        else
        {
            SinTime = 0f;
        }
        float sinAmountY = -Mathf.Abs(EffectIntesity * Mathf.Sin(SinTime));
        Vector3 sinAmountX = FollowerInstance.transform.right * EffectIntesity * Mathf.Cos(SinTime) * EffectIntesityX;
        
        FollowerInstance.Offset = new Vector3
        {
            x = OriginalOffset.x,
            y = OriginalOffset.y + sinAmountY,
            z = OriginalOffset.z,
        };
        FollowerInstance.Offset += sinAmountX;
    }

}
