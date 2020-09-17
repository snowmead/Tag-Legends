using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBlock : MonoBehaviour
{
    private float currentTime;

    private void Update()
    {
        currentTime += Time.deltaTime;

        if (currentTime > FrostMageAbilities.IceBlockDurationEffect)
        {
            Destroy(gameObject);
        }
    }
}
