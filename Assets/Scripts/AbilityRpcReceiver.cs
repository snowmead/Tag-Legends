using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AbilityRpcReceiver : MonoBehaviourPunCallbacks
{
    public static AbilityRpcReceiver instance;
    private PlayerManager playerManager;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        playerManager = gameObject.GetComponent<PlayerManager>();
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
    
    [PunRPC]
    public void FreezingWinds()
    {
        gameObject.GetComponent<PlayerManager>().SetFreezingWindsState();
    }
    
    [PunRPC]
    public void IceBlock(int playerId)
    { 
        GameManager.Instance.GetPlayer(playerId).StartIceBlock();
    }
}
