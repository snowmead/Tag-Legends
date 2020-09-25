using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstTimePlaying : MonoBehaviour
{
    public GameObject tutorial;
    
    // Start is called before the first frame update
    void Start()
    {
        if(PlayerPrefs.GetInt("Tutorial") != 1)
        {
            PlayerPrefs.SetInt("Tutorial", 1);
            tutorial.SetActive(true);
        }
    }
}
