using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class GroundSlam : MonoBehaviourPunCallbacks
{
    private float currentTime;

    private void Update()
    {
        currentTime += Time.deltaTime;

        if (currentTime > BerserkerAbilities.GroundSlamDurationEffect)
        {
            Destroy(gameObject);
        }
    }
    
    private void OnDestroy()
    {
        // destroy root game object
        Destroy(gameObject.transform.root.gameObject);
    }
}
