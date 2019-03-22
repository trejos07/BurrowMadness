using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class LecturaDatos {

    FileInfo archivo = null;
    StreamReader lectura = null;
    int totalLineas;

    public LecturaDatos (string _nombre)
    {
        archivo = new FileInfo(_nombre + ".txt");
        lectura = archivo.OpenText();

        lectura.Close();
        lectura = archivo.OpenText();

        string texto = "";
        while (texto != null)
        {
            texto = lectura.ReadLine();
            totalLineas++;
        }

    }

    public void close ()
    {
        lectura.Close();
    }

    public int TotalLineas
    {
        get
        {
            return totalLineas;
        }
    }

    public string Leer (int _indece)
    {
        string texto = "";

        lectura.Close();
        lectura = archivo.OpenText();

        int i = 0;
        while(texto != null)
        {
            texto = lectura.ReadLine();
            if (i == _indece)
            { break; }
            i++;
        }
        return texto;
    }


}
