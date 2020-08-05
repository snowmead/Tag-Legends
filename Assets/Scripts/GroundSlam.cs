using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class GroundSlam : MonoBehaviourPunCallbacks
{
    private void Awake()
    {
        GameManager.instance.isGroundSlamActive = true;
    }

    private void OnDestroy()
    {
        GameManager.instance.isGroundSlamActive = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!photonView.IsMine)
        {
            GameObject otherObject = other.transform.root.gameObject;
            if (otherObject.tag == "Player")
            {
                PlayerManager player = otherObject.GetComponent<PlayerManager>();
                player.rig.drag = 20f;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(!photonView.IsMine)
        {
            GameObject otherObject = other.transform.root.gameObject;
            if (otherObject.tag == "Player")
            {
                PlayerManager player = otherObject.GetComponent<PlayerManager>();
                player.rig.drag = 0f;
            }
        }
    }
}
