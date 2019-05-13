using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Xml;

public class ItemSlot : MonoBehaviour
{
    //GUI
    Image itemImage;
    TextMeshProUGUI itemCountText;
    Image SelectedImage;

    //artibutos 
    InvSlot slot;

    public static int StackCapacity;
    public Item ItemStored
    {
        get
        {
            return slot.itemStored;
        }
    }
    public int ItemCount
    {
        get
        {
            return slot.itemCount;
        }
    }
    public InvSlot Slot
    {
        get
        {
            return slot;
        }

        set
        {
            slot = value;
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
        if (ItemStored==null)
        {
            slot.itemStored = item;
            slot.itemCount++;
            UpdateGui();
            succes = true;
        }
        else if(item.itemName == ItemStored.itemName )
        {
            if (ItemCount < StackCapacity)
            {
                slot.itemCount++;
                UpdateGui();
                succes = true;
            }
        }
    }
    public void CleraSlot()
    {
        slot.itemCount = 0;
        itemImage.sprite = null;
    }
    public void UpdateGui()
    {
        itemCountText.text = ItemCount.ToString();
        itemImage.sprite = ItemStored.inventorySprite;
        itemImage.color = ItemStored.color;
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
