using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;



public class Mision
{
    //atributos
    private int dificulty = 2;
    private List<SubMision> subMisions = new List<SubMision>();
    private bool completed = false;
    private int reward = 0;

    //eventos
    public delegate void MisionCompleted();
    public static event MisionCompleted OnMisionCompleted;//disparado en el accesor Completed

    #region Accesores
    public List<SubMision> SubMisions
    {
        get
        {
            return subMisions;
        }

        set
        {
            subMisions = value;
        }
    }
    public bool Completed
    {
        get
        {
            return completed;
        }

        set
        {
            completed = value;
            if (OnMisionCompleted != null && completed)
                OnMisionCompleted();
        }
    }

    public int Dificulty
    {
        get
        {
            return dificulty;
        }

        set
        {
            dificulty = Mathf.Clamp(value, 2, 5);
        }
    }


    #endregion

    public Mision()
    {
        List<subMisionInfo> _PosibleSubMisions = XMLManager.ins.LoadSubmisions().PosibleSubMisions;
        //Debug.Log(subMisionDataBase.PosibleSubMisions[0].ItemName);
        if (_PosibleSubMisions.Count != 0)
        {
            for (int i = 0; i < dificulty; i++)//genero cantidad de misiones de a cuerdo a la dificultad 
            {
                subMisionInfo subMision = _PosibleSubMisions[UnityEngine.Random.Range(0, _PosibleSubMisions.Count)];
                subMisions.Add(new SubMision(subMision.ItemName, subMision.Amount));//las misiones se escogen aleatorias de una lista general (statica) 
            }
            Debug.Log("se creo una mision con " + subMisions.Count + " submisiones");

        }

    }

    void Test()
    {
        //posibleSubMisions = new subMisionData[dificulty];
        //posibleSubMisions[0] = new subMisionData("cobalto", 3);
        //posibleSubMisions[1] = new subMisionData("latem", 2);
        //string jsonOut = JsonUtility.ToJson(posibleSubMisions);
        //Debug.Log(jsonOut);

    }
}
