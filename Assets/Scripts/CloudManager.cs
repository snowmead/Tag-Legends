using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CloudOnce;
using CloudOnce.CloudPrefs;

public class CloudManager : MonoBehaviour
{
    private const string LeaderboardId = "CgkIyoCahe4CEAIQAQ";
    public static CloudManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            gameObject.SetActive(false);
        else
        {
            Instance = this;
            // Don't destroy NetworkManager game object when switching scenes
            DontDestroyOnLoad(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        ConnectToCloud();
    }

    public void Reconnect()
    {
        ConnectToCloud();
    }

    private void ConnectToCloud()
    {
        // initialize and load cloudonce
        Cloud.OnInitializeComplete += CloudOnceOnInitializeComplete;
        Cloud.OnCloudLoadComplete += CloudOnceLoadComplete;

        // initialize cloudonce with cloudsave and cloudload
        Cloud.Initialize(true, true);
    }

    // called when cloudonce is initialized
    void CloudOnceOnInitializeComplete()
    {
        // load cloud storage
        Cloud.Storage.Load();
    }

    // on cloudonce load complete, update the menu ui with the player's rank
    void CloudOnceLoadComplete(bool success)
    {
        MenuLoading.instance.CloudConnectionDone();
    }

    
    
    // get rank of player
    public int GetRank()
    {
        return CloudVariables.RankScore;
    }

    public string GetPlayerName()
    {
        return Cloud.PlayerDisplayName;
    }

    // increase rank of player who didn't lose the game
    public int RankModifier(int playersInGame)
    {
        int rankModifier = getRankModifier(playersInGame);
        
        // make sure player rank doesn't go below 0
        if (rankModifier >= CloudVariables.RankScore)
            CloudVariables.RankScore = 0;
        else
            CloudVariables.RankScore += rankModifier;
        
        SubmitNewRankScoreToLeaderBoard();

        return CloudVariables.RankScore;
    }

    public int getRankModifier(int playersInGame)
    {
        int eloMod;
        switch (playersInGame)
        {
            case 1:
                eloMod = 10;
                break;
            case 2:
                eloMod = 5;
                break;
            case 3:
                eloMod = 0;
                break;
            case 4:
                eloMod = -5;
                break;
            case 5:
                eloMod = -10;
                break;
            default:
                eloMod = 0;
                break;
        }
        return eloMod;
    }

    // save data to cloud storage
    private void SubmitNewRankScoreToLeaderBoard()
    {
        Cloud.Storage.Save();
        Cloud.Leaderboards.SubmitScore(LeaderboardId, CloudVariables.RankScore);
    }
}