using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MisionSlot : MonoBehaviour
{
    MisionInfo info;

    bool active;
    Image panel;
    Image misionIcon;
    Button button;
    TextMeshProUGUI misionName;

    public Button Button
    {
        get
        {
            return button;
        }

        set
        {
            button = value;
        }
    }

    public MisionInfo Info
    {
        get
        {
            return info;
        }
    }

    public Image MisionIcon
    {
        get
        {
            return misionIcon;
        }

        set
        {
            misionIcon = value;
        }
    }

    public static MisionSlot CreateComponent(GameObject where, MisionInfo _mision)
    {
        where.name = "Slot";
        where.SetActive(false);
        MisionSlot myC = where.AddComponent<MisionSlot>();
        myC.info = _mision;
        where.SetActive(true);
        return myC;
    }

    private void Awake()
    {
        panel = GetComponent<Image>();
        button = GetComponent<Button>();
        misionIcon = transform.Find("Image").GetComponent<Image>();
        misionName = transform.Find("Mision_Name").GetComponent<TextMeshProUGUI>();
        MisionSelectionMenu.Instance.OnMisionSelected += OnSelection;
        MisionSelectionMenu.Instance.OnMisionActive += OnActive;
        if (info != null)
        {
            UpdateGui();
        }
    }

    public void OnSelection(MisionSlot m)
    {
        if (m == this && !active)
        {
            if (info.completed)
                MisionManager.Instance.ClaimMisionRedward(info);

            panel.color = Color.HSVToRGB(208f/360,1,1);
        }
        else if(!active)
            panel.color = new Color(1,1,1,0.5f);
    }
    public void OnActive(MisionSlot m)
    {
        if (m == this)
        {
            active = true;
            panel.color = Color.green;
        }
        else
        {
            panel.color = new Color(1, 1, 1, 0.5f);
            active = false;
        }
            
    }

    public void UpdateGui()
    {
        panel.color = info.completed ? Color.green : new Color(1, 1, 1, 0.5f); ;
        misionName.text = info.name;

    }

}
