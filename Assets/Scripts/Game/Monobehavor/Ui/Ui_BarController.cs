﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ui_BarController : MonoBehaviour
{
    Image fill_Image;
    public BarTypes type;

    public enum BarTypes { Player, Enemy }

    private void Awake()
    {
        fill_Image = transform.Find("Fill").GetComponent<Image>();
        if(type == BarTypes.Player)
            FindObjectOfType<Player>().OnHealthChange += UpdateBar;
        if(type == BarTypes.Enemy)
            transform.parent.GetComponentInParent<Character>().OnHealthChange += UpdateBar; //Herachy/enemmy/canvas/this
    }

    void UpdateBar(float value)
    {
        if(fill_Image!=null)
            fill_Image.fillAmount = value;
    }


}
