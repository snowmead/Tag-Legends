using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Globalization;
using System.Numerics;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    [Header("Components")]
    public Rigidbody rig;
    public Animator animator;

    [SerializeField]
    private bool grounded;
    [SerializeField]
    private LayerMask groundLayer;

    [Header("Info")]
    public const float gravity = 1f;
    public const float acceleration = 0.00005f;
    public const float speedConstant = 0.15f;
    public float speed = 10f;
    public Vector3 inputVector;

    public float jumpForce;
    public float turnSmoothTime = 0.1f;

    public static PlayerController instance;

    public Joystick joystick;

    void Awake()
    {
        instance = this;
    }

    // update is called once per frame
    void Update()
    {
        // check if the game is started
        if (PlayerManager.instance.startGame)
        {
            // only the master client decides when the game has ended
            if (PhotonNetwork.IsMasterClient)
            {
                // check if the curTagTime is greater then the max time allowed before losing
                if (PlayerManager.instance.curTagTime >= GameManager.instance.timeToLose && !GameManager.instance.gameEnded)
                {
                    // end the game for all players
                    GameManager.instance.gameEnded = true;
                    GameManager.instance.photonView.RPC("GameOver", RpcTarget.All, PlayerManager.instance.id);
                }
            }

            // if game ended
            if (GameManager.instance.gameEnded)
            {
                // set player animation to idle
            }

            // only move my player
            if (photonView.IsMine)
            {
                // Get movement vertices
                float horizontal = Input.GetAxis("Horizontal") + joystick.Horizontal;
                float vertical = Input.GetAxis("Vertical") + joystick.Vertical;
                Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
                float targetAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
                inputVector = direction * speed * Time.deltaTime;

                // check if my player is grounded
                grounded = Physics.Raycast(transform.position + Vector3.up, transform.TransformDirection(Vector3.down), 1.2f, groundLayer);

                // Can only move while grounded
                if (grounded)
                {
                    animator.SetBool("Jump", false);
                    // Only move if input was calculated
                    if (inputVector.x < 0 || inputVector.z < 0 || inputVector.x > 0 || inputVector.z > 0)
                    {
                        transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
                        transform.forward = inputVector;
                        rig.velocity = new Vector3(transform.forward.x * speed, rig.velocity.y, transform.forward.z * speed);
                    }

                    // Completely stop moving (velocity-wise) if no input was found
                    if (horizontal == 0f && vertical == 0f)
                    {
                        rig.velocity = new Vector3(0f, rig.velocity.y, 0f);
                    }

                    // Animatioms
                    if(Mathf.Abs(Input.GetAxisRaw("Vertical")) > .1 || Mathf.Abs(Input.GetAxisRaw("Horizontal")) > .1 || Mathf.Abs(joystick.Vertical) > .1 || Mathf.Abs(joystick.Horizontal) > .1) //(rig.velocity.x > 0 || rig.velocity.z > 0 || rig.velocity.x < 0 || rig.velocity.z < 0)
                {
                        animator.SetBool("Sprint", true);
                    }
                    else
                    {
                        animator.SetBool("Sprint", false);
                    }
                }
                else
                {
                    animator.SetBool("Jump", true);
                }
            }

            // track the amount of time we're tagged
            if (PlayerManager.instance.tagIndicator.activeInHierarchy)
                // increase current tag time every second
                PlayerManager.instance.curTagTime += Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameUI.instance.EscapeMenu();
            }
        } 
        // only the master client decides when to start the game
        else if (PhotonNetwork.IsMasterClient)
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
    }

    public void OnJumpButton()
    {
        if (grounded)
        {
            rig.velocity = new Vector3(0f, 6f, 0f);
            animator.SetBool("Jump", true);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }
}
