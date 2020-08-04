using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.IO;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager instance;
    readonly TypedLobby typedLobby = new TypedLobby("SqlTypedLobby", LobbyType.SqlLobby);
    public const string ELO_PROP_KEY = "C0";
    public const int MAX_PLAYERS = 5;
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
        matchmakingSqlQuery = "C0 BETWEEN -100 + " + CloudManager.instance.GetRank() + " AND 100 + " + CloudManager.instance.GetRank();
        PhotonNetwork.ConnectUsingSettings();
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

    public void CreateRoom(string roomName)
    {
        int rank;
        int.TryParse(CloudManager.instance.GetRank(), out rank);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = MAX_PLAYERS;

        // if it's a ranked game, add the elo to the room
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;

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
        ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable { { ELO_PROP_KEY, CloudManager.instance.GetRank() } };

        // join random room
        PhotonNetwork.JoinRandomRoom(customRoomProperties, MAX_PLAYERS, MatchmakingMode.FillRoom, typedLobby, matchmakingSqlQuery);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Could not find room to join, creating room...");
        Debug.Log(returnCode + " " + message);
        CreateRoom("");
    }

    [PunRPC]
    public void ChangeScene(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }
}