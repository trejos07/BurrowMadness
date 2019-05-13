using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    Stack<GameObject> activeMenus = new Stack<GameObject>();

    GameObject messagePopUp;
    GameObject confirmationPopUp;
    GameObject sentencePopUp;
    Canvas canvas;

    public static MenuManager Instance
    {
        get;
        set;
    }


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(transform.parent);

        TurnOffAllMenus();

        canvas = GetComponentInParent<Canvas>();
        messagePopUp = Resources.Load<GameObject>("Prefabs/Ui/PopUp_Window");
        confirmationPopUp = Resources.Load<GameObject>("Prefabs/Ui/Confirm_PopUp_Window");
        sentencePopUp= Resources.Load<GameObject>("Prefabs/Ui/Dialogue_Window");
       

    }
    private void Start()
    {
        transform.Find("Main_Menu/Play_Button").GetComponent<Button>().onClick.AddListener(() => GameManager.Instance.CheckPlayerInfo());
        transform.Find("Main_Menu/Settings_Button").GetComponent<Button>().onClick.AddListener(() => OpenMenu("Settings_Menu"));
        transform.Find("Lobby_Menu/Setting_Button").GetComponent<Button>().onClick.AddListener(() => OpenMenu("Settings_Menu"));
        transform.Find("Lobby_Menu/Buttons_Panel/Worlds_Button").GetComponent<Button>().onClick.AddListener(() => OpenMenu("WorldSelection_Menu"));
        transform.Find("Lobby_Menu/Buttons_Panel/Misions_Button").GetComponent<Button>().onClick.AddListener(() => OpenMenu("Mision_Menu"));
        transform.Find("Lobby_Menu/Buttons_Panel/Shop_Button").GetComponent<Button>().onClick.AddListener(() => OpenMenu("Shop_Menu"));
        transform.Find("Lobby_Menu/Back_Button").GetComponent<Button>().onClick.AddListener(() => GoBackMenu());
        transform.Find("Settings_Menu/Settings_Window/Button_Close").GetComponent<Button>().onClick.AddListener(() => GoBackMenu());

        transform.Find("WorldSelection_Menu/WorldInfo_Panel/Play_Button").GetComponent<Button>().onClick.AddListener(() => GameManager.Instance.LoaadLevel(GameManager.Instance.SelectedWorld));
        GameManager.Instance.OnSelecWorld += (WorldInfo info) => ChangeText("WorldSelection_Menu/WorldInfo_Panel/World_Name_Text", info.name);
        OpenMenu("Main_Menu");
    }
    public void TurnOffAllMenus()
    {
        foreach (Transform child in transform)
        {
            if (transform != child)
                child.gameObject.SetActive(false);
        }
        activeMenus.Clear();
    }
    public void TurnOffActiveMenus()
    {
        while(activeMenus.Count>0)
        {
            GameObject menu = activeMenus.Pop();
            if (menu != gameObject)
                menu.SetActive(false);
        }
    }
    public PopUp DisplayMesagge(string m ,string title ="")
    {
        GameObject pop = Instantiate(messagePopUp, transform.position, Quaternion.identity, transform);
        PopUp p = PopUp.CreatePopUp(pop, m, title);
        return p;
    }
    public Message DisplaySentence(string m, bool canSkip, bool pause =false)
    {
        GameObject sentence = Instantiate(sentencePopUp, transform.position, Quaternion.identity, transform);
        Message s = Message.CreateMessage(sentence, m, canSkip);
        if(pause)
            Time.timeScale = 0;
        return s;
    }
    public ConfimationPopUp DisplayConfirmation(string m, string title = "")
    {
        GameObject pop = Instantiate(confirmationPopUp, transform.position, Quaternion.identity, transform);
        ConfimationPopUp p = ConfimationPopUp.CreatePopUp(pop, m, title);
        return p;
    }
    public void OpenMenu(string n)
    {
        GameObject menu = transform.Find(n).gameObject;
        menu.SetActive(true);
        activeMenus.Push(menu);
    }
    public void GoBackMenu()
    {
        GameObject menu = activeMenus.Pop();
        menu.SetActive(false);
        
    }
    public void ChangeText(string path, string t)
    {
        transform.Find(path).GetComponent<TextMeshProUGUI>().text = t;
    }
    public void CreateRegisterMenu()
    {
        GameObject registerMenuPrefab = Resources.Load<GameObject>("Prefabs/Ui/Register_Window");
        GameObject registerMenu = Instantiate(registerMenuPrefab, transform);
        registerMenu.transform.Find("Panel_PopUpWindow/Panel_Content/Simple_Button").GetComponent<Button>().onClick.AddListener(()=>RegisterNewPlayer(ref registerMenu));
    }
    public void RegisterNewPlayer(ref GameObject form)
    {
        string playerName= form.transform.Find("Panel_PopUpWindow/Panel_Content/InputField").GetComponent<TMP_InputField>().text;
        GameManager.Instance.CreatePlayerInfo(playerName);
        Destroy(form);
    }
    public void SetPlayerInfo()
    {
        PlayerInfo info = GameManager.Instance.PlayerInfo;
        string basePath = "Lobby_Menu/PalyerInfo_Panel/";
        ChangeText(basePath+"Player_Name_Text", info.Name);
        ChangeText(basePath+"Player_Level_Panel/lvl_Text", info.Lvl.ToString());
        ChangeText(basePath+ "Player_Coins_Panel/Display_Field/Text_Amount", info.Gold.ToString());
        ChangeText(basePath+ "Player_Gems_Panel/Display_Field/Text_Amount", info.Gems.ToString());
    }



}
