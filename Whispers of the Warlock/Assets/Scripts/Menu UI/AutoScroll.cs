using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class AutoScroll : MonoBehaviour
{
    [SerializeField] float speed;


    float textPosBegin = -830.0f;
    float boundryTextEnd = 830.0f;

    RectTransform myGORectTrans;
    [SerializeField] TextMeshProUGUI mainText;
    [SerializeField] bool isLooping = true;

    private void Start()
    {
        myGORectTrans = gameObject.GetComponent<RectTransform>();
        StartCoroutine(AutoScrolltext());
    }
    IEnumerator AutoScrolltext()
    {
        while(myGORectTrans.localPosition.y < boundryTextEnd)
        {
            myGORectTrans.Translate(Vector3.up * speed * Time.deltaTime);
            if(myGORectTrans.localPosition.y > boundryTextEnd )
            {
                if (isLooping)
                {
                    myGORectTrans.localPosition = Vector3.up * textPosBegin;
                }
                else
                {
                    break;
                }
                   
            }            
            yield return null;
        }
    }

}
