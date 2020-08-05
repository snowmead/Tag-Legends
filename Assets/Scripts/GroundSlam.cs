using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class GroundSlam : MonoBehaviourPunCallbacks
{
    private void Awake()
    {
        // all players can be affected by this ground slam
        GameManager.instance.isGroundSlamActive = true;
    }

    private void OnDestroy()
    {
        // destroy root game object
        Destroy(gameObject.transform.root.gameObject);

        // set ground slam active to false in the game manager
        // all players can no longer be affected by this ground slam
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
