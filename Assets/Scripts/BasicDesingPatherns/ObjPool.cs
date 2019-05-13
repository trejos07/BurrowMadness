using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjPool<T> where T : class
{
    MonoBehaviour owner;
    public static ObjPool<T> Me;
    int size;

    List<T> objects = new List<T>();

    public ObjPool(MonoBehaviour _owner, int size)
    {
        this.size = size;
        if (Me != null)
            Me = this;

        for (int i = 0; i < size; i++)
        {
            //objects.Add();
        }
    }

    public MonoBehaviour Owner
    {
        get
        {
            return owner;
        }

        set
        {
            owner = value;
        }
    }

    public T GetT()
    {
        objects.Remove(objects[objects.Count - 1]);
        return objects[objects.Count - 1];
    }

    public void ReturnToPool(T gameObject)
    {
        objects.Add(gameObject);

    }

}
