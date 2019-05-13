using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundManager : MonoBehaviour
{
    SoundManager ManagerS;
    Player playerinscene;

    void Awake()
    {
        playerinscene = FindObjectOfType<Player>();
        playerinscene.OnMoving += PlayOnMove;
    }
    private void Start()
    {
        
        ManagerS.Play("MovingWithoutDrill");
    }

    public void PlayOnMove()
    {
        if (playerinscene.MState == Player.MoveStates.fly)
        {
            if (playerinscene.MovingDir.magnitude > 0)
            {
                CheckAndStop();
                ManagerS.Play("Jetpack");
            }
            else
            {
                CheckAndStop();
                //ManagerS.Play("FallingSound");
            }
        }
        else if (playerinscene.MState == Player.MoveStates.normal)
        {
            if (playerinscene.MovingDir.magnitude > 0)
            {
                CheckAndStop();
                ManagerS.Play("MovingWithoutDrill");
            }
            else
            {
                CheckAndStop();
                ManagerS.Play("IdleTotal");
            }
        }
        else if (playerinscene.MState == Player.MoveStates.dig)
        {
            CheckAndStop();
            ManagerS.Play("Recolecting");
        }

    }

    public void CheckAndStop()
    {
        ManagerS = SoundManager.instance;
        if (ManagerS.CheckPlaying("Jetpack"))
        {
            ManagerS.Stop("Jetpack");
        }
        if (ManagerS.CheckPlaying("FallingSound"))
        {
            ManagerS.Stop("Recolecting");
        }
        if (ManagerS.CheckPlaying("IdleTotal"))
        {
            ManagerS.Stop("IdleTotal");
        }
        if (ManagerS.CheckPlaying("MovingWithoutDrill"))
        {
            ManagerS.Stop("MovingWithoutDrill");
        }
        if (ManagerS.CheckPlaying("Recolecting"))
        {
            ManagerS.Stop("Recolecting");
        }

    }

}
