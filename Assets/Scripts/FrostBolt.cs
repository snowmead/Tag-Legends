using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrostNova1 : MonoBehaviour
{
    private void OnDestroy()
    {
        Destroy(transform.root.gameObject);
    }
}
