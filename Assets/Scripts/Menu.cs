using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class Menu : MonoBehaviourPunCallbacks
{
    [Header("Screens")]
    public GameObject mainScreen;
    public GameObject lobbyScreen;

    [Header("Main Screen")]
    public Button createRoomButton;
    public Button joinRoomButton;

    [Header("Lobby Screen")]
    public TextMeshProUGUI playerListText;
    public Button startGameButton;

    private void Start()
    {
        createRoomButton.interactable = false;
        joinRoomButton.interactable = false;
    }

    public override void OnConnectedToMaster()
    {
        createRoomButton.interactable = true;
        joinRoomButton.interactable = true;
    }

    void SetScreen(GameObject screen)
    {
        mainScreen.SetActive(false);
        lobbyScreen.SetActive(false);

        screen.SetActive(true);
    }

    // called when "Create Room" Button is pressed
    public void OnCreateRoomButton(TMP_InputField roomNameInput)
    {
        NetworkManager.instance.CreateRoom(roomNameInput.text);
    }

    // called when "Join Room" Button is pressed
    public void OnJoinRoomButton(TMP_InputField roomNameInput)
    {
        NetworkManager.instance.JoinRoom(roomNameInput.text);
    }

    // called when Name Input field changes - change the player's nickname
    public void OnPlayerNameUpdate(TMP_InputField playerNameInput)
    {
        PhotonNetwork.NickName = playerNameInput.text;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("SOMEONE ENTERED! " + newPlayer.NickName);
    }

    // called when a player leaves the room
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // We don't need to call the UpdateLobbyUI as an RPC function
        // The OnPlayerLeftRoom function gets called on all clients when a player leaves the room
        UpdateLobbyUI();
    }

    // called when a player joins the room
    public override void OnJoinedRoom()
    {
        SetScreen(lobbyScreen);

        // send an rpc call to update all the other clients that this player has joined the room
        // update everyone elses lobby ui
        photonView.RPC("UpdateLobbyUI", RpcTarget.All);
    }

    // updates the lobby UI to show player list and host buttons
    [PunRPC]
    public void UpdateLobbyUI()
    {
        playerListText.text = "";

        // display all the players currently in the lobby
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            playerListText.text += player.NickName + "\n";
        }

        // only the host can start the game
        if (PhotonNetwork.IsMasterClient)
            startGameButton.interactable = true;
        else
            startGameButton.interactable = false;
    }

    // called when the "Leave Lobby" button is pressed
    public void OnLeaveLobbyButton()
    {
        PhotonNetwork.LeaveRoom();
        SetScreen(mainScreen);
    }

    // called when the "Start Game" button is pressed
    // only the host can click this button
    public void OnStartGameButton()
    {
        // send an rpc call to all players in the room to load the "Game" scene
        NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, "Game");
    }
}
