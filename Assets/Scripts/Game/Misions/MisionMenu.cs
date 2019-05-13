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

            GameObject _slot = Instantiate(SlotPrefab, Vector3.zero, Quaternion.identity, transform);
            SubMisionSlot slot = SubMisionSlot.CreateComponent(_slot, sub);
            slots.Add(slot);
        }
        TurnOff();
        Mision.OnMisionCompleted += OnMisionComplete;

    }

    public Mision LoadMision()
    {
        if(GameManager.Instance.ActiveMision!=null)
        {
            MisionInfo actvMsn=GameManager.Instance.ActiveMision;
            if(GameManager.Instance.SelectedWorld == GameManager.Instance.GetWorldInfo(actvMsn.wname))
                return new Mision(actvMsn);
            else
            {
                WorldInfo actvWorld = GameManager.Instance.SelectedWorld;
                GameManager.Instance.ActiveMision = MisionManager.Instance.GetMisionFromWorld(actvWorld);
                return new Mision(GameManager.Instance.ActiveMision);
            }
        }
        else
        {
            WorldInfo actvWorld = GameManager.Instance.SelectedWorld;
            GameManager.Instance.ActiveMision = MisionManager.Instance.GetMisionFromWorld(actvWorld);
            return new Mision(GameManager.Instance.ActiveMision);
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
