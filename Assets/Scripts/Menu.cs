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
    private const string MAIN_OPTIONS_NAME = "MainOptions";
    private const string CHARACTER_SCREEN_NAME = "CharacterScreen";
    private const string PLAY_OPTIONS_NAME = "PlayOptions";
    private const string SETTINGS_NAME = "SettingsPopUp";

    [Header("Character Screen")]
    public GameObject characterChosen;
    private string characterChosenName;

    [Header("Settings Screen")]
    public GameObject cam;
    private AudioSource audioSource;

    [Header("Lobby Screen")]
    public TextMeshProUGUI playerListText;
    public Button startGameButton;

    [Header("Custom Game")] 
    public GameObject CustomGamePanel;
    public GameObject CustomGameNameObject;
    public TextMeshProUGUI CustomGameName;
    public GameObject SearchForCustomGame;
    public Button CreateCustomGameButton;
    public Button JoinCustomGameButton;
    public TextMeshProUGUI numberOfPlayersInCustomGame;
    public TextMeshProUGUI numberOfPlayersDenominator;
    public int CustomGameMaxNumberOfPlayers;
    public GameObject MaxPlayersDropdown;
    private TMP_Dropdown MaxPlayersCustomGameDropdown;

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

    [Header("Class Abilities Previews")] 
    public GameObject BerserkerAbilitiesPreview;
    public GameObject FrostMageAbilitiesPreview;
    public GameObject NinjaAbilitiesPreview;
    public GameObject IllusionistAbilitiesPreview;
    public GameObject AbilityPreviewCanvas;
    
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
            SwitchClassAnimator(characterChosen.name.Replace("(Clone)", string.Empty));
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

        // set audio source volume to the sliders default setting
        audioSource = cam.GetComponent<AudioSource>();

        // this allows the rank text to appear by setting it everytime we switch from game scene to the menu scene
        UpdateUI(CloudManager.instance.GetRank().ToString());
        RankDisplayer.instance.UpdateRankDisplay();

        searchForGame.SetActive(false);
        
        //Fetch the Dropdown GameObject
        MaxPlayersCustomGameDropdown = MaxPlayersDropdown.GetComponent<TMP_Dropdown>();
    }

    /**
     * FUNCTION TOOLS
     * 
     */

    // called when "BackToMenu" Button is pressed
    public void BackToMainOptions()
    {
        buttonClickAudio.Play();
        SetScreen(GetScreen(MAIN_OPTIONS_NAME));
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
        SetupClassAbilityPreview();
        
        characterChosen.GetComponent<PlayerController>().enabled = false;
        characterChosen.GetComponent<PlayerManager>().enabled = false;
        switch (className)
        {
            case BERSERKER_NAME:
                characterChosen.GetComponent<BerserkerAbilities>().enabled = false;
                SetChosenCharacterTextColorAndIcon(berserkerListText, berserkerButtonIcon);
                BerserkerAbilitiesPreview.SetActive(true);
                break;
            case FROSTMAGE_NAME:
                characterChosen.GetComponent<FrostMageAbilities>().enabled = false;
                SetChosenCharacterTextColorAndIcon(frostMageListText, frostMageButtonIcon);
                FrostMageAbilitiesPreview.SetActive(true);
                break;
            case NINJA_NAME:
                characterChosen.GetComponent<NinjaAbilities>().enabled = false;
                SetChosenCharacterTextColorAndIcon(ninjaListText, ninjaButtonIcon);
                NinjaAbilitiesPreview.SetActive(true);
                break;
            case ILLUSIONIST_NAME:
                characterChosen.GetComponent<IllusionistAbilities>().enabled = false;
                SetChosenCharacterTextColorAndIcon(illusionistListText, illusionistButtonIcon);
                IllusionistAbilitiesPreview.SetActive(true);
                break;
        }

        characterChosen.GetComponent<PhotonView>().enabled = false;
        characterChosen.GetComponent<PhotonAnimatorView>().enabled = false;
        characterChosen.GetComponent<PhotonTransformView>().enabled = false;
        characterChosen.transform.GetChild(2).gameObject.SetActive(false);
        characterChosen.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }

    private void SetupClassAbilityPreview()
    {
        AbilityPreviewCanvas.SetActive(false);
        
        BerserkerAbilitiesPreview.SetActive(false);
        FrostMageAbilitiesPreview.SetActive(false);
        NinjaAbilitiesPreview.SetActive(false);
        IllusionistAbilitiesPreview.SetActive(false);
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
        SwitchClassAnimator(className);
        SetupPlayerPreview(className);
        DontDestroyOnLoad(characterChosen);
    }

    private void SwitchClassAnimator(string classAnimBoolVar)
    {
        // set the player preview in a kneeling animation
        animator = characterChosen.GetComponent<Animator>();

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
        SetScreen(GetScreen(PLAY_OPTIONS_NAME));
    }

    // called when "Character" Button is pressed
    public void OnCharacterButton()
    {
        buttonClickAudio.Play();
        SetScreen(GetScreen(CHARACTER_SCREEN_NAME));
    }

    // called when "Settings" Button is pressed
    public void OnSettingsButton()
    {
        buttonClickAudio.Play();

        if (GetScreen(SETTINGS_NAME).activeInHierarchy)
        {
            // open the settings screen
            GetScreen(SETTINGS_NAME).SetActive(false);

            // dont show the character - so the character doesn't appear over the settings
            characterChosen.transform.GetChild(0).gameObject.SetActive(true);
        } else 
        {
            // close the settings screen
            GetScreen(SETTINGS_NAME).SetActive(true);
            // show the character again
            characterChosen.transform.GetChild(0).gameObject.SetActive(false);
        } 
    }

    public void OnCustomGameButton()
    {
        // show the custom game panel
        CustomGamePanel.SetActive(true);
        
        // set the class character behind the view of the custom game view
        characterChosen.transform.position = 
            new Vector3(
                characterChosen.transform.position.x,
                characterChosen.transform.position.y,
                -10);
    }

    public void OnCustomGameExitButton()
    {
        // set create and join buttons non interactable
        CreateCustomGameButton.interactable = true;
        JoinCustomGameButton.interactable = true;
        
        // don't show the custom game panel
        CustomGamePanel.SetActive(false);
        
        // don't show the search for game text
        SearchForCustomGame.SetActive(false);
        
        // set the custom game input text field to enabled
        // value can be changed
        CustomGameNameObject.GetComponent<TMP_InputField>().interactable = true;
        
        // leave the room if you are in one
        if(NetworkManager.instance.CurrentRoom() != null)
            NetworkManager.instance.LeaveRoom();
        
        // allow the player to choose how many players in the custom game
        MaxPlayersCustomGameDropdown.interactable = true;
        
        // set the class character back to a visible position
        characterChosen.transform.position = 
            new Vector3(characterChosen.transform.position.x,
                characterChosen.transform.position.y,
                0.5f);
    }

    public void OnCustomGameNameChange(string gameName)
    {
        CustomGameName.text = gameName;
    }
    
    public void OnCustomGameCreateButton()
    {
        // set create and join buttons non interactable
        CreateCustomGameButton.gameObject.SetActive(false);
        JoinCustomGameButton.gameObject.SetActive(false);

        // show the search for game text
        SearchForCustomGame.SetActive(true);
        
        // set the custom game input text field to disabled
        // value cannot be changed
        CustomGameNameObject.GetComponent<TMP_InputField>().interactable = false;

        // setup to create an unranked game
        NetworkManager.instance.rankedGame = false;
        
        // create the custom game room
        NetworkManager.instance.CreateRoom(CustomGameName.text, GetMaxNumberOfPlayersFromDropdown());
        
        // disable dropdown
        MaxPlayersCustomGameDropdown.interactable = false;
    }
    
    public void OnCustomGameJoinButton()
    {
        // set create and join buttons non interactable
        CreateCustomGameButton.gameObject.SetActive(false);
        JoinCustomGameButton.gameObject.SetActive(false);
        
        // show the search for game text
        SearchForCustomGame.SetActive(true);
        
        // set the custom game input text field to disabled
        // value cannot be changed
        CustomGameNameObject.GetComponent<TMP_InputField>().interactable = false;
        
        // join the custom game room
        NetworkManager.instance.JoinRoom(CustomGameName.text);
        
        // disable dropdown
        MaxPlayersCustomGameDropdown.interactable = false;
    }

    public void UpdateCustomGamePlayersDenominator(int maxPlayers)
    {
        numberOfPlayersDenominator.text = "/" + maxPlayers;
    }

    public void OnCancelCustomGameSearchButton()
    {
        // set create and join buttons non interactable
        CreateCustomGameButton.gameObject.SetActive(true);
        JoinCustomGameButton.gameObject.SetActive(true);
        
        // don't show the search for game text
        SearchForCustomGame.SetActive(false);
        
        // set the custom game input text field to enabled
        // value can be changed
        CustomGameNameObject.GetComponent<TMP_InputField>().interactable = true;
        
        // allow the player to choose how many players in the custom game
        MaxPlayersCustomGameDropdown.interactable = true;
        
        // leave the room
        NetworkManager.instance.LeaveRoom();
    }

    public int GetMaxNumberOfPlayersFromDropdown()
    {
        switch (MaxPlayersCustomGameDropdown.value)
        {
            case 0:
                CustomGameMaxNumberOfPlayers = 2;
                break;
            case 1:
                CustomGameMaxNumberOfPlayers = 3;
                break;
            case 2:
                CustomGameMaxNumberOfPlayers = 4;
                break;
            case 3:
                CustomGameMaxNumberOfPlayers = 5;
                break;
        }

        return CustomGameMaxNumberOfPlayers;
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
        buttonClickAudio.Play();
        NetworkManager.instance.JoinRandomRoomRanked();
        searchForGame.SetActive(true);
        EnableOrDisbalePlayScreenButtons(false);
    }

    // called when "Create Room" Button is pressed
    public void OnCreateRoomButton(TMP_InputField roomNameInput)
    {
        buttonClickAudio.Play();
        // pass 0 max number of players because in the CreateRoom function, it will add the default 5 player room
        // only custom games will allow the player to change the max number of players
        NetworkManager.instance.CreateRoom(roomNameInput.text, 0);
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
    
    // updates the lobby UI to show player list and host buttons
    [PunRPC]
    public void UpdateLobbyUI()
    {
        numberOfPlayersInLobby.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString();
        numberOfPlayersInCustomGame.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString();
    }
}
