using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FrostNova : MonoBehaviourPunCallbacks
{
    private void OnDestroy()
    {
        // destroy root game object
        Destroy(gameObject.transform.root.gameObject);
    }
}
