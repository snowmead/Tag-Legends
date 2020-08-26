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
    private const string BERSERKER_NAME = "Berserker";
    private const string FROSTMAGE_NAME = "FrostMage";
    private const string ILLUSIONIST_NAME = "Illusionist";
    private const string NINJA_NAME = "Ninja";

    [Header("Screens")]
    public GameObject[] screens;
    private const string mainOptionsName = "MainOptions";
    private const string CharacterScreenName = "CharacterScreen";
    private const string playOptionsName = "PlayOptions";
    private const string settings = "SettingsPopUp";

    [Header("Play Options Screen")]
    public Button createRoomButton;
    public Button joinRoomButton;

    [Header("Character Screen")]
    public GameObject characterChosen;
    private string characterChosenName;

    public TextMeshProUGUI gameTypeTitle;
    private const string rankedGameTitle = "Ranked Game";
    private const string unrankedGameTitle = "Unranked Game";

    [Header("Settings Screen")]
    public Slider masterVolumeSlider;
    public GameObject cam;
    private AudioSource audioSource;

    [Header("Lobby Screen")]
    public TextMeshProUGUI playerListText;
    public Button startGameButton;

    [Header("Player Preview")]
    public Animator animator;
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

    [Header("Play Screen")]
    public TextMeshProUGUI numberOfPlayersInLobby;
    public GameObject searchForGame;
    public GameObject quickPlayButton;
    public GameObject rankedPlayButton;
    public GameObject backButton;

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
            characterChosen.transform.localScale = new Vector3(2, 2, 2);
            InitializeChosenClass("Berserker");
            DontDestroyOnLoad(characterChosen);
        }

        // set chosen preview class game object active so that we can access it in the main menu
        characterChosen.SetActive(true);

        // set the player preview in a kneeling animation
        animator = characterChosen.GetComponent<Animator>();
        animator.SetBool("InMainMenu", true);

        // can't create room or join rooms until connected to the master
        createRoomButton.interactable = false;
        joinRoomButton.interactable = false;

        // set audio source volume to the sliders default setting
        audioSource = cam.GetComponent<AudioSource>();
        audioSource.volume = masterVolumeSlider.value;

        // this allows the rank text to appear by setting it everytime we switch from game scene to the menu scene
        UpdateUI(CloudManager.instance.GetRank().ToString());
        RankDisplayer.instance.UpdateRankDisplay();

        searchForGame.SetActive(false);
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
        characterChosen.transform.localScale = new Vector3(2, 2, 2);
        characterChosenName = BERSERKER_NAME;
        ResetClassSelection();
        InitializeChosenClass(characterChosenName);
        berserkerButtonSelection.SetActive(true);
        BounceButtonEffect(berserkerButton);
    }

    // called when "Frost Mage" Button is pressed
    public void OnFrostMageClassChosen()
    {
        Destroy(characterChosen);
        characterChosen = Instantiate(Resources.Load("Character/FrostMage/FrostMage") as GameObject);
        characterChosen.transform.localScale = new Vector3(2, 2, 2);
        characterChosenName = FROSTMAGE_NAME;
        ResetClassSelection();
        InitializeChosenClass(characterChosenName);
        frostMageButtonSelection.SetActive(true);
        BounceButtonEffect(frostMageButton);
    }

    // called when "Ninja" Button is pressed
    public void OnNinjaClassChosen()
    {
        Destroy(characterChosen);
        characterChosen = Instantiate(Resources.Load("Character/Ninja/Ninja") as GameObject);
        characterChosen.transform.localScale = new Vector3(2, 2, 2);
        characterChosenName = NINJA_NAME;
        ResetClassSelection();
        InitializeChosenClass(characterChosenName);
        ninjaButtonSelection.SetActive(true);
        BounceButtonEffect(ninjaButton);
    }

    // called when "Illusionist" Button is pressed
    public void OnIllusionistClassChosen()
    {
        Destroy(characterChosen);
        characterChosen = Instantiate(Resources.Load("Character/Illusionist/Illusionist") as GameObject);
        characterChosen.transform.localScale = new Vector3(2, 2, 2);
        characterChosenName = ILLUSIONIST_NAME;
        ResetClassSelection();
        InitializeChosenClass(characterChosenName);
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
            case BERSERKER_NAME:
                characterChosen.GetComponent<BerserkerAbilities>().enabled = false;
                SetChosenCharacterTextColorAndIcon(berserkerListText, berserkerButtonIcon);
                break;
            case FROSTMAGE_NAME:
                characterChosen.GetComponent<FrostMageAbilities>().enabled = false;
                SetChosenCharacterTextColorAndIcon(frostMageListText, frostMageButtonIcon);
                break;
            case NINJA_NAME:
                characterChosen.GetComponent<NinjaAbilities>().enabled = false;
                SetChosenCharacterTextColorAndIcon(ninjaListText, ninjaButtonIcon);
                break;
            case ILLUSIONIST_NAME:
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
        switchClassAnimator(className);
        SetupPlayerPreview(className);
        DontDestroyOnLoad(characterChosen);
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

        if (GetScreen(settings).activeInHierarchy)
        {
            // open the settings screen
            GetScreen(settings).SetActive(false);

            // dont show the character - so the character doesn't appear over the settings
            characterChosen.transform.GetChild(0).gameObject.SetActive(true);
        } else 
        {
            // close the settings screen
            GetScreen(settings).SetActive(true);
            Debug.Log(characterChosen.transform.GetChild(0).gameObject);
            // show the character again
            characterChosen.transform.GetChild(0).gameObject.SetActive(false);
        } 
    }

    /**
     * PLAY OPTIONS SECTION
     * 
     */

    public void OnQuickPlayButton()
    {
        buttonClickAudio.Play();
        NetworkManager.instance.JoinRandomRoomUnranked();
        searchForGame.SetActive(true);
        EnableOrDisbalePlayScreenButtons(false);
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
        PhotonNetwork.NickName = CloudManager.instance.GetPlayerName();

        // send an rpc call to update all the other clients that this player has joined the room
        // update everyone elses lobby ui
        photonView.RPC("UpdateLobbyUI", RpcTarget.All);
    }

    // called when the "Leave Lobby" button is pressed
    public void OnLeaveLobbyButton()
    {
        buttonClickAudio.Play();
        NetworkManager.instance.LeaveRoom();
        searchForGame.SetActive(false);
        EnableOrDisbalePlayScreenButtons(true);
    }

    private void EnableOrDisbalePlayScreenButtons(bool isEnabled)
    {
        quickPlayButton.GetComponent<Button>().interactable = isEnabled;
        rankedPlayButton.GetComponent<Button>().interactable = isEnabled;
        backButton.GetComponent<Button>().interactable = isEnabled;
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

    public void OnVolumeSlider()
    {
        audioSource.volume = masterVolumeSlider.value;
    }

    // updates the lobby UI to show player list and host buttons
    [PunRPC]
    public void UpdateLobbyUI()
    {
        numberOfPlayersInLobby.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString();
    }
}
