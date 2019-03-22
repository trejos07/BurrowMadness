using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjPool<T> where T : class, new()
{
    
    static ObjPool<T> me;
    int size;

    List<T> objects = new List<T>();

    public ObjPool(int size)
    {
        this.size = size;
        if (Me != null)
            Me = this;

        for (int i = 0; i < size; i++)
        {
            objects.Add(new T());
        }
    }

    public static ObjPool<T> Me
    {
        get
        {
            return me;
        }

        set
        {
            me = value;
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
