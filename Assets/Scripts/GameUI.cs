using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class GameUI : MonoBehaviour
{
    public PlayerUIContainer[] playerContainers;
    public TextMeshProUGUI loseText;
    public TextMeshProUGUI countdownText;
    public GameObject menu;

    // instance
    public static GameUI instance;

    void Awake()
    {
        // set the instance to this script
        instance = this;
    }

    void Start()
    {
        InitializePlayerUI();
    }

    void Update()
    {
        UpdatePlayerUI();
    }

    // initializes the player UI containers
    void InitializePlayerUI()
    {
        // loop through all of the containers
        for (int x = 0; x < playerContainers.Length; ++x)
        {
            PlayerUIContainer container = playerContainers[x];

            // only enable and modify the UI containers we need
            if (x < PhotonNetwork.PlayerList.Length)
            {
                container.obj.SetActive(true);
                container.nameText.text = PhotonNetwork.PlayerList[x].NickName;
                container.tagTimeSlider.maxValue = GameManager.instance.timeToLose;
            }
            else
                container.obj.SetActive(false);
        }
    }

    // updates the player UI sliders
    void UpdatePlayerUI()
    {
        // loop through all of the players
        for (int x = 0; x < GameManager.instance.players.Length; ++x)
        {
            if (GameManager.instance.players[x] != null) { 
                playerContainers[x].tagTimeSlider.value = GameManager.instance.players[x].curTagTime;
                playerContainers[x].timer.fillAmount = 1.0f / GameManager.instance.timeToLose  * GameManager.instance.players[x].curTagTime;
            }
        }
    }

    // called when a player has won the game
    public void SetLoseText(string loserName)
    {
        loseText.gameObject.SetActive(true);
        loseText.text = loserName + " lost";
    }

    public void BeginCountdown(int seconds)
    {
        StartCoroutine(Countdown(seconds));
    }

    // begin 3... 2... 1... Go! Countdown
    IEnumerator Countdown(int seconds)
    {
        Debug.Log("countdown was called");

        countdownText.gameObject.SetActive(true);
        int count = seconds;

        while (count >= 0)
        {
            // display 3... 2... 1... Go! Text
            if (count == 0)
            {
                countdownText.text = "Go!";
            }
            else
            {
                countdownText.text = count.ToString();
            }

            // wait for second
            count--;
            yield return new WaitForSeconds(1);
        }

        GameManager.instance.StartGame();
        countdownText.gameObject.SetActive(false);
    }

    // open or close menu while in game
    public void EscapeMenu()
    {
        if (menu.activeSelf)
            menu.SetActive(false);
        else
            menu.SetActive(true);
    }
}

// class which holds info for each player's UI element
[System.Serializable]
public class PlayerUIContainer
{
    public GameObject obj;
    public TextMeshProUGUI nameText;
    public Slider tagTimeSlider;
    public Image timer;
}