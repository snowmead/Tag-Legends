using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrostMageAbilities : MonoBehaviourPunCallbacks
{
    [Header("Config")]
    public Animator animator;
    public Rigidbody rig;
    public string FrostMageAbilityResourceLocation = "Character/FrostMage/";

    [Header("Freezing Winds Config")]
    public string freezingWindsAnimFloatVar = "FreezingWindsActive";
    public float freezingWindsDurationEffect = 10f;
    public AudioSource iceBlockAudioSource;
    public AudioSource frostBoltAudioSource;
    public AudioSource FrostNovaAudioSource;
    public AudioSource freezingWindsAudioSource;
    
    [Header("Frost Bolt Ability Config")]
    public float maxDistance = 10f;
    public LayerMask layerMask;

    public static FrostMageAbilities instance;

    private void Awake()
    {
        instance = this;
    }

    public void IceBlock()
    {
        //iceBlockAudioSource.Play();

        // Lift character up in ther air before applying velocity, I think friction occurs if this is not done and prevents velocity from being applied
        //rig.transform.position = new Vector3(rig.transform.position.x, rig.transform.position.y + 0.5f, rig.transform.position.z);

        // Leap
        //rig.velocity = new Vector3(transform.forward.x * 10f, 10f, transform.forward.z * 10.0f);
    }

    public void Frostbolt()
    {
        frostBoltAudioSource.Play();

        Vector3 originPoint = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        Physics.Raycast(originPoint, gameObject.transform.rotation.eulerAngles, out RaycastHit raycastHitInfo, maxDistance, layerMask, QueryTriggerInteraction.Ignore);

        Debug.Log(LayerMask.LayerToName(raycastHitInfo.collider.gameObject.layer));

        PhotonNetwork.Instantiate(
            FrostMageAbilityResourceLocation + "FrostBolt",
            new Vector3(transform.position.x, transform.position.y + 1, transform.position.z),
            gameObject.transform.rotation);
    }

    public void FrostNova()
    {
        FrostNovaAudioSource.Play();
        animator.SetTrigger("FrostNova");
        PhotonNetwork.Instantiate(
            FrostMageAbilityResourceLocation + "FrostNova",
            transform.position,
            Quaternion.identity);
    }

    public void FreezingWinds()
    {
        freezingWindsAudioSource.Play();
        animator.SetTrigger("FreezingWinds");
        // Set all other players feared active state
        AbilityManager.instance.photonView.RPC("FrostMageFreezingWinds", RpcTarget.Others);
        PhotonNetwork.Instantiate(
            FrostMageAbilityResourceLocation + "FreezingWindsParticles",
            transform.position,
            Quaternion.identity);
    }
}
