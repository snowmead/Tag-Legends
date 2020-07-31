using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine.Rendering;
using Firebase;

public class Menu : MonoBehaviourPunCallbacks
{
    [Header("Screens")]
    public GameObject[] screens;
    private const string mainOptionsName = "MainOptions";
    private const string playOptionsName = "PlayOptions";
    private const string settings = "Settings";
    private const string lobbyName = "LobbyScreen";

    [Header("Play Options Screen")]
    public Button createRoomButton;
    public Button joinRoomButton;

    [Header("Settings Screen")]
    public Slider volumeSlider;
    public GameObject cam;
    private AudioSource audioSource;

    [Header("Lobby Screen")]
    public TextMeshProUGUI playerListText;
    public Button startGameButton;

    [Header("Player Preview")]
    public GameObject playerPreview;
    private Animator animator;
    public TextMeshProUGUI rankScore;

    public GameObject buttonClickAudioObject;
    private AudioSource buttonClickAudio;

    public static Menu instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // set the player preview in a kneeling animation
        animator = playerPreview.GetComponent<Animator>();
        animator.SetBool("InMainMenu", true);

        // can't create room or join rooms until connected to the master
        createRoomButton.interactable = false;
        joinRoomButton.interactable = false;

        // set audio source volume to the sliders default setting
        audioSource = cam.GetComponent<AudioSource>();
        audioSource.volume = volumeSlider.value;

        // set button click audio
        buttonClickAudio = buttonClickAudioObject.GetComponent<AudioSource>();

        // this allows the rank text to appear by setting it everytime we switch from game scene to the menu scene
        UpdateUI(CloudManager.instance.GetRank());
    }

    // called when connection to photon server is successful
    public override void OnConnectedToMaster()
    {
        createRoomButton.interactable = true;
        joinRoomButton.interactable = true;
    }

    // get screen game object using screen name
    public GameObject GetScreen(string screenName)
    {
        return screens.First(screen => screen.name == screenName);
    }

    // set the scene in the menu 
    void SetScreen(GameObject sceneToView)
    {
        foreach (GameObject screen in screens)
        {
            if (sceneToView == screen)
                screen.SetActive(true);
            else
                screen.SetActive(false);
        }
    }

    // called when "Play" Button is pressed
    public void OnPlayButton()
    {
        buttonClickAudio.Play();
        SetScreen(GetScreen(playOptionsName));
        NetworkManager.instance.GetListOfRooms("SqlTypedLobby");
    }

    // called when "Settings" Button is pressed
    public void OnSettingsButton()
    {
        buttonClickAudio.Play();
        SetScreen(GetScreen(settings));
    }

    // called when "BackToMenu" Button is pressed
    public void BackToMainOptions()
    {
        buttonClickAudio.Play();
        SetScreen(GetScreen(mainOptionsName));
    }

    public void OnQuickPlayButton()
    {
        NetworkManager.instance.JoinRandomRoom();
    }

    // called when "Create Room" Button is pressed
    public void OnCreateRoomButton(TMP_InputField roomNameInput)
    {
        buttonClickAudio.Play();
        NetworkManager.instance.CreateRoom(roomNameInput.text);
    }

    // called when "Join Room" Button is pressed
    public void OnJoinRoomButton(TMP_InputField roomNameInput)
    {
        buttonClickAudio.Play();
        NetworkManager.instance.JoinRoom(roomNameInput.text);
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
        SetScreen(GetScreen(lobbyName));
        PhotonNetwork.NickName = CloudManager.instance.GetPlayerName();
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
        buttonClickAudio.Play();
        PhotonNetwork.LeaveRoom();
        SetScreen(GetScreen(playOptionsName));
    }

    // called when the "Start Game" button is pressed
    // only the host can click this button
    public void OnStartGameButton()
    {
        buttonClickAudio.Play();
        // set animator to not be kneeling - entering game now
        animator.SetBool("InMainMenu", false);

        // send an rpc call to all players in the room to load the "Game" scene
        NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, "Game");
    }

    public void AdjustVolume(Slider volume)
    {
        audioSource.volume = volume.value;
    }

    public void UpdateUI(string rank)
    {
        rankScore.text = rank;
    }

    public void OnExitButton()
    {
        Application.Quit();
    }
}
