﻿using System.Collections;
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
    private const string CharacterScreenName = "CharacterScreen";
    private const string playOptionsName = "PlayOptions";
    private const string settings = "Settings";
    private const string lobbyName = "LobbyScreen";

    [Header("Play Options Screen")]
    public Button createRoomButton;
    public Button joinRoomButton;
    
    [Header("Character Screen")]
    private GameObject characterChosen;
    
    public TextMeshProUGUI gameTypeTitle;
    private const string rankedGameTitle = "Ranked Game";
    private const string unrankedGameTitle = "Unranked Game";

    [Header("Settings Screen")]
    public Slider volumeSlider;
    public GameObject cam;
    private AudioSource audioSource;

    [Header("Lobby Screen")]
    public TextMeshProUGUI playerListText;
    public Button startGameButton;

    [Header("Player Preview")]
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
        // find already chosen class (this may be found when we come back from a game and go back to the main menu)
        // we use this to preserve the same class he already chose
        GameObject classAlreadyChosen = GameObject.FindGameObjectWithTag("ChosenClass");

        // check if a class was already chosen
        if (classAlreadyChosen != null) {
            // set as the chosen character
            characterChosen = classAlreadyChosen;            
        } 
        else
        {
            // instantiate new character class object and set the chosen character and dont destroy on load to bring to the game scene
            characterChosen = Instantiate(Resources.Load("CharacterPreviewClasses/BerserkerPreview") as GameObject);
            DontDestroyOnLoad(characterChosen);
        }

        InitializeChosenClass("Berserker");

        // set the player preview in a kneeling animation
        animator = characterChosen.GetComponent<Animator>();
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

    /**
     * FUNCTION TOOLS
     * 
     */

    // called when "BackToMenu" Button is pressed
    public void BackToMainOptions()
    {
        buttonClickAudio.Play();
        SetScreen(GetScreen(mainOptionsName));
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

    public void UpdateUI(string rank)
    {
        rankScore.text = rank;
    }

    public void OnExitButton()
    {
        Application.Quit();
    }

    /**
     * CHARACTERS SECTION
     * 
     */

    // called when "Berserker" Button is pressed
    public void OnBerserkerClassChosen()
    {
        Destroy(characterChosen);
        characterChosen = Instantiate(Resources.Load("CharacterPreviewClasses/BerserkerPreview") as GameObject);
        InitializeChosenClass("Berserker");
    }

    // called when "Frost Mage" Button is pressed
    public void OnFrostMageClassChosen()
    {
        Destroy(characterChosen);
        characterChosen = Instantiate(Resources.Load("CharacterPreviewClasses/FrostMagePreview") as GameObject);
        InitializeChosenClass("FrostMage");
    }

    // called when "Ninja" Button is pressed
    public void OnNinjaClassChosen()
    {
        Destroy(characterChosen);
        characterChosen = Instantiate(Resources.Load("CharacterPreviewClasses/NinjaPreview") as GameObject);
        InitializeChosenClass("Ninja");
    }

    // called when "Illusionist" Button is pressed
    public void OnIllusionistClassChosen()
    {
        Destroy(characterChosen);
        characterChosen = Instantiate(Resources.Load("CharacterPreviewClasses/IllusionistPreview") as GameObject);
        InitializeChosenClass("Illusionist");
    }

    // called when "BackToMenu" Button is pressed
    private void InitializeChosenClass(string classAnimBoolVar)
    {
        DontDestroyOnLoad(characterChosen);
        switchClassAnimator(classAnimBoolVar);
    }

    private void switchClassAnimator(string classAnimBoolVar)
    {
        // set the player preview in a kneeling animation
        animator = characterChosen.GetComponent<Animator>();
        animator.SetBool("InMainMenu", true);

        // reset all bool variables in animator and set the chosen class bool variable to true
        // switch state machines in the animator using these values
        animator.SetBool(classAnimBoolVar, true);
        /*string[] classes = { "Berserker", "FrostMage", "Ninja", "Illusionist" };
        foreach (string classInList in classes)
        {
            if(classInList != classAnimBoolVar)
                animator.SetBool(classInList, false);
            else
                animator.SetBool(classAnimBoolVar, true);
        }*/
    }

    /**
     * SETTINGS SECTION
     * 
     */

    public void AdjustVolume(Slider volume)
    {
        audioSource.volume = volume.value;
    }


    /**
     * MAIN OPTIONS SECTION
     * 
     */

    // called when "Play" Button is pressed
    public void OnPlayButton()
    {
        buttonClickAudio.Play();
        SetScreen(GetScreen(playOptionsName));
        NetworkManager.instance.GetListOfRooms();
    }

    // called when "Character" Button is pressed
    public void OnCharacterButton()
    {
        buttonClickAudio.Play();
        SetScreen(GetScreen(CharacterScreenName));
    }

    // called when "Settings" Button is pressed
    public void OnSettingsButton()
    {
        buttonClickAudio.Play();
        SetScreen(GetScreen(settings));
    }

    /**
     * PLAY OPTIONS SECTION
     * 
     */

    public void OnQuickPlayButton()
    {
        buttonClickAudio.Play();
        NetworkManager.instance.JoinRandomRoomUnranked();
    }

    public void OnRankedPlayButton()
    {
        NetworkManager.instance.JoinRandomRoomRanked();
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
        if (NetworkManager.instance.rankedGame)
        {
            gameTypeTitle.text = rankedGameTitle;
        }
        else
        {
            gameTypeTitle.text = unrankedGameTitle;    
        }

        // send an rpc call to update all the other clients that this player has joined the room
        // update everyone elses lobby ui
        photonView.RPC("UpdateLobbyUI", RpcTarget.All);
    }

    // called when the "Leave Lobby" button is pressed
    public void OnLeaveLobbyButton()
    {
        buttonClickAudio.Play();
        NetworkManager.instance.LeaveRoom();
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
}
