using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine.Events;

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
    public GameObject characterChosen;

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

    [Header("Class Buttons and Text")]
    public GameObject berserkerButton;
    public GameObject frostMageButton;
    public GameObject illusionistButton;
    public GameObject ninjaButton;
    public GameObject berserkerButtonSelection;
    public GameObject frostMageButtonSelection;
    public GameObject illusionistButtonSelection;
    public GameObject ninjaButtonSelection;
    public GameObject berserkerButtonIcon;
    public GameObject frostMageButtonIcon;
    public GameObject illusionistButtonIcon;
    public GameObject ninjaButtonIcon;
    public TextMeshProUGUI berserkerListText;
    public TextMeshProUGUI frostMageListText;
    public TextMeshProUGUI illusionistListText;
    public TextMeshProUGUI ninjaListText;

    [Header("Button configs")]
    public GameObject buttonClickAudioObject;
    private AudioSource buttonClickAudio;
    public LeanTweenType inType;
    public LeanTweenType outType;

    public UnityEvent onCompleteCallback;

    public static Menu instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // set button click audio
        buttonClickAudio = buttonClickAudioObject.GetComponent<AudioSource>();

        // find already chosen class (this may be found when we come back from a game and go back to the main menu)
        // we use this to preserve the same class he already chose
        GameObject classAlreadyChosen = GameObject.FindGameObjectWithTag("Player");

        // check if a class was already chosen
        if (classAlreadyChosen != null)
        {
            // set as the chosen character
            characterChosen = classAlreadyChosen;
            // set character animator
            switchClassAnimator(characterChosen.name.Replace("(Clone)", string.Empty));
        }
        else
        {
            // instantiate new character class object and set the chosen character and dont destroy on load to bring to the game scene
            characterChosen = Instantiate(Resources.Load("Character/Berserker/Berserker") as GameObject);
            InitializeChosenClass("Berserker");
            DontDestroyOnLoad(characterChosen);
        }

        // set the player preview in a kneeling animation
        animator = characterChosen.GetComponent<Animator>();
        animator.SetBool("InMainMenu", true);

        // can't create room or join rooms until connected to the master
        createRoomButton.interactable = false;
        joinRoomButton.interactable = false;

        // set audio source volume to the sliders default setting
        audioSource = cam.GetComponent<AudioSource>();
        audioSource.volume = volumeSlider.value;

        // this allows the rank text to appear by setting it everytime we switch from game scene to the menu scene
        UpdateUI(CloudManager.instance.GetRank().ToString());
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
        characterChosen = Instantiate(Resources.Load("Character/Berserker/Berserker") as GameObject);
        ResetClassSelection();
        InitializeChosenClass("Berserker");
        berserkerButtonSelection.SetActive(true);
        BounceButtonEffect(berserkerButton);
    }

    // called when "Frost Mage" Button is pressed
    public void OnFrostMageClassChosen()
    {
        Destroy(characterChosen);
        characterChosen = Instantiate(Resources.Load("Character/FrostMage/FrostMage") as GameObject);
        ResetClassSelection();
        InitializeChosenClass("FrostMage");
        frostMageButtonSelection.SetActive(true);
        BounceButtonEffect(frostMageButton);
    }

    // called when "Ninja" Button is pressed
    public void OnNinjaClassChosen()
    {
        Destroy(characterChosen);
        characterChosen = Instantiate(Resources.Load("Character/Ninja/Ninja") as GameObject);
        ResetClassSelection();
        InitializeChosenClass("Ninja");
        ninjaButtonSelection.SetActive(true);
        BounceButtonEffect(ninjaButton);
    }

    // called when "Illusionist" Button is pressed
    public void OnIllusionistClassChosen()
    {
        Destroy(characterChosen);
        characterChosen = Instantiate(Resources.Load("Character/Illusionist/Illusionist") as GameObject);
        ResetClassSelection();
        InitializeChosenClass("Illusionist");
        illusionistButtonSelection.SetActive(true);
        BounceButtonEffect(illusionistButton);
    }

    private void BounceButtonEffect(GameObject button)
    {
        // bounce lean tween effect
        LeanTween.scale(
            button,
            new Vector2(1.1f, 1.1f),
            .25f)
            .setOnComplete(OnComplete)
            .setEase(inType);
    }

    private void OnComplete()
    {
        if(onCompleteCallback != null)
        {
            onCompleteCallback.Invoke();
        }
    }

    private void ResetClassSelection()
    {
        berserkerButtonSelection.SetActive(false);
        frostMageButtonSelection.SetActive(false);
        illusionistButtonSelection.SetActive(false);
        ninjaButtonSelection.SetActive(false);

        berserkerListText.color = new Color32(190, 181, 182, 255);
        frostMageListText.color = new Color32(190, 181, 182, 255);
        illusionistListText.color = new Color32(190, 181, 182, 255);
        ninjaListText.color = new Color32(190, 181, 182, 255);

        berserkerButtonIcon.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        frostMageButtonIcon.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        illusionistButtonIcon.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        ninjaButtonIcon.GetComponent<Image>().color = new Color32(255, 255, 255, 255);

        LeanTween.scale(
            berserkerButton,
            new Vector2(1, 1),
            .25f)
            .setOnComplete(OnComplete)
            .setEase(outType);
        LeanTween.scale(
            frostMageButton,
            new Vector2(1, 1),
            .25f)
            .setOnComplete(OnComplete)
            .setEase(outType);
        LeanTween.scale(
            illusionistButton,
            new Vector2(1, 1),
            .25f)
            .setOnComplete(OnComplete)
            .setEase(outType);
        LeanTween.scale(
            ninjaButton,
            new Vector2(1, 1),
            .25f)
            .setOnComplete(OnComplete)
            .setEase(outType);
    }

    // Remove all components on the player prefab to set up the character preview
    public void SetupPlayerPreview(string className)
    {
        characterChosen.GetComponent<PlayerController>().enabled = false;
        characterChosen.GetComponent<PlayerManager>().enabled = false;
        switch (className)
        {
            case "Berserker":
                characterChosen.GetComponent<BerserkerAbilities>().enabled = false;
                SetChosenCharacterTextColorAndIcon(berserkerListText, berserkerButtonIcon);
                break;
            case "FrostMage":
                characterChosen.GetComponent<FrostMageAbilities>().enabled = false;
                SetChosenCharacterTextColorAndIcon(frostMageListText, frostMageButtonIcon);
                break;
            case "Ninja":
                characterChosen.GetComponent<NinjaAbilities>().enabled = false;
                SetChosenCharacterTextColorAndIcon(ninjaListText, ninjaButtonIcon);
                break;
            case "Illusionist":
                characterChosen.GetComponent<IllusionistAbilities>().enabled = false;
                SetChosenCharacterTextColorAndIcon(illusionistListText, illusionistButtonIcon);
                break;
        }

        characterChosen.GetComponent<PhotonView>().enabled = false;
        characterChosen.GetComponent<PhotonAnimatorView>().enabled = false;
        characterChosen.GetComponent<PhotonTransformView>().enabled = false;
        characterChosen.transform.GetChild(2).gameObject.SetActive(false);
        characterChosen.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }

    private void SetChosenCharacterTextColorAndIcon(TextMeshProUGUI characterText, GameObject icon)
    {
        characterText.color = new Color32(251, 244, 190, 255);
        icon.GetComponent<Image>().color = new Color32(252, 228, 132, 255);
    }

    // called when "BackToMenu" Button is pressed
    private void InitializeChosenClass(string className)
    {
        buttonClickAudio.Play();
        DontDestroyOnLoad(characterChosen);
        switchClassAnimator(className);
        SetupPlayerPreview(className);
    }

    private void switchClassAnimator(string classAnimBoolVar)
    {
        // set the player preview in a kneeling animation
        animator = characterChosen.GetComponent<Animator>();
        animator.SetBool("InMainMenu", true);

        // reset all bool variables in animator and set the chosen class bool variable to true
        // switch state machines in the animator using these values
        animator.SetBool(classAnimBoolVar, true);
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

    private void ButtonBounceEffectTween()
    {
        
    }
}
