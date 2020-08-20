using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassHandler : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.transform.root.gameObject.tag == "Player")
        {
            PlayerManager playerManager = other.gameObject.GetComponent<PlayerManager>();
            playerManager.PlayerInvisible(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.transform.root.gameObject.tag == "Player")
        {
            PlayerManager playerManager = other.gameObject.GetComponent<PlayerManager>();
            playerManager.PlayerInvisible(true);
        }
    }
}
