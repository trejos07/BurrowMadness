using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Weapon : MonoBehaviour
{
    [SerializeField] GameObject Bala;
    [SerializeField] int NumeroBalas;
    [SerializeField] float Cooldown;
    [SerializeField] float aimOffset;
    [SerializeField] Transform PuntoDeSalida;
    [SerializeField] float detectionRadius;           // Radio de detección en que el enemigo "siente" al jugador
    [SerializeField] LayerMask fireablesLayer;
    [SerializeField] float Velocidad_Rotacion;


    IEnumerator am;


    Transform target;
    BulletPool pool; 
    Camera MainCamera;
    bool PuedoDisparar=true;


    void Start()
    {
        FindObjectOfType<ButtonJoystic>().OnJoysticInput += RotateCanon;
        FindObjectOfType<ButtonJoystic>().OnJoysticButtonRelease += FireTrigger;
        MainCamera = FindObjectOfType<Camera>();
        pool = new BulletPool(Bala, NumeroBalas);
        am = AutoAim();
        StartCoroutine(am);

    }

    public void FireTrigger()
    {
        if (PuedoDisparar)
        {
            StartCoroutine(Disparar());
        }
    }

    public void RotateCanon(Vector2 _lookPos)
    {
        if(_lookPos.magnitude != 0)
        {
            Debug.Log("Simple aim On");
            StopCoroutine(am);
            //StopCoroutine(lt);

            //Vector2 dif = _lookPos - (Vector2)transform.parent.eulerAngles;
            //float angle = Mathf.Atan2(dif.y, dif.x) * Mathf.Rad2Deg;
            //float angle = Mathf.Atan2(_lookPos.y, _lookPos.x) * Mathf.Rad2Deg;
            //if (angle < 0) angle += 360;
            float angle = Vector2.SignedAngle(transform.right, _lookPos);
            transform.RotateAround(transform.parent.position, transform.forward, angle);
            //transform.parent.rotation = Quaternion.Euler(0, 0, angle);
        }

    }

    public IEnumerator Disparar()
    {
        Debug.Log("disparo");
        StopCoroutine(am);
        //StopCoroutine(lt);
        PuedoDisparar = false;
        Bullet bullet = pool.GetNextBullet();
        if(bullet!= null)
        {
            bullet.transform.position = PuntoDeSalida.position;
            bullet.Fire(transform.right);
        }
        StartCoroutine(am);
        yield return new WaitForSeconds(Cooldown);
        Debug.Log("after CD");
        PuedoDisparar = true;
        
    }

    public IEnumerator AutoAim ()
    {
        IEnumerator lt = null;
        IEnumerator ltLats = null;

        while (true)
        {
            target = SelectNearFireable();
            if (target != null)
            {
                Debug.Log("AutoAiming");
                if(ltLats == null)
                {
                    lt = LookTo(target.position);
                    StartCoroutine(lt);
                    ltLats = lt;
                }
                else
                {
                    StopCoroutine(ltLats);
                    lt = LookTo(target.position);
                    StartCoroutine(lt);
                    ltLats = lt;
                }
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    Transform SelectNearFireable()
    {
        Transform Fireable = null;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius, fireablesLayer);
        if (colliders.Length > 0)
        {
            float nearD = Mathf.Infinity;
            for (int i = 0; i < colliders.Length; i++)
            {
                float d = Vector3.Distance(transform.position, colliders[i].transform.position);
                if (nearD > d)
                {
                    nearD = d;
                    Fireable = colliders[i].transform;
                }
            }
        }
        return Fireable;
    }

    public IEnumerator LookTo(Vector3 target)
    {
        while (true)
        {
            Vector3 relative = transform.InverseTransformPoint(target);
            float angle = (Mathf.Atan2(relative.y, relative.x) * Mathf.Rad2Deg);
            float dir = angle / Mathf.Abs(angle);
            if (angle < 0) angle += 360;

            if (angle < 5)
            {
                break;
            }
            else
            {
                transform.parent.RotateAround(transform.parent.position, transform.forward, Velocidad_Rotacion * dir * Time.deltaTime);
            }
            yield return null;
        }

    }
    private void OnDrawGizmos()
    {
        if(target != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(target.position, 1);
        }
        Gizmos.color = Color.Lerp(Color.red, Color.yellow,0.6f);
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

    }


}