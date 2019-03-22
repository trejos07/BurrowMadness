using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlot : MonoBehaviour
{

    //GUI
    Image itemImage;
    TextMeshProUGUI itemCountText;
    Image SelectedImage;

    //artibutos 
    Item itemStored=null;
    int itemCount=0;

    public static int StackCapacity;

    public Item ItemStored
    {
        get
        {
            return itemStored;
        }
    }
    public int ItemCount
    {
        get
        {
            return itemCount;
        }
    }

    private void Awake()
    {
        transform.rotation = Quaternion.LookRotation(Vector3.forward);
        itemImage = transform.Find("ItemImage").GetComponent<Image>();
        itemCountText = transform.Find("ItemCount").GetComponent<TextMeshProUGUI>();
        SelectedImage = transform.Find("SelectedImage").GetComponent<Image>();
    }

    public void AddToSlot( Item item, out bool succes)
    {
        succes = false;
        if (itemStored==null)
        {
            itemStored = item;
            itemCount++;
            UpdateGui();
            succes = true;
        }
        else if(item.itemName == itemStored.itemName )
        {
            if (itemCount < StackCapacity)
            {
                itemCount++;
                UpdateGui();
                succes = true;
            }
        }
    }

    public void CleraSlot()
    {
        itemCount = 0;
        itemStored = null;
        itemImage.sprite = null;
    }

    public void UpdateGui()
    {
        itemCountText.text = itemCount.ToString();
        itemImage.sprite = itemStored.inventorySprite;
        itemImage.color = itemStored.color;
        itemImage.enabled = itemImage.sprite!=null;
    }

    public ItemSlot SelecSlot()
    {
        SelectedImage.enabled = true;
        return this;
    }

    public void DisSelecSlot()
    {
        SelectedImage.enabled = false;
    }

    public void TurnOff()
    {
        gameObject.SetActive(! gameObject.activeSelf);
    }
}
