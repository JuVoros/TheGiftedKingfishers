using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    // Start is called before the first frame update
    private bool isShieldActive = false;
    private Collider shieldCollider;

    void Start()
    {

        if (shieldCollider == null)
        {
            shieldCollider = GetComponent<Collider>();
        }

        if (shieldCollider == null)
        {
            Debug.LogError("Collider not found on the shield GameObject.");
        }
    }


    void Update()
    {
        if (Input.GetButtonDown("Block"))
        {
            ToggleShield(true); 
        }
        else if (Input.GetButtonUp("Block"))
        {
            ToggleShield(false); 
        }

    }

    private void ToggleShield(bool enable)
    {
        isShieldActive = enable;

        if(shieldCollider != null )
        {
            shieldCollider.enabled = enable;
        }

        if (isShieldActive)
        {
            Debug.Log("Shield is now active.");
        }
        else
        {
            Debug.Log("Shield is now inactive.");
        }
    }

    public bool IsShieldActive()
    {
        return isShieldActive;
    }
}
