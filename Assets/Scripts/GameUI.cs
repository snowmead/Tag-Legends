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

    // called when a player has reached the tag limit
    public void SetEndGameScreen(int newRank, int playersLeft)
    {
        EndGameScreen.SetActive(true);
        
        // Show rank section on screen if it's a ranked game
        if (newRank >= 0)
        {
            RankScoreScreen.SetActive(true);
            RankScore.text = newRank.ToString();
        }
        else
        {
            RankScoreScreen.SetActive(false);
        }
        
        EndGameScreenTitle.gameObject.SetActive(true);
        if (playersLeft == 1)
        {
            EndGameScreenTitle.text = "You WON!";
        }
        else
        {
            switch (playersLeft)
            {
                case 2:
                    EndGameScreenTitle.text = "You came in " + playersLeft + "nd place!";
                    break;
                case 3:
                    EndGameScreenTitle.text = "You came in " + playersLeft + "rd place!";
                    break;
                case 4:
                    EndGameScreenTitle.text = "You came in " + playersLeft + "th place!";
                    break;
                case 5:
                    EndGameScreenTitle.text = "You came in " + playersLeft + "th place!";
                    break;
            }
        }
    }

    public void BeginCountdown(int seconds)
    {
        StartCoroutine(Countdown(seconds));
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