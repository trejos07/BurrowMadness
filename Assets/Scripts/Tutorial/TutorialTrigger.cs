using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class TutorialTrigger : MonoBehaviour
{
    [SerializeField]Tutorial tutorial;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        tutorial.ShowInstructions();
        DestroyImmediate(gameObject);
    }

}
