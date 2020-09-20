using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Axe : MonoBehaviourPunCallbacks
{
    public float lifeTime = 5f;
    public float startTime;
    public float endAxeTime;
    public float speed = 10f;
    public float currentTime;

    private Rigidbody rig;

    private void Awake()
    {
        // set start and end time of axe
        startTime = Time.deltaTime;
        endAxeTime = startTime + lifeTime;

        // get rigidbody
        rig = transform.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // set current time
        currentTime += Time.deltaTime;

        // destroy axe after lifetime exceeded
        if(currentTime > endAxeTime)
        {
            Destroy(gameObject);
        }

        // add velocity and rotation
        rig.velocity = transform.forward * speed;
        transform.Rotate(0, 0, 1000 * Time.deltaTime); //rotates 50 degrees per second around z 
    }

    private void OnTriggerEnter(Collider other)
    {
        // is the axe mine?
        if (!photonView.IsMine)
        {
            // did the axe hit a player?
            if (other.gameObject.transform.root.gameObject.CompareTag("Player"))
            {
                // did the axe hit me?
                if (other.gameObject.transform.root.gameObject.GetComponent<PlayerManager>().photonView.IsMine)
                {
                    // stun the player
                    PlayerManager playerManager = other.gameObject.transform.root.gameObject.GetComponent<PlayerManager>();
                    playerManager.AxeStunned();
                }
            }
        }
    }
}
