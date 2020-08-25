using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerserkerAbilities : MonoBehaviourPunCallbacks
{
    [Header("Config")]
    public Animator animator;

    public Rigidbody rig;
    public const string BERSERKER_ABILTIES_RESOURCE_LOCATION = "Character/Berserker/";
    private AbilityCooldownManager abilityCooldownManager;

    [Header("Leap Ability Config")]
    private const int LEAP_ABILITY_INDEX = 0;
    private const float LEAP_COOLDOWN = 5f;

    [Header("Axe Ability Config")]
    private const int AXE_ABILITY_INDEX = 1;
    private const float AXE_COOLDOWN = 5f;
    public const float AXE_DURATION_EFFECT = 5f;

    [Header("Ground Slam Ability Config")]
    private const int GROUND_SLAM_ABILITY_INDEX = 2;
    private const float GROUND_SLAM_COOLDOWN = 5f;
    public const float GROUND_SLAM_DURATION_EFFECT = 5f;

    [Header("Shout Ability Config")]
    private const int SHOUT_ABILITY_INDEX = 3;
    private const float SHOUT_COOLDOWN = 15f;
    public const float SHOUT_DURATION_EFFECT = 10f;
    public const string SHOUT_ACTIVE_ANIMATOR_FLOAT_VAR = "ShoutActive";

    public AudioSource leapAudioSource;
    public AudioSource axeThrowAudioSource;
    public AudioSource groundSlamAudioSource;
    public AudioSource shoutAudioSource;

    public static BerserkerAbilities instance;

    private void Awake()
    {
        instance = this;
        abilityCooldownManager = gameObject.GetComponent<AbilityCooldownManager>();
    }

    public void Leap()
    {
        // start the ability cooldown
        abilityCooldownManager.StartCooldown(LEAP_ABILITY_INDEX, LEAP_COOLDOWN);

        leapAudioSource.Play();

        // Lift character up in ther air before applying velocity, I think friction occurs if this is not done and prevents velocity from being applied
        rig.transform.position = new Vector3(rig.transform.position.x, rig.transform.position.y + 0.5f, rig.transform.position.z);

        // Leap
        rig.velocity = new Vector3(transform.forward.x * 8f, 8f, transform.forward.z * 8.0f);
    }

    public void AxeThrow()
    {
        // start the ability cooldown
        abilityCooldownManager.StartCooldown(AXE_ABILITY_INDEX, AXE_COOLDOWN);

        axeThrowAudioSource.Play();

        PhotonNetwork.Instantiate(
            BERSERKER_ABILTIES_RESOURCE_LOCATION + "Axe",
            transform.position + Vector3.up,
            gameObject.transform.rotation);
    }

    public void GroundSlam()
    {
        // start the ability cooldown
        abilityCooldownManager.StartCooldown(GROUND_SLAM_ABILITY_INDEX, GROUND_SLAM_COOLDOWN);

        groundSlamAudioSource.Play();
        animator.SetTrigger("GroundSlam");
        PhotonNetwork.Instantiate(
            BERSERKER_ABILTIES_RESOURCE_LOCATION + "GroundSlam",
            transform.position,
            Quaternion.identity);
    }

    public void Shout()
    {
        // start the ability cooldown
        abilityCooldownManager.StartCooldown(SHOUT_ABILITY_INDEX, SHOUT_COOLDOWN);

        shoutAudioSource.Play();
        animator.SetTrigger("Shout");
        // Set all other players feared active state
        AbilityRpcReceiver.instance.photonView.RPC("BerserkerShout", RpcTarget.Others);
        PhotonNetwork.Instantiate(
            BERSERKER_ABILTIES_RESOURCE_LOCATION + "ShoutParticles",
            transform.position,
            Quaternion.identity);
    }
}