using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeThrow : MonoBehaviour
{
    private void OnDestroy()
    {
        Destroy(transform.root.gameObject);
    }
}
