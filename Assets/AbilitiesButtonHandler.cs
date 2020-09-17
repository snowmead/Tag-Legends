using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilitiesButtonHandler : MonoBehaviour
{
    private Button ability;
    private PlayerManager playerManager;
    private void Awake()
    {
        playerManager = gameObject.transform.root.gameObject.GetComponent<PlayerManager>();
        ability = gameObject.GetComponent<Button>();
    }

    // Start is called before the first frame update
    void Start()
    {
        ability.interactable = false;
    }

    // Update is called once per frame
    void Update()
    {
        ability.interactable = !GameManager.instance.gameEnded && playerManager.startGame;
    }
}
