﻿using System.Collections;
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
    string matchmakingSqlQuery = "C0 == -10";

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
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
      
    }

    // get list of rooms based on string query
    public void GetListOfRooms(string query)
    {
        Debug.Log("GetListRooms");
        PhotonNetwork.GetCustomRoomList(typedLobby, matchmakingSqlQuery);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("OnRoomListUpdate " + roomList.Count);

        for (int i = 0; i < roomList.Count; i++)
        {
            Debug.LogFormat(this, "[{0}] - {1}", i, roomList[i].Name);
        }

        PopulateGrid.instance.PopulateRoomList(roomList);
    }

    public void CreateRoom(string roomName)
    {
        int rank;
        int.TryParse(CloudManager.instance.GetRank(), out rank);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = MAX_PLAYERS;
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable { { ELO_PROP_KEY, rank } };
        roomOptions.CustomRoomPropertiesForLobby = roomPropertiesLobby;
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;

        PhotonNetwork.CreateRoom(roomName, roomOptions, typedLobby);
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public void JoinRandomRoom()
    {
        ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable { { ELO_PROP_KEY, -10 } };

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
