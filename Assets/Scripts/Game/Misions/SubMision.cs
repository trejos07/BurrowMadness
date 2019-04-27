using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using System;
using System.Xml.Serialization;

public class SubMision
{
    //gui
    SubMisionSlot slot;


    //Atributos
    subMisionInfo info;
    Item item;
    int progres;

    #region Accesores
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
            return info.Amount;
        }
    }
    public string ItemName
    {
        get
        {
            return item.itemName;
        }
        
    }
    public int Redward
    {
        get
        {
            return info.Redward;
        }
    }
    public int Progres
    {
        get
        {
            return progres;
        }

        set
        {
            progres = Mathf.Clamp(value,0,Amount);
            Completed = progres == Amount;
                
        }
    }
    public bool Completed
    {
        get
        {
            return info.completed;
        }

        set
        {
            info.completed = value;
            if (OnCompleted != null && info.completed)
            {
                OnCompleted();
                Debug.Log("se completo la subMision : " + item.itemName);
            }
        }
    }
    #endregion

    //eventos
    public delegate void CompletedSubMision();
    public event CompletedSubMision OnCompleted;//disparado en el accesor Completed

    //Constructor
    public SubMision(subMisionInfo info)
    {
        this.info = info;
        Item item = Item.ItemsList.Where(i => i.itemName.ToLower() == info.ItemName.ToLower()).FirstOrDefault();
        this.item = item;
        progres = 0;
    }

}
