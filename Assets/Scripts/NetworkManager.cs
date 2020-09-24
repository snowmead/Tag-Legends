using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager instance;
    readonly TypedLobby typedLobby = new TypedLobby("SqlTypedLobby", LobbyType.SqlLobby);
    public const string ELO_PROP_KEY = "C0";
    public const int MaxPlayersDefault = 5;
    string[] roomPropertiesLobby = { ELO_PROP_KEY };
    string matchmakingSqlQuery;
    public bool rankedGame = false;

    private void Awake()
    {
        // If an instance already exists and it's not this one - destroy to avoid duplicate NetworkManager object
        if (instance != null && instance != this)
            gameObject.SetActive(false);
        else
        {
            // Set the instance
            instance = this;
            // Don't destroy NetworkManager game object when switching scenes
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        matchmakingSqlQuery = "C0 BETWEEN -100 + " + 
                              CloudManager.Instance.GetRank() + 
                              " AND 100 + " + 
                              CloudManager.Instance.GetRank();
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        // increase the progress bar of the loading screen
        MenuLoading.instance.PhotonConnectionDone();
        
        #if UNITY_EDITOR
            Debug.Log("Connected to Photon Region: " + PhotonNetwork.CloudRegion);
        #endif
    }

    // get list of rooms based on string query
    public void GetListOfRooms()
    {
        PhotonNetwork.GetCustomRoomList(typedLobby, matchmakingSqlQuery);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (roomList.Count > 0)
            PopulateGrid.instance.PopulateRoomList(roomList);
    }

    public Room CurrentRoom()
    {
        return PhotonNetwork.CurrentRoom;
    }

    public void CreateRoom(string roomName, int numberOfPlayers)
    {
        int rank;
        int.TryParse(CloudManager.Instance.GetRank().ToString(), out rank);

        RoomOptions roomOptions = new RoomOptions();
        
        roomOptions.IsOpen = true;

        // number of players is specified when creating a custom game
        if (numberOfPlayers <= 1)
        {
            // if it's a ranked game, add the elo to the room
            roomOptions.IsVisible = true;
            roomOptions.MaxPlayers = MaxPlayersDefault;
        }
        else
        {
            roomOptions.IsVisible = false;
            roomOptions.MaxPlayers = (byte) numberOfPlayers;
        }
        
        if (rankedGame)
        {
            roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable { { ELO_PROP_KEY, rank } };
            roomOptions.CustomRoomPropertiesForLobby = roomPropertiesLobby;
            PhotonNetwork.CreateRoom(roomName, roomOptions, typedLobby);
        } 
        else
        {
            PhotonNetwork.CreateRoom(roomName, roomOptions);
        }
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    // join specific unranked room
    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    // join random unranked room
    public void JoinRandomRoomUnranked()
    {
        rankedGame = false;
        PhotonNetwork.JoinRandomRoom();
    }

    // join random ranked room
    public void JoinRandomRoomRanked()
    {
        // set ranked game to true
        rankedGame = true;

        // set custom room properties - elo
        ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable { { ELO_PROP_KEY, CloudManager.Instance.GetRank().ToString() } };

        // join random room
        PhotonNetwork.JoinRandomRoom(customRoomProperties, MaxPlayersDefault, MatchmakingMode.FillRoom, typedLobby, matchmakingSqlQuery);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        // create a room if unable to join one
        CreateRoom("", 0);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        CreateRoom(Menu.instance.CustomGameName.text, Menu.instance.GetMaxNumberOfPlayersFromDropdown());
    }

    public override void OnJoinedRoom()
    {
        Menu.instance.UpdateCustomGamePlayersDenominator(PhotonNetwork.CurrentRoom.MaxPlayers);
    }

    public override void OnCreatedRoom()
    {
        Menu.instance.UpdateCustomGamePlayersDenominator(PhotonNetwork.CurrentRoom.MaxPlayers);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient &&
            PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            // send an rpc call to all players in the room to load the "Game" scene
            photonView.RPC("ChangeScene", RpcTarget.All, "Game");
        }
    }

    // We use this function to handle what happens when someone leaves in the middle of a game of tag
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // check if we are in the Game Scene (in a game)
            if (SceneManager.GetActiveScene().name == "Game")
            {
                GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

                // remove player from the list of players
                GameManager.Instance.photonView.RPC(
                    "RemovePlayer",
                    RpcTarget.All,
                    otherPlayer.ActorNumber,
                    true);

                // check if there's only one player left in the game
                Debug.Log(gameManager.players.Length);
                if (gameManager.players.Length <= 1)
                {
                    // send -1 to the RPC function that handles this integer as an indicator that the game was cut short
                    GameManager.Instance.photonView.RPC("GameOver", RpcTarget.All, -1);
                }
                // check if the person that left is the one who was tag
                // if he is then we must make someone else tag
                else if (otherPlayer.ActorNumber == gameManager.taggedPlayer)
                {
                    Debug.Log("otherPlayer.ActorNumber" + otherPlayer.ActorNumber);
                    // since there is still players remaining in the game
                    // we must tag another random person to keep the game going
                    GameManager.Instance.photonView.RPC(
                        "TagPlayer",
                        RpcTarget.All,
                        gameManager.GetPlayerFirstPlayerFromList().id,
                        false, true);
                }
            }
        }
    }

    [PunRPC]
    public void ChangeScene(string sceneName)
    {
        // when a game has started - make the room impossible to join
        PhotonNetwork.CurrentRoom.IsOpen = false;
        // load game scene
        PhotonNetwork.LoadLevel(sceneName);
    }
}