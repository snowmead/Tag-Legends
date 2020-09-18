using System;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerserkerAbilities : MonoBehaviourPunCallbacks
{
    [Header("Config")]
    public Animator animator;

    public Rigidbody rig;
    public const string BerserkerAbiltiesResourceLocation = "Character/Berserker/";
    private AbilityCooldownManager abilityCooldownManager;

    [Header("Leap Ability Config")]
    private const int LeapAbilityIndex = 0;
    private const float LeapCooldown = 10f;

    [Header("Axe Ability Config")]
    private const int AxeAbilityIndex = 1;
    private const float AxeCooldown = 10f;
    public const float AxeDurationEffect = 2f;

    [Header("Ground Slam Ability Config")]
    public const string GroundSlamTag = "GroundSlam";
    private const int GroundSlamAbilityIndex = 2;
    private const float GroundSlamCooldown = 20f;
    public const float GroundSlamDurationEffect = 5f;

    [Header("Shout Ability Config")] 
    public const string FearedParticlesObjectName = "FearedParticles";
    private const int ShoutAbilityIndex = 3;
    private const float ShoutCooldown = 30f;
    public const float ShoutDurationEffect = 3f;
    public const string ShoutActiveAnimatorFloatVar = "ShoutActive";

    public AudioSource leapAudioSource;
    public AudioSource axeThrowAudioSource;
    public AudioSource groundSlamAudioSource;
    public AudioSource shoutAudioSource;
    
    private void Awake()
    {
        abilityCooldownManager = gameObject.GetComponent<AbilityCooldownManager>();
    }

    public void Leap()
    {
        // start the ability cooldown
        abilityCooldownManager.StartCooldown(LeapAbilityIndex, LeapCooldown);

        leapAudioSource.Play();

        // Lift character up in ther air before applying velocity, I think friction occurs if this is not done and prevents velocity from being applied
        rig.transform.position = new Vector3(rig.transform.position.x, rig.transform.position.y + 0.5f, rig.transform.position.z);

        // Leap
        rig.velocity = new Vector3(transform.forward.x * 6f, 6f, transform.forward.z * 6.0f);
    }

    public void AxeThrow()
    {
        // start the ability cooldown
        abilityCooldownManager.StartCooldown(AxeAbilityIndex, AxeCooldown);

        axeThrowAudioSource.Play();

        PhotonNetwork.Instantiate(
            BerserkerAbiltiesResourceLocation + "Axe",
            transform.position + Vector3.up,
            gameObject.transform.rotation);
    }

    public void GroundSlam()
    {
        // start the ability cooldown
        abilityCooldownManager.StartCooldown(GroundSlamAbilityIndex, GroundSlamCooldown);

        groundSlamAudioSource.Play();

        Vector3 position = new Vector3(transform.position.x, 0.1f, transform.position.z);

        PhotonNetwork.Instantiate(
            BerserkerAbiltiesResourceLocation + "GroundSlam",
            position,
            gameObject.transform.rotation);
    }

    public void Shout()
    {
        // start the ability cooldown
        abilityCooldownManager.StartCooldown(ShoutAbilityIndex, ShoutCooldown);

        shoutAudioSource.Play();
        
        // Set all other players feared active state
        AbilityRpcReceiver.instance.photonView.RPC("BerserkerShout", RpcTarget.Others);
        PhotonNetwork.Instantiate(
            BerserkerAbiltiesResourceLocation + "ShoutParticles",
            transform.position,
            Quaternion.identity);
    }
}