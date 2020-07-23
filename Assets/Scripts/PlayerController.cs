using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Globalization;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    [HideInInspector]
    public int id;
    public float curTagTime;
    public bool startGame = false;

    [Header("Components")]
    public Player photonPlayer;
    public Rigidbody rig;
    public Animator animator;
    public Camera cam;
    public GameObject tagIndicator;
    private Vector3 inputVector;

    [SerializeField]
    private bool grounded;
    [SerializeField]
    private LayerMask groundLayer;

    [Header("Info")]
    public const float gravity = 1f;
    public const float acceleration = 0.00005f;
    public const float speedConstant = 0.15f;
    public float speed = 0f;

    public float jumpForce;
    public float turnSmoothTime = 0.1f;

    // called when the player object is instantiated
    [PunRPC]
    public void Initialize(Player player)
    {
        photonPlayer = player;
        id = player.ActorNumber;

        GameManager.instance.players[id - 1] = this;

        // tag the first player
        if (id == 1)
            GameManager.instance.TagPlayer(id, true);

        // if this isn't our local player - disable physics and camera
        // the other players sync their own position accross the network
        if (!photonView.IsMine)
        {
            cam.enabled = false;
        }

        rig.isKinematic = true;
    }

    // start is called before the first frame update
    void Start()
    {

    }

    // update is called once per frame
    void Update()
    {
        if (startGame)
        {
            // only the master client decides when the game has ended
            if (PhotonNetwork.IsMasterClient)
            {
                // check if the curTagTime is greater then the max time allowed before losing
                if (curTagTime >= GameManager.instance.timeToLose && !GameManager.instance.gameEnded)
                {
                    // end the game for all players
                    GameManager.instance.gameEnded = true;
                    GameManager.instance.photonView.RPC("GameOver", RpcTarget.All, id);
                }
            }

            // only move my player
            if (photonView.IsMine)
            {
                // get x and z input axis
                inputVector = new Vector3(Input.GetAxis("Horizontal"), transform.position.y, Input.GetAxis("Vertical"));

                // set animator parameters for player animations
                animator.SetFloat("Speed", inputVector.z);
                animator.SetFloat("Turn", -inputVector.x);
                
                // check if my player is grounded
                grounded = Physics.Raycast(transform.position + Vector3.up, Vector3.down, 1.1f, groundLayer);

                // only jump when i press the space bar and if I'm grounded
                if (Input.GetButtonDown("Jump") && grounded)
                {
                    Jump();
                }
            }

            // track the amount of time we're tagged
            if (tagIndicator.activeInHierarchy)
                curTagTime += Time.deltaTime;
        } 
        // only the master client decides when to start the game
        else if (PhotonNetwork.IsMasterClient)
        {
            // check if all players have initialized their player in the game
            if (GameManager.instance.players.Length == PhotonNetwork.PlayerList.Length && !GameManager.instance.countdownStarted)
            {
                // start the game for all players
                GameManager.instance.photonView.RPC("StartCountdown", RpcTarget.All);
            }
        }
    }

    private void Jump()
    {
        animator.SetTrigger("Jump");
    }

    // check if we're grounded and if so - jump
    /*void TryJump()
    {
        // create a ray which shoots below us
        Ray ray = new Ray(transform.position, Vector3.down);

        // if we hit something then we're grounded - so then jump
        if (Physics.Raycast(ray, 0.7f))
        {
            rig.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }*/

    public void TagPlayer(bool tagged)
    {
        if (tagged)
            tagIndicator.SetActive(true);
        else
            tagIndicator.SetActive(false);
    }

    void OnCollisionEnter(Collision collision)
    {
        // only the client controlling this player will check for collisions
        // client based collision detection
        if (!photonView.IsMine)
            return;

        // did we hit another player?
        if (collision.gameObject.CompareTag("Player"))
        {
            // do they have the hat?
            if (GameManager.instance.GetPlayer(collision.gameObject).id == GameManager.instance.taggedPlayer)
            {
                // can we get the hat?
                if (GameManager.instance.CanGetTagged())
                {
                    // give us the hat
                    GameManager.instance.photonView.RPC("TagPlayer", RpcTarget.All, id, false);
                }
            }
        }
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

    // called when all the players are ready to play and the countdown was done
    [PunRPC]
    public void BeginGame()
    {
        Debug.Log("BeginGame was called");
        startGame = true;
        if (photonView.IsMine)
        {
            rig.isKinematic = false;
        }
    }
}
