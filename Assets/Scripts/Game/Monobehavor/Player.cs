using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour, IDamagable {

    [SerializeField] float maxFuel=10;
    [SerializeField] float movSpeed=15;
    [SerializeField] float trhusterForce=40;
    [SerializeField] float flyUnbralSensiviliti=0.2f;
    [SerializeField] float digCooldown=0.4f;
    [SerializeField] int maxHealth = 100;
    [SerializeField] int fallingDamage = 20;
    [SerializeField] float fallingDamageUnbral = 2.5f;

    Rigidbody2D rigidbody;

    private float fuel;
    private int health;
    private bool canDig =true;
    private MoveStates mState = MoveStates.normal;
    private Vector2 movingDir;
    private float digCount = 0;

    public int Health
    {
        get
        {
            return health;
        }

        set
        {
            health = value;
            if (OnHealthChange != null)
                OnHealthChange((float)health/maxHealth);
        }
    }

    public float Fuel
    {
        get
        {
            return fuel;
        }

        set
        {
            fuel = Mathf.Clamp(value,0,maxFuel);
            if (OnFuelChange != null)
                OnFuelChange(fuel / maxFuel);
        }
    }

    public delegate void DiggingToDir(Vector3 dPos);
    public event DiggingToDir OnDiggingToDir;

    public delegate void HealthChange(float _newHealth);
    public event HealthChange OnHealthChange;

    public delegate void FuelChange(float _newFuel);
    public event FuelChange OnFuelChange;

    public enum MoveStates { normal, fly, dig }

    void Start () {

        FindObjectOfType<VariableJoystick>().OnJoysticInput += Move;
        rigidbody = GetComponent<Rigidbody2D>();
        Health = maxHealth;
        Fuel = maxFuel;

	}

    void Move(Vector2 _movDir)
    {
        movingDir = _movDir;
        if (movingDir.magnitude !=0)
        {
            if (movingDir.y > flyUnbralSensiviliti)
                mState = MoveStates.fly;
            else if(mState == MoveStates.dig)
                mState = MoveStates.normal;

            switch (mState)
            {
                case MoveStates.normal:
                    if (Mathf.Abs(movingDir.x) >= Mathf.Abs(movingDir.y))
                    {
                        rigidbody.velocity = new Vector2(movingDir.x, 0) * movSpeed * Time.deltaTime;
                        rigidbody.velocity = new Vector2(Mathf.Clamp(rigidbody.velocity.x, -10, 10), Mathf.Clamp(rigidbody.velocity.y, -10, 10));
                    }
                    break;

                case MoveStates.fly:
                    if(Fuel>0)
                    {
                        rigidbody.AddForce(new Vector2(movingDir.x * (trhusterForce / 2), movingDir.y * trhusterForce ) * Time.deltaTime);
                        //rigidbody.velocity += new Vector2(_inputPos.x/4 , _inputPos.y)*Time.deltaTime;
                        //rigidbody.velocity = new Vector2(rigidbody.velocity.x, rigidbody.velocity.y + 1);
                        rigidbody.velocity = new Vector2(Mathf.Clamp(rigidbody.velocity.x, -5, 5), Mathf.Clamp(rigidbody.velocity.y, -5, 5));
                        Fuel -= Time.deltaTime;
                    }
                    break;

            }
        }
       
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D c in collision.contacts)
        {
            Vector3 relativePosition = transform.InverseTransformPoint(c.point);
            if (Mathf.Abs(relativePosition.x) < Mathf.Abs(relativePosition.y))//si la colicion es en y
            {
                if (relativePosition.y < 0 )//si la colicion es por abajo 
                {
                    //Debug.Log("recibi  una colicion por abajo a velocidad: "+ collision.relativeVelocity.magnitude);
                    if (collision.relativeVelocity.magnitude > fallingDamageUnbral)
                        GetDamage(fallingDamage);
                }
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        foreach (ContactPoint2D c in collision.contacts)
        {
            Vector3 relativePosition = transform.InverseTransformPoint(c.point);

            if (Mathf.Abs(relativePosition.x) < Mathf.Abs(relativePosition.y))//si la colicion es en y
            {
                if (relativePosition.y < 0 )
                {
                    if (Fuel < maxFuel)
                        Fuel += Time.deltaTime * 4;


                    if ( mState != MoveStates.dig)//si la colicion es por abajo y no estoy minando
                    {
                        mState = MoveStates.normal;

                        if (movingDir.magnitude != 0)//sim me estoy intentado mover
                        {
                            if ((Mathf.Abs(movingDir.x) < Mathf.Abs(movingDir.y) && movingDir.y < 0))//si intento ir hacia abajo
                            {
                                Vector3 nextPos = transform.position + new Vector3(movingDir.x, movingDir.y, 0);
                                DigCheker(nextPos);
                            }
                        }
                    }
                }
            }
            else
            {
                if (Mathf.Abs(movingDir.x) > Mathf.Abs(movingDir.y))
                {
                    if (mState == MoveStates.normal && movingDir.magnitude != 0)
                    {
                        if (relativePosition.x < 0 && movingDir.x < 0)
                        {
                            Vector3 nextPos = transform.position + new Vector3(movingDir.x, movingDir.y, 0);
                            DigCheker(nextPos);
                        }
                        if (relativePosition.x > 0 && movingDir.x > 0)
                        {
                            Vector3 nextPos = transform.position + new Vector3(movingDir.x, movingDir.y, 0);
                            DigCheker(nextPos);
                        }
                    }

                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        digCount = 0;
        if (collision.contacts.Length > 0)
        {
            foreach (ContactPoint2D c in collision.contacts)
            {
                Vector3 relativePosition = transform.InverseTransformPoint(c.point);

                if (Mathf.Abs(relativePosition.x) < Mathf.Abs(relativePosition.y))//si la colicion es en y
                {
                    Debug.Log("sali de colicion en y");
                    if (relativePosition.y < 0)//si la colicion es por abajo y no estoy minando
                    {
                        Debug.Log("sali de colicion por abajo");
                        mState = MoveStates.fly;
                    }
                }
            }
        }
        else
        {
            mState = MoveStates.fly;
        }
        
    }

    void DigCheker(Vector3 _dir)
    {
        digCount += Time.deltaTime;
        if (digCount > digCooldown)
        {
            if (canDig && mState == MoveStates.normal)
            {
                digCount = 0;
                canDig = false;
                StopAllCoroutines();
                StartCoroutine(digMove(movingDir));
                if (OnDiggingToDir != null)
                    OnDiggingToDir(_dir);

            }
        }
    }

    IEnumerator digMove(Vector3 fPos)
    {
        //teletrasporte
        mState = MoveStates.dig;
        yield return new WaitForSeconds(digCooldown);
        mState = MoveStates.normal;
        canDig = true;
    }

    public void GetDamage(int _Damage)
    {
        Health -=  _Damage;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + new Vector3(movingDir.x, movingDir.y, 0), 1);
    }
}

  



