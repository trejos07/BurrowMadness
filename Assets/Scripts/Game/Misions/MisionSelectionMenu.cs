using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MisionSelectionMenu : MonoBehaviour
{

    public static MisionSelectionMenu Instance;

    [SerializeField] GameObject misionsViewPort;
    GameObject worldsMisionsPanel;
    GameObject misionSlot;
    GameObject subMisionSlot;

    MisionSlot selectedSlot;

    Image misionIcon;
    Transform subMisionPanel;
    TextMeshProUGUI misionName;

    public MisionInfo SelectedMision
    {
        get
        {
            return selectedSlot.Info;
        }
    }

    public MisionSlot SelectedSlot
    {
        get
        {
            return selectedSlot;
        }

        set
        {
            selectedSlot = value;
            UpdateSelectedMision();
        }
    }

    public delegate void MisionSlotOparation(MisionSlot slot);
    public event MisionSlotOparation OnMisionSelected;
    public event MisionSlotOparation OnMisionActive;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        subMisionPanel = transform.Find("MisionInfo_Panel/SubMisions_Details_Panel");
        misionName = transform.Find("MisionInfo_Panel/Mision_Name_Text").GetComponent<TextMeshProUGUI>();
        misionIcon = transform.Find("MisionInfo_Panel/Mision_Image").GetComponent<Image>();
        transform.Find("MisionInfo_Panel/Active_Button").GetComponent<Button>().onClick.AddListener(ActiveMision);
        worldsMisionsPanel = Resources.Load<GameObject>("Prefabs/WorldMisions_Panel");
        misionSlot = Resources.Load<GameObject>("Prefabs/Mision_Panel");
        subMisionSlot = Resources.Load<GameObject>("Prefabs/PanelSubMision");
        MisionManager.Instance.OnInfoChange += SetWorldSMisionsPanels;

    }

    private void Start()
    {
        SetWorldSMisionsPanels();

    }

    //public void Create()
    public void SetWorldSMisionsPanels()
    {
        foreach (Transform child in misionsViewPort.transform)
        {
            if (child != misionsViewPort.transform)
            {
                Destroy(child.gameObject);
            }
        }

        foreach (List<MisionInfo> lm in MisionManager.Instance.MisionsPerWorld)
        {
            int i = MisionManager.Instance.MisionsPerWorld.IndexOf(lm);
            WorldInfo world = GameManager.ins.Worlds[i];
            GameObject wMisionPanel= Instantiate(worldsMisionsPanel, misionsViewPort.transform);
            wMisionPanel.transform.Find("Mision_Name_Text").GetComponent<TextMeshProUGUI>().text = world.name+" Misions";
            wMisionPanel.transform.Find("World_Image").GetComponent<Image>();//Por implentar
            Transform misionsPanel= wMisionPanel.transform.Find("Misions_Details_Panel");

            foreach (MisionInfo m in lm)
            {
                GameObject ms = Instantiate(misionSlot, misionsPanel);
                MisionSlot slot = MisionSlot.CreateComponent(ms, m);
                slot.Button.onClick.AddListener(()=> {
                    SelectedSlot = slot;
                    if(OnMisionSelected!= null)
                        OnMisionSelected(slot); 
                });
                
            }

        }
        selectedSlot = null;
        UpdateSelectedMision();
    }
    public void UpdateSelectedMision()
    {
        misionName.text = "";
        foreach (Transform child in subMisionPanel)
        {
            if (child != subMisionPanel)
            {
                Destroy(child.gameObject);
            }
        }


        if(selectedSlot!= null)
        {
            misionName.text = selectedSlot.name;
            //misionIcon.sprite =

            foreach (subMisionInfo si in SelectedMision.SubMisions)
            {
                GameObject _slot = Instantiate(subMisionSlot, Vector3.zero, Quaternion.identity, subMisionPanel);
                SubMisionSlot slot = SubMisionSlot.CreateComponent(_slot, new SubMision(si));

            }
        }





    }
    public void ActiveMision()
    {
        GameManager.ins.ActiveMision = SelectedMision;
        if (OnMisionActive != null)
            OnMisionActive(SelectedSlot);
    }


}
