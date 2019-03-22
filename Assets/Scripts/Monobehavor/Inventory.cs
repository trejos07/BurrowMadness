using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.EventSystems;
using System.Linq;

public class Inventory : MonoBehaviour 
{
    GameObject SlotPrefab;

    //atributos 
    [SerializeField] Vector2 size= new Vector2(3,4);
    [SerializeField] float spaceBtwn=1;

    int ItemsCapacity;
    public int stackCapacity = 5;

    List<ItemSlot> slots = new List<ItemSlot>();
    ItemSlot currentSelectedSlot;

    GraphicRaycaster m_Raycaster;
    EventSystem m_EventSystem;

    public delegate void ItemSelected(Item item);
    public event ItemSelected OnItemSelected;


    void Awake()
    {
        SlotPrefab = Resources.Load("ItemSlot") as GameObject;

        m_Raycaster = FindObjectOfType<GraphicRaycaster>();
        m_EventSystem = FindObjectOfType<EventSystem>();

        Item.OnItemPickedUp += StoredValidation;

        ItemsCapacity = Mathf.FloorToInt(size.x * size.y);
        ItemSlot.StackCapacity = stackCapacity;

        GenerateSlots();
        DisSelectSlot();
        TurnSlots();

    }

    public int CountEmptySlots()
    {
        int empty = 0;
        for (int i = 0; i < slots.Count; i++)
        {
            if(slots[i]!=null)
            {
                if (slots[i].ItemStored == null)
                    empty++;
            }
        }
        return empty;
    }

    public void GenerateSlots()
    {
        for (int i = 0; i < ItemsCapacity; i++)
        {
            GameObject _slot = Instantiate(SlotPrefab, Vector3.zero, Quaternion.identity, transform);
            _slot.AddComponent<ItemSlot>();
            slots.Add(_slot.GetComponent<ItemSlot>());
        }
    }
    

    public void TurnSlots()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].TurnOff();
        }
    }

    public void SelectItem()
    {
        DisSelectSlot();
        TurnSlots();//muestro el inventario 
        StartCoroutine(SelectSlot());
    }

    public void DisSelectSlot()
    {
        currentSelectedSlot = null;
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].DisSelecSlot();
        }
    }

    public IEnumerator SelectSlot()
    {
        currentSelectedSlot = null;
        PointerEventData m_PointerEventData;
        m_PointerEventData = new PointerEventData(m_EventSystem);

        List<RaycastResult> results = new List<RaycastResult>();

        while (true)
        {
            if(Input.touchCount>0|| Input.GetMouseButtonDown(0))
            {
                m_PointerEventData.position = Input.mousePosition;
                m_Raycaster.Raycast(m_PointerEventData, results);

                foreach (RaycastResult result in results)
                {
                    for (int i = 0; i < slots.Count; i++)
                    {
                        if(slots[i].transform == result.gameObject.transform)
                        {
                            currentSelectedSlot = slots[i].SelecSlot();
                        }
                    }
                }

                /*Input.touches[0].phase == TouchPhase.Ended */
                if (currentSelectedSlot!=null)
                { 
                    if (OnItemSelected != null)
                        OnItemSelected(slots[slots.IndexOf(currentSelectedSlot)].ItemStored);
                    TurnSlots();
                    break;
                }
                    
            }

            yield return null;
        }
    }

    public void StoredValidation(Item itemToAdd)
    {
        bool canStore = false;
        Debug.Log("evalundo el item " + itemToAdd.itemName);

        if (CountEmptySlots() > 0)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i] != null)
                {
                    slots[i].AddToSlot(itemToAdd, out canStore);//mira si puede almacenar el item y retorna true si completo la operacion 

                    if (canStore)//si completo la operacion 
                        break;
                }
            }
        }
            

        if (canStore)
            Debug.Log("se agrego" + itemToAdd.itemName);

    }

    public void RemoveSelectedItem(Item itemToRemove)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i] != null)
                slots[i].CleraSlot();
        }
    }

}
