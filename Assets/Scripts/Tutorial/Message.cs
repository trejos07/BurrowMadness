using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Message : MonoBehaviour
{
    bool nextButton;
    private string msg = "";
    Button next;

    public const string path = "Panel_PopUpWindow/";

    #region Accesores
    
    public string Msg
    {
        get
        {
            return msg;
        }

        set
        {
            msg = value;
        }
    }

    public bool NextButton
    {
        get
        {
            return nextButton;
        }

        set
        {
            nextButton = value;
        }
    }

    public Button Next
    {
        get
        {
            return next;
        }

        set
        {
            next = value;
        }
    }
    #endregion

    public delegate void CloseCallback();
    public event CloseCallback OnClose;

    public static Message CreateMessage(GameObject where, string m, bool nextButton)
    {
        where.SetActive(false);
        Message myC = where.AddComponent<Message>();
        myC.Msg = m;
        myC.NextButton = nextButton;
        where.SetActive(true);

        return myC;
    }

    private void Awake()
    {
        transform.Find(path + "Panel_Content/Text_Description").GetComponentInChildren<TextMeshProUGUI>().text = Msg;
        next = transform.Find(path + "Panel_Content/Button_Next").GetComponent<Button>();
        if (NextButton)
            next.onClick.AddListener(Close);
        else
           next.gameObject.SetActive(false);

    }

    public void Close()
    {
        if (OnClose != null)
            OnClose();
        DestroyImmediate(gameObject);
    }

}
