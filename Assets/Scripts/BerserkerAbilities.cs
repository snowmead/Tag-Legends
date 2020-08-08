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

    public Vector3 originPlayerRotation;

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
        originPlayerRotation = gameObject.transform.rotation.eulerAngles;
        bool hitTheGround = true;
        Quaternion axeDirectionRotation = Quaternion.Euler(originPlayerRotation);
        float xAngle = originPlayerRotation.x;

        while (hitTheGround)
        {
            hitTheGround = Physics.Raycast(
                gameObject.transform.position,
                originPlayerRotation,
                out RaycastHit raycastHit,
                15f,
                PlayerController.instance.groundLayer,
                QueryTriggerInteraction.Ignore);

            if (hitTheGround)
            {
                xAngle = originPlayerRotation.x + 10;
                axeDirectionRotation = Quaternion.AngleAxis(xAngle, new Vector3(1, 0, 0));
            }
        }

        PhotonNetwork.Instantiate(
            berserkerAbilityResourceLocation + "AxeThrow", 
            new Vector3(transform.position.x, transform.position.y + 1, transform.position.z),
            axeDirectionRotation);
    }

    public void GroundSlam()
    {
        animator.SetTrigger("GroundSlam");
        PhotonNetwork.Instantiate(berserkerAbilityResourceLocation + "GroundSlam", transform.position, Quaternion.identity);
    }

    public void Shout()
    {
        animator.SetTrigger("Shout");
        // Set all other players feared active state
        AbilityManager.instance.photonView.RPC("BerserkerShout", RpcTarget.Others);
        PhotonNetwork.Instantiate(berserkerAbilityResourceLocation + "ShoutParticles", transform.position, Quaternion.identity);
    }
}
