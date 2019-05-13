using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Mision
{
    //atributos.
    MisionInfo info;
    private List<SubMision> subMisions = new List<SubMision>();

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
            return info.completed;
        }

        set
        {
            info.completed = value;
            if (OnMisionCompleted != null && value)
                OnMisionCompleted();
        }
    }
    public int Dificulty
    {
        get
        {
            return info.dificulty;
        }

        set
        {
            info.dificulty = Mathf.Clamp(value, 2, 5);
        }
    }
    public MisionInfo Info
    {
        get
        {
            return info;
        }
    }
    #endregion

    public Mision()
    {


        //List<subMisionInfo> _PosibleSubMisions = XMLManager.LoadData<subMisionDataBase>(XMLManager.SUBMISIONS_FILE_NAME).PosibleSubMisions;
        ////Debug.Log(subMisionDataBase.PosibleSubMisions[0].ItemName);
        //if (_PosibleSubMisions.Count != 0)
        //{
        //    for (int i = 0; i < dificulty; i++)//genero cantidad de misiones de a cuerdo a la dificultad 
        //    {
        //        subMisionInfo subMision = _PosibleSubMisions[UnityEngine.Random.Range(0, _PosibleSubMisions.Count)];
        //        subMisions.Add(new SubMision(subMision.ItemName, subMision.Amount));//las misiones se escogen aleatorias de una lista general (statica) 
        //    }
        //    Debug.Log("se creo una mision con " + subMisions.Count + " submisiones");

        //}

        //Inventory.OnItemAdd += UpdateMisionProgres;

    }
    public Mision(MisionInfo info)
    {
        this.info = info;

        if (info.SubMisions.Count != 0)
        {
            for (int i = 0; i < info.SubMisions.Count; i++)//genero cantidad de misiones de a cuerdo a la dificultad 
            {
                subMisionInfo subMision = info.SubMisions[i];
                subMisions.Add(new SubMision(subMision)); 
            }

        }

        Inventory.OnItemAdd += UpdateMisionProgres;
    }
    public void UpdateMisionProgres(Item item)
    {
        if(!Completed )
        {
            int n = 0;

            for (int i = 0; i < subMisions.Count; i++)
            {
                if (subMisions[i].Completed)
                    n++;

                else if (subMisions[i].Item == item)
                {
                    SubMisions[i].Progres++;
                    if (subMisions[i].Completed)
                        n++;
                }
            }

            Completed = n == SubMisions.Count;
        }
    }
}

[System.Serializable]
public class MisionInfo
{
    public string name;
    public string wname;
    public int dificulty;
    public List<subMisionInfo> SubMisions;
    public int reward;
    public bool completed;

    public MisionInfo()
    {
    }

    public MisionInfo(string name , string wname, int dificulty, List<subMisionInfo> SubMisions, int reward)
    {
        this.name = name;
        this.wname = wname;
        this.dificulty = dificulty;
        this.SubMisions = SubMisions;
        this.reward = reward;
    }


}

