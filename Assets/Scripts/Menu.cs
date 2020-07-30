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

    public GameObject buttonClickAudioObject;
    private AudioSource buttonClickAudio;

    private void Start()
    {
        ValidateFirebase();

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
    }

    public void ValidateFirebase()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                FirebaseApp app = Firebase.FirebaseApp.DefaultInstance;

                // Set a flag here to indicate whether Firebase is ready to use by your app.
                Debug.Log("Firebase is ready to use!");
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
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

    // called when Name Input field changes - change the player's nickname
    public void OnPlayerNameUpdate(TMP_InputField playerNameInput)
    {
        PhotonNetwork.NickName = playerNameInput.text;
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

    public void OnExitButton()
    {
        Application.Quit();
    }
}
