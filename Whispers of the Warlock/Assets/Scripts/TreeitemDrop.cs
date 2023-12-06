using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeitemDrop : MonoBehaviour
{

    Vector3 dropLoca;
    Vector3 placeHolder = new Vector3(1,-10,0);






    public void DropItem(List<GameObject> drops)
    {
        if (drops.Count == 0)
        {
            return;
        }

        dropLoca = transform.position + placeHolder;

        int drop = Random.Range(0, drops.Count - 1);
        Instantiate(drops[drop], dropLoca, transform.rotation);
        drops.RemoveAt(drop);
    }
}
