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

    Coroutine am;
    Coroutine wi;


    Transform target;
    BulletPool pool; 
    Camera MainCamera;
    bool PuedoDisparar=true;
    bool autoAiming = false;


    void Start()
    {
        FindObjectOfType<ButtonJoystic>().OnJoysticInput += RotateCanon;
        FindObjectOfType<ButtonJoystic>().OnJoysticButtonRelease += FireTrigger;
        MainCamera = FindObjectOfType<Camera>();
        pool = new BulletPool(Bala, NumeroBalas);
        am = StartCoroutine(AutoAim());
        
        

    }

    public void FireTrigger()
    {
        if (PuedoDisparar)
        {
            if (autoAiming)
            {
                StartCoroutine(Disparar());
            }
            else
            {
                if(wi != null)
                    StopCoroutine(wi);
                StartCoroutine(Disparar());
                wi = StartCoroutine(PauseAutoAim(1));
            }

        }
    }

    public void RotateCanon(Vector2 _lookPos)
    {
        if(_lookPos.magnitude != 0)
        {
            Debug.Log("Simple aim On");
            StopCoroutine(am);
            autoAiming = false;
            
            float angle = Vector2.SignedAngle(transform.right, _lookPos);
            transform.RotateAround(transform.parent.position, transform.forward, angle);
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

    public IEnumerator PauseAutoAim(float t)
    {
        autoAiming = false;
        StopCoroutine(am);
        //Debug.Log("pausing Auto Aim for " + t + " seconds");
        yield return new WaitForSeconds(t);
        Debug.Log("starting Auto Aim");
        am = StartCoroutine(AutoAim());
    }

    public IEnumerator Disparar()
    {
        Debug.Log("disparo");
        
        PuedoDisparar = false;
        Bullet bullet = pool.GetNextBullet();
        if(bullet!= null)
        {
            bullet.transform.position = PuntoDeSalida.position;
            bullet.Fire(transform.right);
        }
        
        yield return new WaitForSeconds(Cooldown);
        Debug.Log("after CD");
        PuedoDisparar = true;
        
    }

    public IEnumerator AutoAim ()
    {
        Coroutine lt = null;
        Coroutine ltLats = null;
        autoAiming = true;
        while (true)
        {
            
            target = SelectNearFireable();
            if (target != null)
            {
                if(ltLats == null)
                {
                    lt = StartCoroutine(LookTo(target.position));
                    ltLats = lt;
                }
                else
                {
                    StopCoroutine(ltLats);
                    lt = StartCoroutine(LookTo(target.position));
                    ltLats = lt;
                }
            }
            yield return new WaitForSeconds(0.2f);
        }
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