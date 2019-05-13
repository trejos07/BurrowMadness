using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Fuel : MonoBehaviour
{
    Image fill_Image;

    private void Awake()
    {
        fill_Image = transform.Find("Fill").GetComponent<Image>();
        FindObjectOfType<Player>().OnFuelChange += UpdateBar;
        
    }

    void UpdateBar(float value)
    {
        if(fill_Image!=null)
            fill_Image.fillAmount = value;
    }
}
