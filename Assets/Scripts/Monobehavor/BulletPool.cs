using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; 

public class BulletPool
{
    GameObject Bala;
    int NumeroBalas;

    int BalaActual;
    Bullet[] Balas;

    public BulletPool(GameObject bala, int numeroBalas)
    {
        Bala = bala;
        NumeroBalas = numeroBalas;
        CrearBalas();
    }

    public void CrearBalas()
    {
        Balas = new Bullet[NumeroBalas];
        for (int i = 0; i < NumeroBalas; i++)
        {
            GameObject InstanciaBala = GameObject.Instantiate(Bala, new Vector3(i,0, 100), Quaternion.identity);
            Balas[i] = InstanciaBala.GetComponent<Bullet>();
        }
    }

    public Bullet GetNextBullet()
    {
        try
        {
            return Balas.First(b => b.IsFlying == false);
        }
        catch
        {
            Debug.LogWarning("Se Acabaron las Balas, Espera que se Recargen");
            return null;
        }
    }


}
