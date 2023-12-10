using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{

    GameObject canvas;
    float disappearTime;
    Color textColor;
    TextMeshPro textMesh;

    public static DamagePopup Create(Vector3 position, int damageAmount)
    {
        
        GameObject damagePopupTransform = Instantiate(gameManager.instance.pfDamagePopup, position, Quaternion.identity);
        Debug.Log("Create");

        DamagePopup damagePopup = damagePopupTransform.GetComponentInChildren<DamagePopup>();
        if (damagePopup != null )
            damagePopup.Setup(damageAmount);

        return damagePopup;
    }
    private void Awake()
    {
        textMesh = transform.GetComponentInChildren<TextMeshPro>();
        Debug.Log(textMesh.text);
        Debug.Log("Awake");
    }
    public void Setup(int damageAmount)
    {

        textMesh.text = damageAmount.ToString();
        Debug.Log(textMesh.text);

        textColor = textMesh.color;
        disappearTime = 1f;
        Debug.Log("Setup");

    }
    private void Update()
    {

        float moveYSpeed = 20f;
        transform.position += new Vector3(0, moveYSpeed) * Time.deltaTime;
        Debug.Log("Update");

        disappearTime -= Time.deltaTime;
        if (disappearTime < 0)
        {
            float disappearSpeed = 3f;
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;

            if (textColor.a < 0)
            {
                Destroy(gameObject);

            }


        }

    }
}
