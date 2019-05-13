using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConfimationPopUp : MonoBehaviour
{
    private string title = "";
    private string msg = "";
    public const string path = "Panel_PopUpWindow/";

    public event Action OnCancel;
    public event Action OnConfirm;

    #region Accesores
    public string Title
    {
        get
        {
            return title;
        }

        set
        {
            title = value;
        }
    }

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
    #endregion

    public static ConfimationPopUp CreatePopUp(GameObject where, string m, string title = "")
    {
        where.SetActive(false);
        ConfimationPopUp myC = where.AddComponent<ConfimationPopUp>();
        myC.Title = title;
        myC.Msg = m;
        where.SetActive(true);

        return myC;
    }

    private void Awake()
    {
        transform.Find(path + "Text_WindowTitle").GetComponent<Text>().text = Title;
        transform.Find(path + "Panel_Content/Text_Description").GetComponent<TextMeshProUGUI>().text = Msg;
        transform.Find(path + "Panel_Content/Buttons/Cancel_Button").GetComponent<Button>().onClick.AddListener(Cancel);
        transform.Find(path + "Panel_Content/Buttons/Confirm_Button").GetComponent<Button>().onClick.AddListener(Confirm);
    }

    public void Cancel()
    {
        if (OnCancel!=null)
        {
            OnCancel();
        }
        DestroyImmediate(gameObject);
    }
    public void Confirm()
    {
        if (OnConfirm != null)
        {
            OnConfirm();
        }
        DestroyImmediate(gameObject);
    }


}