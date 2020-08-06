using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerserkerAbilities : MonoBehaviourPunCallbacks
{
    [Header("Config")]
    public Animator animator;
    public Rigidbody rig;
    private string berserkerAbilityResourceLocation = "Character/Berserker/";

    [Header("Ability Modifiers")]
    public float shoutDurationEffect = 10f;

    public static BerserkerAbilities instance;

    private void Awake()
    {
        instance = this;
    }

    public void Leap()
    {
         
    }

    public void AxeThrow()
    {
        PhotonNetwork.Instantiate(berserkerAbilityResourceLocation + "AxeThrow", transform.position, Quaternion.identity);
    }

    public void GroundSlam()
    {
        //int groundSlamHash = Animator.StringToHash("GroundSlam");
        animator.SetTrigger("GroundSlam");

        PhotonNetwork.Instantiate(berserkerAbilityResourceLocation + "GroundSlam", transform.position, Quaternion.identity);
        /*Debug.Log("ground slam hash " + groundSlamHash);
        Debug.Log("animator full path hash " + animator.GetCurrentAnimatorStateInfo(0).fullPathHash);

        while (animator.GetCurren tAnimatorStateInfo(0).fullPathHash == groundSlamHash)
        {
            Debug.Log(animator.GetCurrentAnimatorStateInfo(0).fullPathHash);
            rig.isKinematic = true;
        }

        rig.isKinematic = false;*/
    }

    public void Shout()
    {
        animator.SetTrigger("Shout");
        AbilityManager.instance.photonView.RPC("BerserkerShout", RpcTarget.Others);
        PhotonNetwork.Instantiate(berserkerAbilityResourceLocation + "ShoutParticles", transform.position, Quaternion.identity);
    }
}
