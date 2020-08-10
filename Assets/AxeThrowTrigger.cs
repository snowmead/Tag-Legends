using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeThrowTrigger : MonoBehaviour
{
    private void OnParticleCollision(GameObject other)
    {
        Debug.Log(other.name);
    }
}
