using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; 

public class BulletPool: MonoBehaviour
{
    [SerializeField] private Bullet m_bullet;
    [SerializeField] int size;

    private int active;
    private Queue<Bullet> objs = new Queue<Bullet>();
    private Queue<Bullet> returnigObj = new Queue<Bullet>();
    private Vector3 defPos = Vector2.one * 5000;

    public static BulletPool CreatePool(GameObject where, Bullet bullet, int size)
    {
        where.SetActive(false);
        where.name = "BulletPool_" + bullet.Name;
        BulletPool myC = where.AddComponent<BulletPool>();
        myC.m_bullet = bullet;
        myC.size = size;
        where.SetActive(true);
        return myC;
    }

    private void Awake()
    {
        Bullet.OnInactive += ReturnToPool;
        for (int i = 0; i < size; i++)
        {
            objs.Enqueue(RequireT());
        }
    }
    private void FixedUpdate()
    {
        Movebodies();
    }
    void Movebodies()
    {
        while (returnigObj.Count > 0)
        {
            Bullet bullet = returnigObj.Dequeue();
            objs.Enqueue(bullet);
            bullet.Rigidbody.position = (defPos);
            //bullet.Rigidbody.Sleep();
            //bullet.Rigidbody.simulated = false;
            bullet.Collider.enabled = false;
        }
    }
    public Bullet GetAt(Vector3 pos, float rot)
    {
        if (objs.Count > 0)
        {
            Bullet obj = objs.Dequeue();
            active++;
            obj.Rigidbody.simulated = true;
            obj.Rigidbody.WakeUp();
            obj.Collider.enabled = true;
            obj.Trail.Clear();
            obj.Rigidbody.position = pos;
            obj.Rigidbody.rotation=(rot);
            obj.Trail.Clear();
            return obj;
        }
        else
        {
            Bullet obj = RequireT();
            active++;
            obj.Rigidbody.simulated = true;
            obj.Rigidbody.WakeUp();
            obj.Collider.enabled = true;
            obj.Trail.Clear();
            obj.Rigidbody.position = pos;
            obj.Rigidbody.rotation = (rot);
            obj.Trail.Clear();
            obj.GetComponent<TrailRenderer>().Clear();
            return obj;
        }


    }
    public void ReturnToPool(Bullet bullet)
    {
        if(bullet.name == m_bullet.name)
        {
            returnigObj.Enqueue(bullet);
            active--;
        }
    }
    public virtual Bullet RequireT()
    {
        Bullet b = Instantiate<Bullet>(m_bullet, defPos, transform.rotation, transform);
        b.Rigidbody.simulated = false;
        b.Collider.enabled = false;
        return b;
    }
   
}
