using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using System.ComponentModel;

public class GameUI : MonoBehaviour
{
    [Header("End Game Screen")] 
    public GameObject EndGameScreen;
    public TextMeshProUGUI EndGameScreenTitle;
    public GameObject RankScoreScreen;
    public TextMeshProUGUI RankScore; 
    
    public PlayerUIContainer[] playerContainers;
    public TextMeshProUGUI loseText;
    public TextMeshProUGUI countdownText;
    public GameObject menu;

    public Sprite berserkerSprite;
    public Sprite frostMageSprite;
    public Sprite illusionistSprite;
    public Sprite ninjaSprite;

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
            // update the grey mask area in the players profile
            if (GameManager.instance.players[x] != null)
                playerContainers[x].timer.fillAmount =
                    1.0f / GameManager.instance.timeToLose *
                    GameManager.instance.players[x].curTagTime;
        }
    }

    // called when a player has won the game
    public void SetLoseText(string loserName, int newRank, string playersLeft)
    {
        EndGameScreen.SetActive(true);
        if (newRank >= 0)
        {
            RankScoreScreen.SetActive(true);
            RankScore.text = newRank.ToString();
        }
            
        EndGameScreenTitle.gameObject.SetActive(true);
        if (playersLeft == "0")
        {
            EndGameScreenTitle.text = "You WON!";
        }
        EndGameScreenTitle.text = "You came in " + playersLeft + "th place!";
    }

    public void BeginCountdown(int seconds)
    {
        StartCoroutine(Countdown(seconds));
    }

    public void SetPlayerVingettes()
    {
        // Game started, set player vingettes
        for (int x = 0; x < PhotonNetwork.PlayerList.Length; ++x)
        {
            PlayerUIContainer container = playerContainers[x];

            switch (GameManager.instance.players[x].chosenClass)
            {
                case GameManager.BERSERKER_ACTIVE_CLASS_NAME:
                    container.classImage.sprite = berserkerSprite;
                    break;
                case GameManager.FROSTMAGE_ACTIVE_CLASS_NAME:
                    container.classImage.sprite = frostMageSprite;
                    break;
                case GameManager.ILLUSIONIST_ACTIVE_CLASS_NAME:
                    container.classImage.sprite = illusionistSprite;
                    break;
                case GameManager.NINJA_ACTIVE_CLASS_NAME:
                    container.classImage.sprite = ninjaSprite;
                    break;
            }           
            
            // Increase the size of your own UI vingette and set your image to the top of the player list
            if (GameManager.instance.GetPlayer(GameManager.instance.players[x].id).photonView.IsMine)
            {              
                // Increase the size of your vingette
                container.obj.gameObject.transform.localScale += new Vector3(0.5f, 0.5f, 0.5f);
                // Set yourself as the first child in the list
                container.obj.gameObject.transform.SetAsFirstSibling();               
            }
          
        }
    }

    // begin 3... 2... 1... Go! Countdown
    IEnumerator Countdown(int seconds)
    {
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
    public Image timer;
    public Image classImage;
}