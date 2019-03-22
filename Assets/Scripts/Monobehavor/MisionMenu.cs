using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MisionMenu : MonoBehaviour
{
    Mision mision;
    private List<SubMisionSlot> slots = new List<SubMisionSlot>();


    GameObject SlotPrefab;

    private void Awake()
    {
        SlotPrefab = Resources.Load("PanelSubMision") as GameObject;

        mision = new Mision();
        foreach (SubMision sub in mision.SubMisions)
        {
            Debug.Log(sub.Amount);
            GameObject _slot = Instantiate(SlotPrefab, Vector3.zero, Quaternion.identity, transform);
            SubMisionSlot slot = SubMisionSlot.CreateComponent(_slot, sub);
            slots.Add(slot);
            Debug.Log(sub.Item.itemName);
        }

        TurnOff();
    }

    public void TurnOff()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

}
