using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using System;
using TMPro;
using System.Diagnostics;

public class GameManager : MonoBehaviourPunCallbacks
{
    public const string BERSERKER_ACTIVE_CLASS_NAME = "Berserker";
    public const string FROSTMAGE_ACTIVE_CLASS_NAME = "FrostMage";
    public const string ILLUSIONIST_ACTIVE_CLASS_NAME = "Illusionist";
    public const string NINJA_ACTIVE_CLASS_NAME = "Ninja";

    [Header("Stats")]
    public bool gameEnded = false;
    public float timeToLose;
    public float invincibleDuration;
    public float taggedTime;

    [Header("Players")]
    public GameObject character;
    public Transform[] spawnPoints;
    public PlayerManager[] players;
    public int taggedPlayer;
    private int playersInGame;
    GameObject chosenClass;

    [HideInInspector]
    PlayerManager playerManagerScript;
    public bool countdownStarted = false;

    // instance
    public static GameManager instance;

    private void Awake()
    {
        instance = this;
        instance.enabled = true;
    }

    private void Start()
    {
        // set players maximum size to the number of players who were originally in the room lobby
        players = new PlayerManager[PhotonNetwork.PlayerList.Length];

        // send rpc call to all players to spawn his player in their game
        // rpc target is set to AllBuffered since not all players will be loaded in at the same time
        photonView.RPC("ImInGame", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void ImInGame()
    {
        playersInGame++;

        // Spawn all players if all the players have been loaded in
        if (playersInGame == PhotonNetwork.PlayerList.Length)
            SpawnPlayer();
    }

    // spawns a player and initializes it
    void SpawnPlayer()
    {
        // get player's chosen class to instantiate the correct class
        chosenClass = GameObject.FindGameObjectWithTag("Player");
        chosenClass.SetActive(false);

        string characterResourceFolder = "Character/";
        string activeClass = BERSERKER_ACTIVE_CLASS_NAME;
        string prefabName;

        // instantiate the player accross the network
        switch (chosenClass.name)
        {
            case "Berserker(Clone)":
                activeClass = "Berserker";
                prefabName = characterResourceFolder + activeClass + "/" + activeClass;
                character = PhotonNetwork.Instantiate(prefabName, spawnPoints[PhotonNetwork.LocalPlayer.ActorNumber - 1].position, Quaternion.identity);
                setClassAnimator(activeClass);           
                break;
            case "FrostMage(Clone)":
                activeClass = "FrostMage";
                prefabName = characterResourceFolder + activeClass + "/" + activeClass;
                character = PhotonNetwork.Instantiate(prefabName, spawnPoints[PhotonNetwork.LocalPlayer.ActorNumber - 1].position, Quaternion.identity);
                setClassAnimator(activeClass);
                break;
            case "Ninja(Clone)":
                activeClass = "Ninja";
                prefabName = characterResourceFolder + activeClass + "/" + activeClass;
                character = PhotonNetwork.Instantiate(prefabName, spawnPoints[PhotonNetwork.LocalPlayer.ActorNumber - 1].position, Quaternion.identity);
                setClassAnimator(activeClass);
                break;
            case "Illusionist(Clone)":
                activeClass = "Illusionist";
                prefabName = characterResourceFolder + activeClass + "/" + activeClass;
                character = PhotonNetwork.Instantiate(prefabName, spawnPoints[PhotonNetwork.LocalPlayer.ActorNumber - 1].position, Quaternion.identity);
                setClassAnimator(activeClass);
                break;

        }    

        // get the player script
        playerManagerScript = character.GetComponent<PlayerManager>();

        // intialize the player
        playerManagerScript.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer, activeClass);
    }

    private void setClassAnimator(string activeClass)
    {
        Animator animator = character.GetComponent<Animator>();
        animator.SetBool(activeClass, true);
    }

    // returns the player of the requested id
    public PlayerManager GetPlayer(int playerId)
    {
        return players.First(x => x.id == playerId);
    }

    // returns the player of the requested GameObject
    public PlayerManager GetPlayer(GameObject playerObj)
    {
        return players.First(x => x.gameObject == playerObj);
    }

    // called when a player tagged someone else
    [PunRPC]
    public void TagPlayer(int playerId, bool initialTag)
    {
        // untag the player that was tag
        if (!initialTag)
            GetPlayer(taggedPlayer).TagPlayer(false);

        taggedPlayer = playerId;
        // tag the player who should now be tag
        GetPlayer(playerId).TagPlayer(true);
        taggedTime = Time.time;
    }

    // is the player able to get tagged at this current time?
    public bool CanGetTagged(int id)
    {
        // check invincibleDuration and if the player is in an iceblock
        if (Time.time > taggedTime + invincibleDuration || GetPlayer(taggedPlayer).isIceBlock)
            return true;
        else
            return false;
    }

    // called when all players are ready and loaded in
    [PunRPC]
    void StartCountdown()
    {
        // set to true
        // player controller will no longer call this method from it's update function
        countdownStarted = true;

        // begin countdown for each player
        GameUI.instance.BeginCountdown(3);
    }

    // Called when all players are ready and loaded in
    [PunRPC]
    void UpdateInGameUI()
    {
        GameUI.instance.SetPlayerVingettes();
    }

    // start the game
    public void StartGame()
    {
        // send rpc call to all players to begin their game
        playerManagerScript.photonView.RPC("BeginGame", RpcTarget.All);
    }

    // called when a player was tagged passed the maxed time - player tagged lost
    [PunRPC]
    void GameOver(int playerId)
    {
        // get player
        PlayerManager player = GetPlayer(playerId);

        int newRank = -1;
        
        // if it is me, modify my rank
        if (player.photonView.IsMine && NetworkManager.instance.rankedGame)
        {
            newRank = CloudManager.instance.RankModifier(playersInGame);
        }

        // reduce the number of players in the game
        playersInGame--;

        // end the game
        gameEnded = true;
        GameUI.instance.SetLoseText(player.PhotonPlayer.NickName, newRank, playersInGame.ToString());
    }

    // called after the game has been won - navigates back to the Menu scene
    public void GoBackToMenu()
    {
        PhotonNetwork.LeaveRoom();
        NetworkManager.instance.ChangeScene("Menu");        
    }
}