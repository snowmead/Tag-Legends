using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrostMageAbilities : MonoBehaviour
{
    public const string FrostMageAbiltiesResourceLocation = "Character/FrostMage/";

    private PlayerManager playerManager;
    private AbilityCooldownManager abilityCooldownManager;
    
    [Header("Frost Nova Ability Config")] 
    public const string FrostNovaTag = "FrostNova";
    private const int FrostNovaAbilityIndex = 0;
    private const float FrostNovaCooldown = 20f;
    public const float FrostNovaDurationEffect = 5f;
    private const string FrostNovaResource = "FrostNova";

    [Header("Ice Bolt Ability Config")]
    private const int IceBoltAbilityIndex = 1;
    private const float IceBoltCooldown = 10f;
    public const float IceBoltDurationEffect = 3f;
    private const string IceBoltResource = "IceBolt";

    [Header("Ice Block Ability Config")]
    private const int IceBlockAbilityIndex = 2;
    private const float IceBlockCooldown = 10f;
    public const float IceBlockDurationEffect = 3f;
    public const string IceBlockResource = "IceBlock";

    [Header("Freezing Winds Ability Config")]
    private const int FreezingWindsAbilityIndex = 3;
    private const float FreezingWindsCooldown = 30f;
    public const float FreezingWindsDurationEffect = 3f;
    
    private void Awake()
    {
        playerManager = gameObject.GetComponent<PlayerManager>();
        abilityCooldownManager = gameObject.GetComponent<AbilityCooldownManager>();
    }

    public void FrostNova()
    {
        // start the ability cooldown
        abilityCooldownManager.StartCooldown(FrostNovaAbilityIndex, FrostNovaCooldown);

        //frostNovaAudioSource.Play();

        PhotonNetwork.Instantiate(
            FrostMageAbiltiesResourceLocation + FrostNovaResource,
            transform.position,
            Quaternion.identity);
    }

    public void IceBolt()
    {
        // start the ability cooldown
        abilityCooldownManager.StartCooldown(IceBoltAbilityIndex, IceBoltCooldown);

        //iceBoltAudioSource.Play();

        PhotonNetwork.Instantiate(
            FrostMageAbiltiesResourceLocation + IceBoltResource,
            transform.position + Vector3.up,
            gameObject.transform.rotation);
    }

    public void IceBlock()
    {
        // start the ability cooldown
        abilityCooldownManager.StartCooldown(IceBlockAbilityIndex, IceBlockCooldown);

        //iceBlockAudioSource.Play();
        
        PhotonNetwork.Instantiate(
            FrostMageAbiltiesResourceLocation + IceBlockResource,
            transform.position,
            gameObject.transform.rotation);
        
        AbilityRpcReceiver.instance.photonView.RPC("IceBlock", RpcTarget.Others, playerManager.id);

        playerManager.StartIceBlock();
    }

    public void FreezingWinds()
    {
        // start the ability cooldown
        abilityCooldownManager.StartCooldown(FreezingWindsAbilityIndex, FreezingWindsCooldown);

        //freezingWindsAudioSource.Play();
        
        AbilityRpcReceiver.instance.photonView.RPC("FreezingWinds", RpcTarget.Others);
    }
}
