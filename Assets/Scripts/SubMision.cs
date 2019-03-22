using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using System;

public class SubMision
{
    //gui
    SubMisionSlot slot;


    //Atributos
    string itemName;
    Item item;
    int amount;
    bool completed = false;

    #region Accesores
    public bool Completed
    {
        get
        {
            return completed;
        }

        set
        {
            completed = value;
            if (OnCompleted != null && completed)
            {
                OnCompleted(this);
                Debug.Log("se completo la subMision : " + item.itemName);
            }

        }
    }
    public Item Item
    {
        get
        {
            return item;
        }
    }
    public int Amount
    {
        get
        {
            return amount;
        }
    }
    public string ItemName
    {
        get
        {
            return itemName;
        }

        set
        {
            itemName = value;
        }
    }
    #endregion

    //eventos
    public delegate void CompletedSubMision(SubMision subMision);
    public static event CompletedSubMision OnCompleted;//disparado en el accesor Completed

    //Constructor
    public SubMision(string _item, int amount)
    {
        itemName = _item.ToLower();
        Item item = Item.ItemsList.Where(i => i.itemName.ToLower() == itemName).FirstOrDefault();

        if (item != null)
            this.item = item;
        else
            item = null;

        this.amount = amount;
        completed = false;
    }
}


[System.Serializable]
public class subMisionInfo
{
    public string ItemName;
    public int Amount;
}

[System.Serializable]
public class subMisionDataBase
{
    public static subMisionDataBase ins;
    public List<subMisionInfo> PosibleSubMisions = new List<subMisionInfo>();

    private void Awake()
    {
        ins = this;
    }
}