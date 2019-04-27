using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [SerializeField] Tutorial mainTutorial;

    public Tutorial MainTutorial
    {
        get
        {
            return mainTutorial;
        }

        set
        {
            mainTutorial = value;
        }
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (GameManager.ins.Settings.tutorialOn)
        {
            MainTutorial.ShowInstructions();
        }
    }

}
