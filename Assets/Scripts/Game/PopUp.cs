using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUp : MonoBehaviour
{
    private string title = "";
    private string msg = "";

    public const string path = "Panel_PopUpWindow/";

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

    public static PopUp CreatePopUp(GameObject where, string m, string title = "")
    {
        where.SetActive(false);
        PopUp myC = where.AddComponent<PopUp>();
        myC.Title = title;
        myC.Msg = m;
        where.SetActive(true);

        return myC;
    }

    private void Awake()
    {
        transform.Find(path+ "Text_WindowTitle").GetComponent<Text>().text = Title;
        transform.Find(path+ "Panel_Content/Text_Description").GetComponent<Text>().text = Msg;
        transform.Find(path + "Button_WindowClose").GetComponent<Button>().onClick.AddListener(Close);

    }

    public void Close()
    {
        DestroyImmediate(gameObject);
    }


}
