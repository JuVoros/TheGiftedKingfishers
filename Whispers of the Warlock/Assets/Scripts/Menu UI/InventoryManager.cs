using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    public List<Item> items = new List<Item>();
    public List<Item> hpItems = new List<Item>();
    public List<Item> manaItems = new List<Item>();

    public Transform itemContent;
    public GameObject inventoryItem;

    private void Awake()
    {
        
        Instance = this;

    }
    private void Update()
    {
        if (Input.GetButtonDown("Inventory"))
            ListItem();
    }


    public void Add(Item item)
    {

        items.Add(item);

    }
    public void Remove(Item item)
    {

        items.Remove(item);
    }

    public void ListItem()
    {
        foreach(Transform item in itemContent)
        {

            Destroy(item.gameObject);
        }

        foreach (Item item in items)
        {
            GameObject obj = Instantiate(inventoryItem, itemContent);
            
            var itemName = obj.transform.Find("ItemName").GetComponent<Text>();
            var itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();

            itemName.text = item.itemName;
            itemIcon.sprite = item.itemIcon;
        }
    }
   
}
