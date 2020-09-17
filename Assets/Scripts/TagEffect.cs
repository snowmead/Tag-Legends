using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagEffect : MonoBehaviour
{
    private void OnDestroy()
    {
        Destroy(transform.root.gameObject);
    }
}
