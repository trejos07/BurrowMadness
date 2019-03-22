using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct Equivalency 
{

    Item item;
    float[] convertionPrice;
    Item[] toItem;

    static List<Equivalency> equivalencies = new List<Equivalency>();

    public Equivalency(Item item, float[] convertionPrice) : this()//por que se pone esta referencia a mi mismo ?
    {
        this.item = item;
        this.convertionPrice = convertionPrice;
        toItem = Item.ItemsList.ToArray();

        if (equivalencies == null)
            equivalencies = new List<Equivalency>();

        equivalencies.Add(this);

    }
    
}
