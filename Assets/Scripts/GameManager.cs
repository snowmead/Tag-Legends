using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using System;
using TMPro;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

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
    public int playersInGame;
    GameObject chosenClass;

    [Header("Tag Effects")] 
    public AudioSource TagSound;

    public GameObject TagYoureItText;

    [HideInInspector] 
    PlayerManager playerManagerScript;
    public bool countdownStarted = false;

    // instance
    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
        Instance.enabled = true;
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
    
    // returns the player of the requested id
    public PlayerManager GetPlayerFirstPlayerFromList()
    {
        return players.First();
    }
    
    // returns the player of the requested GameObject
    public PlayerManager GetPlayer(GameObject playerObj)
    {
        return players.First(x => x.gameObject == playerObj);
    }
    
    // removes the player from the list of players in the game
    [PunRPC]
    public void RemovePlayer(int playerId, bool destroyPlayer)
    {
        if(destroyPlayer)
            Destroy(GetPlayer(playerId));
        
        int newArrayLength = players.Where(x => x.id != playerId).ToArray().Length;
        PlayerManager[] tempPlayerManagers = new PlayerManager[newArrayLength];
        tempPlayerManagers = players.Where(x => x.id != playerId).ToArray();
        players = new PlayerManager[newArrayLength];
        players = tempPlayerManagers;
        
        // modify my rank
        if(NetworkManager.instance.rankedGame)
            CloudManager.Instance.RankModifier(playersInGame);
        
        if(destroyPlayer)
            playersInGame--;
    }

    // called when a player tagged someone else
    [PunRPC]
    public void TagPlayer(int playerId, bool initialTag, bool leftAbruptly)
    {
        // untag the player that was tag
        if (!initialTag && !leftAbruptly)
            GetPlayer(taggedPlayer).TagPlayer(false);

        taggedPlayer = playerId;
        
        // tag the player who should now be tag
        GetPlayer(playerId).TagPlayer(true);
        taggedTime = Time.time;

        // play tag sound effect
        TagSound.Play();
        
        if(playerId == playerManagerScript.id)
            TagYoureItText.SetActive(true);
        else 
            TagYoureItText.SetActive(false);
    }

    // is the player able to get tagged at this current time?
    public bool CanGetTagged(int id)
    {
        // check invincibleDuration and if the player is in an iceblock
        return Time.time > taggedTime + invincibleDuration && !GetPlayer(id).isIceBlock;
    }
    
    // sets the player in a feared state from the berserker shout ability
    [PunRPC]
    public void BerserkerShout()
    {
        playerManagerScript.SetFearState();
    }
    
    [PunRPC]
    public void FreezingWinds()
    {
        playerManagerScript.SetFreezingWindsState();
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
        // stop game over process if the player is already out of the game
        if (gameEnded)
        {
            return;
        }

        // from the network manager, when someone disconnects from the game and you are the last person in the game
        // the playerId -1 is sent to indicate that the game is finished since no one is there
        if (playerId == -1)
        {
            // reduce the number of players in the game
            EndGameForPlayer(false, PhotonNetwork.LocalPlayer.ActorNumber);
            return;
        }

        // get player
        PlayerManager player = GetPlayer(playerId);

        // only stop my game
        if (player.photonView.IsMine && !gameEnded)
        {
            EndGameForPlayer(true, playerId);
        }

        // reduce the number of players in the game
        playersInGame--;

        // end the game for the last player
        if (!player.photonView.IsMine && playersInGame == 1)
        {
            EndGameForPlayer(false, playerId);
        }
    }

    // End game actions to finish when a player has ended his game
    private void EndGameForPlayer(bool continueTheGame, int playerId)
    {
        // continue the game for the remaining players battle royale style
        if (continueTheGame)
        {
            // get the first player found from the remaining player list
            PlayerManager playerManager = players.First(x => x.id != playerId);
            
            // tag that player to continue the flow of the game
            photonView.RPC("TagPlayer", RpcTarget.All, playerManager.id, false, false);
        }

        int newRank = -1;
        
        // end the game
        gameEnded = true;
            
        // modify my rank
        if(NetworkManager.instance.rankedGame)
            newRank = CloudManager.Instance.RankModifier(playersInGame);

        // show end game screen
        GameUI.instance.SetEndGameScreen(newRank, playersInGame);
        
        // remove player from the list of players
        photonView.RPC(
            "RemovePlayer",
            RpcTarget.All,
            playerId,
            false);
    }

    // called after the game has been won - navigates back to the Menu scene
    public void GoBackToMenu()
    {
        PhotonNetwork.LeaveRoom();
        NetworkManager.instance.ChangeScene("Menu");        
    }
}