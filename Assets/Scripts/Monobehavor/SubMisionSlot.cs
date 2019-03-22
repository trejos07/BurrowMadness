using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class SubMisionSlot : MonoBehaviour
{
    //GUI
    Image itemImage;
    Image SelectedImage;
    TextMeshProUGUI itemCountText;
    TextMeshProUGUI itemName_Text;


    //artibutos 
    SubMision subMision;
    

    public static int StackCapacity;


    public Item ItemStored
    {
        get
        {
            return subMision.Item;
        }
    }
    public int ItemCount
    {
        get
        {
            return subMision.Amount;
        }
    }

    public SubMision SubMision
    {
        get
        {
            return subMision;
        }

        set
        {
            subMision = value;
        }
    }

    public static SubMisionSlot CreateComponent(GameObject where, SubMision _subMision)
    {
        SubMisionSlot myC = where.AddComponent<SubMisionSlot>();
        Debug.Log(_subMision.ItemName+" "+ _subMision.Amount);
        myC.subMision = _subMision;
        return myC;
    }

    private void Start()
    {
        transform.rotation = Quaternion.LookRotation(Vector3.forward);
        itemImage = (transform.Find("ItemImage").GetComponent<Image>());
        itemCountText = transform.Find("ItemCount").GetComponent<TextMeshProUGUI>();
        SelectedImage = transform.Find("SelectedImage").GetComponent<Image>();
        itemName_Text = transform.Find("ItenmNameText").GetComponent<TextMeshProUGUI>();

        UpdateGui();
    }

    
    public void CleraSlot()
    {
        itemImage.sprite = null;
        itemCountText.text = "0";
        itemName_Text.text = "";
    }

    public void UpdateGui()
    {
        itemCountText.text = ItemCount.ToString();
        itemImage.sprite = ItemStored.inventorySprite;
        itemImage.color = ItemStored.color;
        itemImage.enabled = itemImage.sprite != null;
        itemName_Text.text = ItemStored.itemName;
    }

    public SubMisionSlot SelecSlot()
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
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
