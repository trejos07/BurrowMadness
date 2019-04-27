using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue 
{

    public string name;

    [TextArea(3,10)]
    public string[] Content;

    public delegate void EndDialogue();
    public static event EndDialogue OnEndDialogue;

    public Dialogue()
    {
    }
   
    public static void Dispalay(Dialogue dialogue)
    {
        Queue<string> sentences = new Queue<string>();
        foreach (string s in dialogue.Content)
        {
            sentences.Enqueue(s);
        }
        Message m = MenuManager.Instance.DisplaySentence(sentences.Dequeue(), true, false);
        m.OnClose += () => DisplayNext(sentences);
    }

    public static void DisplayNext(Queue<string> sentences )
    {
        if(sentences.Count>0)
        {
            Message m = MenuManager.Instance.DisplaySentence(sentences.Dequeue(), true, false);
            m.OnClose += () => DisplayNext(sentences);
        }
        else if(OnEndDialogue!=null)
        {
            OnEndDialogue();
            foreach (EndDialogue d in OnEndDialogue.GetInvocationList())
            {
                OnEndDialogue -= d;
            }
        }
        
    }
}
