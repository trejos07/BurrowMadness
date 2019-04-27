using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class SubMisionSlot : MonoBehaviour
{
    //GUI
    Image panel;
    Image itemImage;
    Image SelectedImage;
    TextMeshProUGUI itemCountText;
    TextMeshProUGUI itemName_Text;
    TextMeshProUGUI redward_Text;


    //artibutos 
    SubMision subMision;

    public static int StackCapacity;


    #region Accesores
    public Item ItemStored
    {
        get
        {
            return subMision.Item;
        }
    }
    public int ItemAmount
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
    #endregion

    public static SubMisionSlot CreateComponent(GameObject where, SubMision _subMision)
    {
        where.name = "Slot";
        SubMisionSlot myC = where.AddComponent<SubMisionSlot>();
        Debug.Log(_subMision.ItemName+" "+ _subMision.Amount);
        myC.subMision = _subMision;
        _subMision.OnCompleted += myC.OnSubMisionCompleted;
        return myC;
    }


    private void Start()
    {
        
        panel = GetComponent<Image>();
        transform.rotation = Quaternion.LookRotation(Vector3.forward);
        itemImage = (transform.Find("ItemSlot/ItemImage").GetComponent<Image>());
        itemCountText = transform.Find("ItemSlot/ItemCount").GetComponent<TextMeshProUGUI>();
        SelectedImage = transform.Find("ItemSlot/SelectedImage").GetComponent<Image>();
        itemName_Text = transform.Find("ItenmNameText").GetComponent<TextMeshProUGUI>();
        redward_Text = transform.Find("RedwardImg/RedwardText").GetComponent<TextMeshProUGUI>();
        UpdateGui();
    }
    public void OnSubMisionCompleted()
    {
        panel.color = Color.green*new Vector4(1,1,1,0.4f);
        
    }
    public void CleraSlot()
    {
        itemImage.sprite = null;
        itemCountText.text = "0";
        itemName_Text.text = "";
    }
    public void UpdateGui()
    {
        itemCountText.text = ItemAmount.ToString();
        itemImage.sprite = ItemStored.inventorySprite;
        itemImage.color = ItemStored.color;
        itemImage.enabled = itemImage.sprite != null;
        itemName_Text.text = ItemStored.itemName;
        redward_Text.text = subMision.Redward.ToString();
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
