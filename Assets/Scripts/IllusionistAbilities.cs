using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class IllusionistAbilities : MonoBehaviour
{
    public string illusionistAbilityResourceLocation = "Character/Illusionist/";

    public void Clone()
    {
        PhotonNetwork.Instantiate(
            illusionistAbilityResourceLocation + "IllusionistClone",
            new Vector3(transform.position.x + 1, transform.position.y, transform.position.z),
            transform.rotation);
    }

    public void MindMelt()
    {

    }

    public void Charm()
    {

    }

    public void MultiClone()
    {

    }
}
