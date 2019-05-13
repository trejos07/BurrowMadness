using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{

    [SerializeField] private Enemy m_enemy;
    [SerializeField] int size;
    private int active;
    private Vector3 defPos = Vector2.one * 5010;
    private Queue<Enemy> enemys = new Queue<Enemy>();
    private Queue<Enemy> returnigEnemies = new Queue<Enemy>();

    public static EnemyPool CreatePool(GameObject where, Enemy bullet, int size)
    {
        where.SetActive(false);
        where.name = "Pool_" + bullet.Name;
        EnemyPool myC = where.AddComponent<EnemyPool>();
        myC.m_enemy = bullet;
        myC.size = size;
        where.SetActive(true);
        return myC;
    }

    private void Awake()
    {
        returnigEnemies = new Queue<Enemy>();
        for (int i = 0; i < size; i++)
        {
            returnigEnemies.Enqueue(RequireT());
        }
    }
    private void FixedUpdate()
    {
        Movebodies();
    }
    public Enemy GetAt(Vector3 pos, float rot)
    {
        if (enemys.Count > 0)
        {
            Enemy obj = enemys.Dequeue();
            active++;
            //obj.Rigidbody.simulated = true;
            //obj.gameObject.SetActive(true);
            obj.Rigidbody.WakeUp(); 
            obj.Collider.enabled = true;
            obj.Rigidbody.position = pos;
            obj.Rigidbody.rotation = rot;
            obj.OnDeath += () => { ReturnToPool(obj); };
            return obj;
        }
        else
        {
            Enemy obj = RequireT();
            active++;
            //obj.Rigidbody.simulated = true;
            //obj.gameObject.SetActive(true);
            obj.Rigidbody.WakeUp();
            obj.Collider.enabled = true;
            obj.Rigidbody.position = pos;
            obj.Rigidbody.rotation = rot;
            obj.OnDeath += () => { ReturnToPool(obj); };
            return obj;
        }


    }
    public void ReturnToPool(Enemy e)
    {
        active--;
        returnigEnemies.Enqueue(e);
    }
    void Movebodies()
    {
        while (returnigEnemies.Count>0)
        {
            Enemy enemy =returnigEnemies.Dequeue();
            enemys.Enqueue(enemy);
            enemy.Rigidbody.position = (defPos);
            //enemy.Rigidbody.Sleep();
            //enemy.Rigidbody.simulated = false;
            enemy.Collider.enabled = false;
            //enemy.gameObject.SetActive(false);
        }
    }
    public virtual Enemy RequireT()
    {
        Enemy e = Instantiate<Enemy>(m_enemy, defPos, transform.rotation, transform);
        //e.Rigidbody.simulated = false;
        e.Collider.enabled = false;
        return e;
    }



}
