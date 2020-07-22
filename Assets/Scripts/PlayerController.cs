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

    [Header("Components")]
    public Player photonPlayer;
    public Rigidbody rig;
    public Animator runForwardAnim;
    public Animator jump;
    public Animator runJump;
    public Camera cam;
    public GameObject tagIndicator;
    
    [Header("Info")]
    public float gravity = 1f;
    public float speed = 4f;
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
            rig.isKinematic = true;
            cam.enabled = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        runForwardAnim = GetComponent<Animator>();
        jump = GetComponent<Animator>();
        runJump = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (curTagTime >= GameManager.instance.timeToLose && !GameManager.instance.gameEnded)
            {
                GameManager.instance.gameEnded = true;
                GameManager.instance.photonView.RPC("GameOver", RpcTarget.All, id);
            }
        }

        if (photonView.IsMine)
        {
            float x = Input.GetAxis("Horizontal") * speed;
            float z = Input.GetAxis("Vertical") * speed;

            Vector3 direction = new Vector3(x, 0f, z).normalized;

            if (direction.magnitude >= 0.1f)
            {
                //float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
                //float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                //transform.rotation = Quaternion.Euler(0f, angle, 0f);

                rig.velocity = new Vector3(x, rig.velocity.y, z);
            }

            runForwardAnim.SetBool("isRunningForward", false);
            jump.SetBool("isJump", false);
            runJump.SetBool("isRunJump", false);

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
            {
                runForwardAnim.SetBool("isRunningForward", true);
            }

            if (Input.GetKey(KeyCode.Space) && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)))
            {
                runForwardAnim.SetBool("isRunJump", true);
                TryJump();
            }

            if (Input.GetKey(KeyCode.Space))
            {
                runForwardAnim.SetBool("isJump", true);
                TryJump();
            }
        }

        // track the amount of time we're wearing the hat
        if (tagIndicator.activeInHierarchy)
            curTagTime += Time.deltaTime;
    }

    // check if we're grounded and if so - jump
    void TryJump()
    {
        // create a ray which shoots below us
        Ray ray = new Ray(transform.position, Vector3.down);

        // if we hit something then we're grounded - so then jump
        if (Physics.Raycast(ray, 0.7f))
        {
            rig.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

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
}
