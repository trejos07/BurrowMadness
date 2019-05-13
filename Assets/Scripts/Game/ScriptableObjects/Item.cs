using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu]
public class Item : ScriptableObject
{
    private static List<Item> itemsList = new List<Item>();
    private int id;
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
        set
        {
            itemsList = value;
        }

    }
    public int Id
    {
        get
        {
            return id;
        }
        
    }


    public static Item ChanceRandomItem()
    {
        List<Item> ItemsList = new List<Item>(itemsList);
        ItemsList.OrderBy(x => x.chance).ToArray();
        Item item = null;

        int chance = Random.Range(1, 70);

        for (int i = 0; i < ItemsList.Count; i++)
        {
            if (chance <= ItemsList[i].chance)
            {
                item = ItemsList[i];
                break;
            }
        }
        return item;

    }
    public static Item ChanceRandomItem(Item[] ItemsList)
    {
        ItemsList = ItemsList.OrderBy(x => x.chance).ToArray();
        Item item = null;

        int chance = Random.Range(1, 70);

        for (int i = 0; i < ItemsList.Length; i++)
        {
            if(ItemsList[i]!=null)
            {
                if (chance <= ItemsList[i].chance)
                {
                    item = ItemsList[i];
                    break;
                }
            }
        }
        return item;

    }
    public static int[] IdsFormItems(Item[] items)
    {
        int[] ids = new int[items.Length];
        for (int i = 0; i < items.Length; i++)
        {
            ids[i] = items[i].id;
        }
        return ids;
    }
    public static Item[] ItemsFromIds(int[] items)
    {
        Item[] ids = new Item[items.Length];
        for (int i = 0; i < items.Length; i++)
        {
            ids[i] = itemsList[items[i]-1];
        }
        return ids;
    }

    private void Awake()
    {
        if (itemsList != null && !itemsList.Contains(this))
            itemsList.Add(this);
        id = itemsList.IndexOf(this)+1;

    }
    private void OnDestroy()
    {
        itemsList.Remove(this);
    }
    public static Item GetItemOfID(int id)
    {
        Item item = null;
        for (int i = 0; i < itemsList.Count; i++)
        {
            if (itemsList[i].id == id)
            {
                item = itemsList[i];
            }
        }
        return item;
    }
    public void Piked()
    {
        if (OnItemPickedUp != null)
            OnItemPickedUp(this);
        
    }

}
