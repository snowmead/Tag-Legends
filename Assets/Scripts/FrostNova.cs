using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FrostNova : MonoBehaviourPunCallbacks
{
    private float currentTime;
    private readonly float endFrostNova = 5f;

    private void Update()
    {
        currentTime += Time.deltaTime;

        if (currentTime > endFrostNova)
        {
            Destroy(gameObject);
        }
    }
}
