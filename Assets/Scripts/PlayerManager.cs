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

    public static PlayerManager instance;


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
        }
        else
        {
            GameObject.Find("UselessCamera").SetActive(false);
            cam.gameObject.SetActive(true);
            rig.isKinematic = false;
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

    private void OnTriggerEnter(Collider collision)
    {
        // only the client controlling this player will check for collisions
        // client based collision detection
        if (!photonView.IsMine)
            return;

        // did we hit another player's tag range circle?
        if (collision.gameObject.CompareTag("TagCircle"))
        {
            // are they tag?
            if (collision.gameObject.GetComponentInParent<PlayerManager>().id == GameManager.instance.taggedPlayer)
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
                // update player vingettes in UI
                GameManager.instance.photonView.RPC("UpdateInGameUI", RpcTarget.All);
                // start the game for all players
                GameManager.instance.photonView.RPC("StartCountdown", RpcTarget.All);
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
    }

    // called when all the players are ready to play and the countdown was done
    [PunRPC]
    public void BeginGame()
    {
        // start the game
        // update function will now have player movement
        startGame = true;
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
