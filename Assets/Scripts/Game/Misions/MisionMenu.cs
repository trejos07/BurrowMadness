using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MisionMenu : MonoBehaviour
{
    Mision selectedMision;
    private List<SubMisionSlot> slots = new List<SubMisionSlot>();
    GameObject SlotPrefab;

    public Mision SelectedMision
    {
        get
        {
            return selectedMision;
        }

        set
        {
            selectedMision = value;
        }
    }

    private void Awake()
    {
        if(SlotPrefab==null)
            SlotPrefab = Resources.Load<GameObject>("Prefabs/PanelSubMision");

        SelectedMision = LoadMision();
        foreach (SubMision sub in SelectedMision.SubMisions)
        {
            Debug.Log(sub.Amount);
            GameObject _slot = Instantiate(SlotPrefab, Vector3.zero, Quaternion.identity, transform);
            SubMisionSlot slot = SubMisionSlot.CreateComponent(_slot, sub);
            slots.Add(slot);
            Debug.Log(sub.Item.itemName);
        }
        TurnOff();
        Mision.OnMisionCompleted += OnMisionComplete;

    }

    public Mision LoadMision()
    {
        if(GameManager.ins.ActiveMision!=null)
        {
            MisionInfo actvMsn=GameManager.ins.ActiveMision;
            if(GameManager.ins.SelectedWorld == GameManager.ins.GetWorldInfo(actvMsn.wname))
                return new Mision(actvMsn);
            else
            {
                WorldInfo actvWorld = GameManager.ins.SelectedWorld;
                GameManager.ins.ActiveMision = MisionManager.Instance.GetMisionFromWorld(actvWorld);
                return new Mision(GameManager.ins.ActiveMision);
            }
        }
        else
        {
            WorldInfo actvWorld = GameManager.ins.SelectedWorld;
            GameManager.ins.ActiveMision = MisionManager.Instance.GetMisionFromWorld(actvWorld);
            return new Mision(GameManager.ins.ActiveMision);
        }

    }

    public void TurnOff()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void OnMisionComplete()
    {
        string m = "Se Completo una Mision \n Ve a reclamar tu recompensa";
        MenuManager.Instance.DisplayMesagge(m, "Mision Completada");
    }

}
