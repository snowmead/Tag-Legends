using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrostMageAbilities : MonoBehaviour
{
    public Animator animator;
    public Rigidbody rig;
    public const string FrostMageAbiltiesResourceLocation = "Character/FrostMage/";
    private AbilityCooldownManager abilityCooldownManager;

    [Header("Frost Nova Ability Config")] 
    public const string FrostNovaTag = "FrostNova";
    private const int FrostNovaAbilityIndex = 0;
    private const float FrostNovaCooldown = 5f;

    [Header("Ice Bolt Ability Config")]
    private const int IceBoltAbilityIndex = 1;
    private const float IceBoltCooldown = 5f;
    public const float IceBoltDurationEffect = 6f;

    [Header("Ice Block Ability Config")]
    private const int IceBlockAbilityIndex = 2;
    private const float IceBlockCooldown = 5f;
    public const float IceBlockDurationEffect = 5f;

    [Header("Freezing Winds Ability Config")]
    private const int FreezingWindsAbilityIndex = 3;
    private const float FreezingWindsCooldown = 15f;
    public const float FreezingWindsDurationEffect = 10f;
    public const string FreezingWindsActiveAnimatorFloatVar = "FreezingWindsActive";

    public AudioSource frostNovaAudioSource;
    public AudioSource iceBoltAudioSource;
    public AudioSource iceBlockAudioSource;
    public AudioSource freezingWindsAudioSource;

    public static FrostMageAbilities instance;

    private void Awake()
    {
        instance = this;
        abilityCooldownManager = gameObject.GetComponent<AbilityCooldownManager>();
    }

    public void FrostNova()
    {
        // start the ability cooldown
        abilityCooldownManager.StartCooldown(FrostNovaAbilityIndex, FrostNovaCooldown);

        frostNovaAudioSource.Play();

        PhotonNetwork.Instantiate(
            FrostMageAbiltiesResourceLocation + "FrostNova",
            transform.position,
            Quaternion.identity);
    }

    public void IceBolt()
    {
        // start the ability cooldown
        abilityCooldownManager.StartCooldown(IceBoltAbilityIndex, IceBoltCooldown);

        iceBoltAudioSource.Play();

        PhotonNetwork.Instantiate(
            FrostMageAbiltiesResourceLocation + "IceBolt",
            transform.position + Vector3.up,
            gameObject.transform.rotation);
    }

    public void IceBlock()
    {
        // start the ability cooldown
        abilityCooldownManager.StartCooldown(IceBlockAbilityIndex, IceBlockCooldown);

        iceBlockAudioSource.Play();
    }

    public void FreezingWinds()
    {
        // start the ability cooldown
        abilityCooldownManager.StartCooldown(FreezingWindsAbilityIndex, FreezingWindsCooldown);

        freezingWindsAudioSource.Play();
    }
}
