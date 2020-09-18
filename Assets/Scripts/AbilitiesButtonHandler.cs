using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilitiesButtonHandler : MonoBehaviour
{
    private Button ability;
    private PlayerManager playerManager;

    private bool startAbilities;
    
    private void Awake()
    {
        playerManager = gameObject.transform.root.gameObject.GetComponent<PlayerManager>();
        ability = gameObject.GetComponent<Button>();
    }

    // Start is called before the first frame update
    void Start()
    {
        startAbilities = false;
        ability.interactable = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerManager.startGame && !startAbilities)
        {
            ability.interactable = true;
            startAbilities = true;
        } else if (!startAbilities)
        {
            ability.interactable = false;
        }

        if (startAbilities && GameManager.Instance.gameEnded)
            ability.interactable = false;
    }
}
