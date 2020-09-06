using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AbilityRpcReceiver : MonoBehaviourPunCallbacks
{
    public static AbilityRpcReceiver instance;

    private void Awake()
    {
        instance = this;
    }

    [PunRPC]
    public void BerserkerShout()
    {
        gameObject.GetComponent<PlayerManager>().SetFearState();
    }
    
    [PunRPC]
    public void IceBolt()
    {
        gameObject.GetComponent<PlayerManager>().SetIceBolt();
    }
}
