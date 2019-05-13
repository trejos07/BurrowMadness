using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : Character, IDamagable {

    private static Player instance;

    [SerializeField] GameObject jet_FX;

    [SerializeField] float maxFuel=10;
    [SerializeField] float trhusterForce=40;
    [SerializeField] float flyUnbralSensiviliti=0.2f;
    [SerializeField] float digCooldown=0.4f;
    [SerializeField] int fallingDamage = 20;
    [SerializeField] float fallingDamageUnbral = 2.5f;

    private List<Weapon> weapons;
    private float fuel;
    private bool canDig =true;
    private MoveStates mState = MoveStates.normal;
    private Vector2 movingDir;
    private float digCount = 0;
    
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
    public MoveStates MState
    {
        get
        {
            return mState;
        }

        set
        {
            
            mState = value;
        }
    }
    public Vector2 MovingDir
    {
        get
        {
            return movingDir;
        }

    }
    public List<Weapon> Weapons
    {
        get
        {
            return weapons;
        }
    }
    public static Player Instance
    {
        get
        {
            return instance;
        }
    }

    public delegate void Vector3Action(Vector3 dPos);
    public delegate void ChangeState();
    public event Action OnMoving;
    public event Vector3Action OnDiggingToDir;
    public event FloatValueChange OnFuelChange;

    public enum MoveStates { normal, fly, dig }

    protected override void Awake()
    {
        base.Awake();
        if (instance == null)
            instance = this;
        else
        {
            DestroyImmediate(gameObject);
            return;
        }
        DontDestroyOnLoad(instance.gameObject);
        weapons = new List<Weapon>(transform.Find("GunPivot").GetComponentsInChildren<Weapon>());
        
    }

    protected override void Start ()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        //OnGameplay();
        GameManager.Instance.OnLevelLoaded += OnGameplay;
        if (Weapons.Count > 0)
            EquipWeapon(Weapons[0]);
    }

    public void EquipWeapon(Weapon _weapon)
    {
        if(_weapon!=null)
        {
            if (weapon != null)
                weapon.Renderer.enabled = false;
            weapon = _weapon;
            weapon.Renderer.enabled = true;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0)
        {
            OnLobby();
        }

    }

    void OnLobby()
    {
        if (weapon != null)
        {
            weapon.Active = false;
        }
        Rigidbody.velocity = Vector2.zero;
        Rigidbody.position = Vector2.down * 9;
        weapon.Rigidbody.rotation = 0;
        foreach (Delegate d in OnFuelChange.GetInvocationList())
        {
            OnFuelChange -= (FloatValueChange)d;
        }

    }

    void OnGameplay()
    {
        if (weapon != null)
        {
            weapon.Active = true;
        }
        VariableJoystick.Instance.OnJoysticInput += MoveTo;
        HP = maxHealth;
        Fuel = maxFuel;
        Rigidbody.velocity = Vector2.zero;
        Rigidbody.position = Vector2.up * 5;
        weapon.OnGameplayStart();
    }
    public override void MoveTo(Vector2 _movDir)
    {
        movingDir = _movDir;
        if (movingDir.magnitude !=0)
        {
            if (movingDir.y > flyUnbralSensiviliti)
                MState = MoveStates.fly;
            else if(MState == MoveStates.dig)
                MState = MoveStates.normal;

            switch (MState)
            {
                case MoveStates.normal:
                    if (Mathf.Abs(movingDir.x) >= Mathf.Abs(movingDir.y))
                    {
                        Rigidbody.velocity = new Vector2(movingDir.x, 0) * MovSpeed * Time.deltaTime;
                        Rigidbody.velocity = new Vector2(Mathf.Clamp(Rigidbody.velocity.x, -10, 10), Mathf.Clamp(Rigidbody.velocity.y, -10, 10));
                    }
                    break;

                case MoveStates.fly:
                    if(Fuel>0)
                    {
                        Rigidbody.AddForce(new Vector2(movingDir.x * (trhusterForce / 2), movingDir.y * trhusterForce ) * Time.deltaTime);
                        //rigidbody.velocity += new Vector2(_inputPos.x/4 , _inputPos.y)*Time.deltaTime;
                        //rigidbody.velocity = new Vector2(rigidbody.velocity.x, rigidbody.velocity.y + 1);
                        Rigidbody.velocity = new Vector2(Mathf.Clamp(Rigidbody.velocity.x, -5, 5), Mathf.Clamp(Rigidbody.velocity.y, -5, 5));
                        Fuel -= Time.deltaTime;
                    }
                    break;

            }
            
        }
        UpdateAnimation(_movDir);

        //if (OnMoving != null)
        //{
        //    OnMoving();
        //}
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


                    if ( MState != MoveStates.dig)//si la colicion es por abajo y no estoy minando
                    {
                        MState = MoveStates.normal;

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
                    if (MState == MoveStates.normal && movingDir.magnitude != 0)
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
                        MState = MoveStates.fly;
                    }
                }
            }
        }
        else
        {
            MState = MoveStates.fly;
        }
        
    }
    public void UpdateAnimation(Vector2 _movDir)
    {
        if (_movDir.magnitude > 0)
        {
            if (_movDir.x != 0)
            {
                SoundManager.instance.Play("MovingWithoutDrill");
                if (_movDir.x > 0)
                {
                    SpriteRender.flipX = false;

                }
                else if (_movDir.x < 0)
                {
                    SpriteRender.flipX = true;
                }
            }
            if (_movDir.y >= flyUnbralSensiviliti)
            {
                SoundManager.instance.Stop("MovingWithoutDrill");
                if (Fuel > 0)
                {
                    jet_FX.SetActive(true);
                    SoundManager.instance.Play("Jetpack");
                }
            }
            else
            {
                jet_FX.SetActive(false);
                SoundManager.instance.Stop("Jetpack");
            }

        }
        else
        {
            jet_FX.SetActive(false);
            SoundManager.instance.Stop("MovingWithoutDrill");
            SoundManager.instance.Stop("Jetpack");
        }

    }
    void DigCheker(Vector3 _dir)
    {
        digCount += Time.deltaTime;
        if (digCount > digCooldown)
        {
            if (canDig && MState == MoveStates.normal)
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
        MState = MoveStates.dig;
        yield return new WaitForSeconds(digCooldown);
        MState = MoveStates.normal;
        canDig = true;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + new Vector3(movingDir.x, movingDir.y, 0), 1);
    }
    protected override void Death()
    {
        base.Death();
        GameManager.Instance.LoadLobby();
    }
}

  



