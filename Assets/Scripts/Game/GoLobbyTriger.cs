using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoLobbyTriger : MonoBehaviour
{
    void Start()
    {
        transform.GetComponent<Button>().onClick.AddListener(() => GameManager.Instance.LoadLobby());       
    }

}
