using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class GroundSlam : MonoBehaviourPunCallbacks
{
    private void OnDestroy()
    {
        // destroy root game object
        Destroy(gameObject.transform.root.gameObject);
    }
}
