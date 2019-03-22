using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exchanger : MonoBehaviour
{
  
    Dictionary<Item, Equivalency> mEquivalencies;

    void SetMyEquivalencies()
    {

    }

    private void OnMouseDown()
    {
        Inventory inventory = FindObjectOfType<Inventory>();
        inventory.OnItemSelected += LookForEquivalency;
        inventory.SelectItem();
    }

    void LookForEquivalency(Item item)
    {
        FindObjectOfType<Inventory>().OnItemSelected -= LookForEquivalency;



    }

    void checkForUnitariExchange()
    {

    }

}
