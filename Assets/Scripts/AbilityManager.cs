using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AbilityManager : MonoBehaviourPunCallbacks
{
    public static AbilityManager instance;
    public GameObject shoutParticles;

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
