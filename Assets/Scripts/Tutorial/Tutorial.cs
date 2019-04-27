using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Tutorial 
{
    [SerializeField] Dialogue[] Instrucctions;
    int step = 0;

    public delegate void TutoruialEnd();
    public event TutoruialEnd OnTutorialEnd;

    public void ShowInstructions()
    {
        Dialogue.Dispalay(Instrucctions[step]);

        if(step<Instrucctions.Length)
            Dialogue.OnEndDialogue += ()=> { step++; ShowInstructions(); };

        else if (OnTutorialEnd!=null)
        {
            OnTutorialEnd();
        }
    }

    
}
