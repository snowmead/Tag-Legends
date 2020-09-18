using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    [Header("RPC Method Names")]
    private const string TagPlayerMethodName      = "TagPlayer";
    private const string GameOverMethodName       = "GameOver";
    private const string StartCountdownMethodName = "StartCountdown";
    private const string UpdateInGameUIMethodName = "UpdateInGameUI";
    private const string UselessCameraObjectName  = "UselessCamera";

    [HideInInspector]
    public int    id;          // player id
    public float  curTagTime;  // current tag time of player
    public bool   startGame;   // determines if the game started
    public string chosenClass; // Holds player's chosen class
    public GameObject TagTimer;
    private TMP_Text TagTimerText;
    
    [Header("Components")]
    public Player     PhotonPlayer;
    public Rigidbody  rig;
    public Camera     cam;
    public GameObject tagIndicator;
    public GameObject playerUI;
    public GameObject playerUIAbilities;
    public GameObject playerUIJoystic;
    public GameObject body;
    public GameObject TagCollider;

    [Header("General Ability Configs")]
    private const float SlowedRigDrag = 20f;

    [Header("Shout Variables")]
    public bool isShoutAnimationActive;
    public bool isFearedActive;
    public GameObject fearParticles;
    public float startFearedFromShoutAbility;
    public float endFearFromShoutAbility;

    [Header("Axe Variables")]
    public bool isAxeStunned;
    public float startAxeStunned;
    public float endAxeStunned;

    [Header("IceBolt Variables")] 
    public bool isIceBoltFreeze;
    public float startIceBoltStunned;
    public float endIceBoltStunned;

    [Header("IceBlock Variables")] 
    public bool isIceBlock;
    private float startIceBlock;
    private float endIceBlock;

    [Header("Freezing Winds Variables")] 
    public bool isFreezingWindsActive;
    private float startFreezingWindsTimer;
    private float endFreezingWindsTimer;
        
    public static PlayerManager Instance;

    public float currentTime;
    private static readonly int Dead = Animator.StringToHash("dead");

    void Awake()
    {
        Instance = this;
    }

    // called when the player object is instantiated
    [PunRPC]
    public void Initialize(Player player, string activeClass)
    {
        // set network photon player
        PhotonPlayer = player;
        // set player id using photon player actor number
        id = player.ActorNumber;
        // set chosen class
        chosenClass = activeClass;

        // set the amount of players in the game 
        GameManager.Instance.players[id - 1] = this;

        // tag the first player
        if (id == 1)
            GameManager.Instance.TagPlayer(id, true, false);

        // if this isn't our local player - disable physics and camera
        // the other players sync their own position accross the network
        if (!photonView.IsMine)
        {
            // disable all other player's cameras
            cam.gameObject.SetActive(false);
            rig.isKinematic = true;
            playerUI.SetActive(false);
        }
        else
        {
            GameObject.Find(UselessCameraObjectName).SetActive(false);
            cam.gameObject.SetActive(true);
            rig.isKinematic = false;
            playerUI.SetActive(true);
        }
    }

    private void Start()
    {
        TagTimer.SetActive(true);
        TagTimerText = TagTimer.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
        TagTimerText.enabled = true;
    }

    private void Update()
    {
        currentTime += Time.deltaTime;
        
        // check if the curTagTime is greater then the max time allowed before losing
        // check if the game has already ended for me 
        // check if I'm the one who reached the tag limit
        if (curTagTime >= GameManager.Instance.timeToLose && !GameManager.Instance.gameEnded && photonView.IsMine)
        {
            // end the game for all players
            //GameManager.instance.gameEnded = true;
            GameManager.Instance.photonView.RPC(GameOverMethodName, RpcTarget.All, id);
            
            // play dead animation
            gameObject.GetComponent<PlayerController>().animator.SetBool(Dead, true);

            gameObject.GetComponent<PlayerController>().SoundFootSteps.Stop();
        }

        // only the master client decides when to start the game
        if (PhotonNetwork.IsMasterClient)
        {
            // check if all players have initialized their player in the game
            if (GameManager.Instance.players.Length == PhotonNetwork.PlayerList.Length && !GameManager.Instance.countdownStarted)
            {
                // start the game for all players
                GameManager.Instance.photonView.RPC(StartCountdownMethodName, RpcTarget.All);
            }
        }

        // when the game has started start tag timer
        if (startGame)
        {
            // track the amount of time we're tagged
            if (tagIndicator.activeInHierarchy)
            {
                // increase current tag time every second
                curTagTime += Time.deltaTime;
                
                // displayer tag timer above player's head
                int curTagTimeInt = Mathf.RoundToInt(curTagTime);
                int myCurrentTagTime = 50 - curTagTimeInt;
                TagTimerText.text = myCurrentTagTime.ToString();
            }

        }

        if(currentTime > endFearFromShoutAbility)
        {
            isFearedActive = false;
        }

        if (currentTime > endAxeStunned)
        {
            isAxeStunned = false;
        }

        if (currentTime > endIceBoltStunned)
        {
            isIceBoltFreeze = false;
        }
        
        if (currentTime > endIceBlock)
        {
            isIceBlock = false;
        }
        
        if (currentTime > endFreezingWindsTimer)
        {
            isFreezingWindsActive = false;
        }
    }

    // tag a player or remove tag from player
    public void TagPlayer(bool tagged)
    {
        if (tagged)
            tagIndicator.SetActive(true);
        else
            tagIndicator.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        // only the client controlling this player will check for collisions
        // client based collision detection
        if (!photonView.IsMine)
            return;

        if (!GameManager.Instance.gameEnded)
        {
            // did we hit another player's tag range circle?
            if (other.gameObject.CompareTag("TagCircle"))
            {
                // are they tag?
                if (other.gameObject.GetComponentInParent<PlayerManager>().id == GameManager.Instance.taggedPlayer)
                {
                    // can we get tagged?
                    if (GameManager.Instance.CanGetTagged(id))
                    {
                        // get tagged
                        GameManager.Instance.photonView.RPC(TagPlayerMethodName, RpcTarget.All, id, false, false);
                    }
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // only the client controlling this player will check for collisions
        // client based collision detection
        if (!photonView.IsMine)
            return;

        // am I in any slowing effects
        if (SlowingAbilityEffectCheck(other, BerserkerAbilities.GroundSlamTag) || 
            SlowingAbilityEffectCheck(other, FrostMageAbilities.FrostNovaTag))
        {
            // slow myself
            rig.drag = SlowedRigDrag;
        }
        else
        {
            // run at normal speed
            rig.drag = 0f;   
        }
    }
    
    private bool SlowingAbilityEffectCheck(Collider other, string ability)
    {
        // get parent game object
        GameObject otherCollider = other.gameObject.transform.root.gameObject;

        // am I in a ground slam
        if (otherCollider.CompareTag(ability))
        {
            // am I in someone else's ground slam
            return !otherCollider.GetPhotonView().IsMine;
        }

        return false;
    }

    // called when all the players are ready to play and the countdown was done
    [PunRPC]
    public void BeginGame()
    {
        // start the game
        // update function will now have player movement and tag time counter
        startGame = true;
    }

    // sets the player in a feared state from the berserker shout ability
    public void SetFearState()
    {
        // instantiate the feared particles over the network so all players can see that this player is feared
        fearParticles = PhotonNetwork.Instantiate(
            BerserkerAbilities.BerserkerAbiltiesResourceLocation + BerserkerAbilities.FearedParticlesObjectName,
            new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), Quaternion.identity);

        // set shout to active so that the player can't move during this time
        isFearedActive = true;

        // start feared timer
        startFearedFromShoutAbility = currentTime;
        endFearFromShoutAbility = startFearedFromShoutAbility + BerserkerAbilities.ShoutDurationEffect;
    }

    // sets the player in a ice bolt freezed state
    public void SetIceBolt()
    {
        isIceBoltFreeze = true;
        startIceBoltStunned = currentTime;
        endIceBoltStunned = startIceBoltStunned + FrostMageAbilities.IceBoltDurationEffect;
    }

    public void StartIceBlock()
    {
        isIceBlock = true;
        startIceBlock = currentTime;
        endIceBlock = startIceBlock + FrostMageAbilities.IceBlockDurationEffect;
    }
    
    // sets the player in a freezing winds state from the frost mage freezing winds ability
    public void SetFreezingWindsState()
    {
        // instantiate the freezing winds block of ice over the network so all players can see that this player is feared
        fearParticles = PhotonNetwork.Instantiate(
            FrostMageAbilities.FrostMageAbiltiesResourceLocation + FrostMageAbilities.IceBlockResource,
            transform.position, Quaternion.identity);

        // set shout to active so that the player can't move during this time
        isFreezingWindsActive = true;

        // start feared timer
        startFreezingWindsTimer = currentTime;
        endFreezingWindsTimer = startFreezingWindsTimer + FrostMageAbilities.FreezingWindsDurationEffect;
    }

    // sets the berserker player's shout animation state
    // determines if the berserker can move or not depending if he's casting the shout ability
    public void SetShoutAnimationState(bool shoutAnimationState)
    {
        // set berserker shout animation to active so that the berserker can't move while he's casting his shout ability
        isShoutAnimationActive = shoutAnimationState;
    }

    // make the player invisible
    public void PlayerInvisible(bool isInvisible)
    {
        body.SetActive(isInvisible);

        TagTimerText.enabled = isInvisible;
    }

    // set the amount of seconds the player is stunned
    public void AxeStunned()
    {
        isAxeStunned = true;

        startAxeStunned = currentTime;
        endAxeStunned = startAxeStunned + BerserkerAbilities.AxeDurationEffect;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // we want to sync the 'curTagTime' between all clients
        if (stream.IsWriting)
        {
            stream.SendNext(curTagTime);
        }
        else if (stream.IsReading)
        {
            curTagTime = (float)stream.ReceiveNext();
        }
    }
}
