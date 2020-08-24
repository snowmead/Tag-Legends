using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuLoading : MonoBehaviour
{
    public GameObject progressBarObject;
    private Slider progressBar;

    public static MenuLoading instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        progressBar = progressBarObject.GetComponent<Slider>();
        progressBar.value = 0;
    }

    private void Update()
    {
        if(progressBar.value == 1)
        {
            SceneManager.LoadScene("Menu", LoadSceneMode.Single);
        }
    }

    public void PhotonConnectionDone()
    {
        progressBar.value = 0.5f;
    }

    public void CloudConnectionDone()
    {
        progressBar.value += 0.5f;
    }
}
