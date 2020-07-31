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
        GetListOfRooms("");
    }

    // get list of rooms based on string query
    public void GetListOfRooms(string query)
    {
        Debug.Log("GetListRooms");

        PhotonNetwork.GetCustomRoomList(typedLobby, query);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("OnRoomListUpdate");

        PopulateGrid.instance.PopulateRoomList(roomList);
    }

    public void CreateRoom(string roomName)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 5;
        
        PhotonNetwork.CreateRoom(roomName, roomOptions, typedLobby);
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Could not find room to join, creating room...");
        CreateRoom("");
    }

    [PunRPC]
    public void ChangeScene(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }
}
