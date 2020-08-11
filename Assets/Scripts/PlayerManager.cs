using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    [HideInInspector]
    public int id;                  // player id
    public float curTagTime;        // current tag time of player
    public bool startGame = false;  // determines if the game started

    [Header("Components")]
    public Player photonPlayer;
    public Rigidbody rig;
    public Camera cam;
    public GameObject tagIndicator;
    public GameObject playerUI;

    [Header("Shout Variables")]
    public bool isShoutAnimationActive;
    public bool isFearedActive = false;
    public GameObject fearParticles;
    public float startFearedFromShoutAbility;
    public float endFearFromShoutAbility;

    public static PlayerManager instance;

    public float currentTime;

    void Awake()
    {
        instance = this;
    }

    // called when the player object is instantiated
    [PunRPC]
    public void Initialize(Player player)
    {
        // set network photon player
        photonPlayer = player;
        // set player id using photon player actor number
        id = player.ActorNumber;

        // set the amount of players in the game 
        GameManager.instance.players[id - 1] = this;

        // tag the first player
        if (id == 1)
            GameManager.instance.TagPlayer(id, true);

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
            GameObject.Find("UselessCamera").SetActive(false);
            cam.gameObject.SetActive(true);
            rig.isKinematic = false;
            playerUI.SetActive(true);
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

        // did we hit another player's tag range circle?
        if (other.gameObject.CompareTag("TagCircle"))
        {
            // are they tag?
            if (other.gameObject.GetComponentInParent<PlayerManager>().id == GameManager.instance.taggedPlayer)
            {
                // can we get tagged?
                if (GameManager.instance.CanGetTagged())
                {
                    // get tagged
                    GameManager.instance.photonView.RPC("TagPlayer", RpcTarget.All, id, false);
                }
            }
        }
    }

    private void Update()
    {
        currentTime += Time.deltaTime;

        // only the master client decides when the game has ended
        if (PhotonNetwork.IsMasterClient)
        {
            // check if the curTagTime is greater then the max time allowed before losing
            if (curTagTime >= GameManager.instance.timeToLose && !GameManager.instance.gameEnded)
            {
                // end the game for all players
                GameManager.instance.gameEnded = true;
                GameManager.instance.photonView.RPC("GameOver", RpcTarget.All, PlayerManager.instance.id);
            }
        }

        // only the master client decides when to start the game
        if (PhotonNetwork.IsMasterClient)
        {
            // check if all players have initialized their player in the game
            if (GameManager.instance.players.Length == PhotonNetwork.PlayerList.Length && !GameManager.instance.countdownStarted)
            {
                // start the game for all players
                GameManager.instance.photonView.RPC("StartCountdown", RpcTarget.All);

                // update player vingettes in UI
                GameManager.instance.photonView.RPC("UpdateInGameUI", RpcTarget.All);
            }
        }

        // when the game has started start tag timer
        if (startGame)
        {
            // track the amount of time we're tagged
            if (tagIndicator.activeInHierarchy)
                // increase current tag time every second
                curTagTime += Time.deltaTime;
        }

        if(currentTime > endFearFromShoutAbility)
        {
            isFearedActive = false;
        }
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
            BerserkerAbilities.instance.berserkerAbilityResourceLocation + "FearedParticles",
            new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), Quaternion.identity);

        // set shout to active so that the player can't move during this time
        isFearedActive = true;

        // start feared timer
        startFearedFromShoutAbility = currentTime;
        endFearFromShoutAbility = startFearedFromShoutAbility + BerserkerAbilities.instance.shoutDurationEffect;
    }

    // sets the berserker player's shout animation state
    // determines if the berserker can move or not depending if he's casting the shout ability
    public void SetShoutAnimationState(bool shoutAnimationState)
    {
        // set berserker shout animation to active so that the berserker can't move while he's casting his shout ability
        isShoutAnimationActive = shoutAnimationState;
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
