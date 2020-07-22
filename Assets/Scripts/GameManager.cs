using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("Stats")]
    public bool gameEnded = false;
    public float timeToLose;
    public float invincibleDuration;
    public float taggedTime;

    [Header("Players")]
    public string playerPrefabLocation;
    public Transform[] spawnPoints;
    public PlayerController[] players;
    public int taggedPlayer;
    private int playersInGame;

    [Header("Components")]
    public PhotonView photonView;

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
        players = new PlayerController[PhotonNetwork.PlayerList.Length];
        
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
        // instantiate the player accross the network
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation, spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);
        
        // get the player script
        PlayerController playerScript = playerObj.GetComponent<PlayerController>();

        // intialize the player
        playerScript.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }

    // returns the player of the requested id
    public PlayerController GetPlayer(int playerId)
    {
        return players.First(x => x.id == playerId);
    }

    // returns the player of the requested GameObject
    public PlayerController GetPlayer(GameObject playerObj)
    {
        return players.First(x => x.gameObject == playerObj);
    }

    // called when a player tagged someone else
    [PunRPC]
    public void TagPlayer(int playerId, bool initialTag)
    {
        if (!initialTag)
            GetPlayer(taggedPlayer).TagPlayer(false);

        taggedPlayer = playerId;
        GetPlayer(playerId).TagPlayer(true);
        taggedTime = Time.time;
    }

    // is the player able to get tagged at this current time?
    public bool CanGetTagged()
    {
        if (Time.time > taggedTime + invincibleDuration)
            return true;
        else
            return false;
    }

    // called when a player was tagged passed the maxed time - player tagged lost
    [PunRPC]
    void GameOver(int playerId)
    {
        gameEnded = true;
        PlayerController player = GetPlayer(playerId);
        GameUI.instance.SetLoseText(player.photonPlayer.NickName);

        // Wait 3 seconds before going back to Menu Scene
        Invoke("GoBackToMenu", 3.0f);
    }

    // called after the game has been won - navigates back to the Menu scene
    void GoBackToMenu()
    {
        PhotonNetwork.LeaveRoom();
        NetworkManager.instance.ChangeScene("Menu");
    }
}
