using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingColorChanger : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        // from the ring game object
        // get the GroundSlamCollider game object child and check if photonView is mine 
        // if it isn't mine - change ring color to differentiate enemy groundslams
        if (!gameObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<GroundSlam>().photonView.IsMine)
        {
            ParticleSystem ps = GetComponent<ParticleSystem>();
            var col = ps.colorOverLifetime;
            col.enabled = true;
            Gradient grad = new Gradient();
            grad.SetKeys(new GradientColorKey[] { new GradientColorKey(Color.blue, 0.0f), new GradientColorKey(Color.red, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) });
            col.color = grad;
        }
    }
}
