using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerserkerAbilities : MonoBehaviourPunCallbacks
{
    private string berserkerAbilityResourceLocation = "Character/Berserker/";

    public void Leap()
    {
         
    }

    public void AxeThrow()
    {
        PhotonNetwork.Instantiate(berserkerAbilityResourceLocation + "AxeThrow", transform.position, Quaternion.identity);
    }

    public void GroundSlam()
    {
        PhotonNetwork.Instantiate(
            berserkerAbilityResourceLocation + "GroundSlam", transform.position, 
            Quaternion.Euler(-90, Quaternion.identity.eulerAngles.y, Quaternion.identity.eulerAngles.z)
            );
    }

    public void Shout()
    {
        PhotonNetwork.Instantiate(berserkerAbilityResourceLocation + "Shout", transform.position, Quaternion.identity);
    }
}
