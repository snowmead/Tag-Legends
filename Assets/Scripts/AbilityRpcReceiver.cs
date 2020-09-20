using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AbilityRpcReceiver : MonoBehaviourPunCallbacks
{
    public static AbilityRpcReceiver Instance;

    private void Awake()
    {
        Instance = this;
    }

    [PunRPC]
    public void IceBolt()
    {
        gameObject.GetComponent<PlayerManager>().SetIceBolt();
    }

    [PunRPC]
    public void IceBlock(int playerId)
    { 
        GameManager.Instance.GetPlayer(playerId).StartIceBlock();
    }
}
