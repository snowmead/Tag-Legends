using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrostMageAbilities : MonoBehaviour
{
    public Animator animator;
    public Rigidbody rig;
    public const string FROSTMAGE_ABILTIES_RESOURCE_LOCATION = "Character/FrostMage/";
    private AbilityCooldownManager abilityCooldownManager;

    [Header("Frost Nova Ability Config")]
    private const int FROST_NOVA_ABILITY_INDEX = 0;
    private const float FROST_NOVA_COOLDOWN = 5f;

    [Header("Ice Bolt Ability Config")]
    private const int ICE_BOLT_ABILITY_INDEX = 1;
    private const float ICE_BOLT_COOLDOWN = 5f;
    public const float ICE_BOLT_DURATION_EFFECT = 5f;

    [Header("Ice Block Ability Config")]
    private const int ICE_BLOCK_ABILITY_INDEX = 2;
    private const float ICE_BLOCK_COOLDOWN = 5f;
    public const float ICE_BLOCK_DURATION_EFFECT = 5f;

    [Header("Freezing Winds Ability Config")]
    private const int FREEZING_WINDS_ABILITY_INDEX = 3;
    private const float FREEZING_WINDS_COOLDOWN = 15f;
    public const float FREEZING_WINDS_DURATION_EFFECT = 10f;
    public const string FREEZING_WINDS_ACTIVE_ANIMATOR_FLOAT_VAR = "FreezingWindsActive";

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
        abilityCooldownManager.StartCooldown(FROST_NOVA_ABILITY_INDEX, FROST_NOVA_COOLDOWN);

        frostNovaAudioSource.Play();

        PhotonNetwork.Instantiate(
            FROSTMAGE_ABILTIES_RESOURCE_LOCATION + "FrostNova",
            transform.position,
            Quaternion.identity);
    }

    public void IceBolt()
    {
        // start the ability cooldown
        abilityCooldownManager.StartCooldown(ICE_BOLT_ABILITY_INDEX, ICE_BOLT_COOLDOWN);

        iceBoltAudioSource.Play();

        PhotonNetwork.Instantiate(
            FROSTMAGE_ABILTIES_RESOURCE_LOCATION + "IceBolt",
            transform.position + Vector3.up,
            gameObject.transform.rotation);
    }

    public void IceBlock()
    {
        // start the ability cooldown
        abilityCooldownManager.StartCooldown(ICE_BLOCK_ABILITY_INDEX, ICE_BLOCK_COOLDOWN);

        iceBlockAudioSource.Play();
    }

    public void FreezingWinds()
    {
        // start the ability cooldown
        abilityCooldownManager.StartCooldown(FREEZING_WINDS_ABILITY_INDEX, FREEZING_WINDS_COOLDOWN);

        freezingWindsAudioSource.Play();
    }
}
