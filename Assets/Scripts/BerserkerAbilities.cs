using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;

public class BerserkerAbilities : MonoBehaviourPunCallbacks
{
    [Header("Config")]
    public Animator animator;
    public Rigidbody rig;
    public string berserkerAbilityResourceLocation = "Character/Berserker/";

    [Header("Ability config")]
    public string shoutActiveAnimFloatVar = "ShoutActive";
    public float shoutDurationEffect = 10f;

    public static BerserkerAbilities instance;

    private void Awake()
    {
        instance = this;
    }

    public void Leap()
    {
        // Lift character up in ther air before applying velocity, I think friction occurs if this is not done and prevents velocity from being applied
        rig.transform.position = new Vector3(rig.transform.position.x, rig.transform.position.y + 0.5f, rig.transform.position.z);
        
        // Leap
        rig.velocity = new Vector3(transform.forward.x * 10f, 10f, transform.forward.z * 10.0f);
    }

    public void AxeThrow()
    {
        PhotonNetwork.Instantiate(berserkerAbilityResourceLocation + "AxeThrow", transform.position, Quaternion.identity);
    }

    public void GroundSlam()
    {
        animator.SetTrigger("GroundSlam");
        PhotonNetwork.Instantiate(berserkerAbilityResourceLocation + "GroundSlam", transform.position, Quaternion.identity);
    }

    public void Shout()
    {
        animator.SetTrigger("Shout");
        AbilityManager.instance.photonView.RPC("BerserkerShout", RpcTarget.Others);
        PhotonNetwork.Instantiate(berserkerAbilityResourceLocation + "ShoutParticles", transform.position, Quaternion.identity);
    }
}
