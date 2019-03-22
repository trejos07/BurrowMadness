using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Item : ScriptableObject
{
    private static List<Item> itemsList = new List<Item>();
    public string itemName;
    public Sprite groundSprite;
    public Sprite inventorySprite;
    public Color color;
    [Range(0,1)]
    public float hardness;
    public int deathLimit;
    public int birthLimit;
    public int chance;



    public delegate void ItemPickedUp( Item item);
    public static event ItemPickedUp OnItemPickedUp;

    public static List<Item> ItemsList
    {
        get
        {
            return itemsList;
        }
    }

    private void OnEnable()
    {
        if (itemsList != null && !itemsList.Contains(this))
            itemsList.Add(this);
    }

    public void Piked()
    {
        if (OnItemPickedUp != null)
            OnItemPickedUp(this);
        
    }

}
