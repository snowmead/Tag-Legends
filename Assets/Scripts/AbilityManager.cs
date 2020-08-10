using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AbilityManager : MonoBehaviourPunCallbacks
{
    public static AbilityManager instance;

    private void Awake()
    {
        instance = this;
    }

    [PunRPC]
    public void BerserkerShout()
    {
        gameObject.GetComponent<PlayerManager>().SetFearState();
    }
}
