using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FrostNova : MonoBehaviourPunCallbacks
{
    private float currentTime;

    private void Update()
    {
        currentTime += Time.deltaTime;

        if (currentTime > FrostMageAbilities.FrostNovaDurationEffect)
        {
            Destroy(gameObject);
        }
    }
}
