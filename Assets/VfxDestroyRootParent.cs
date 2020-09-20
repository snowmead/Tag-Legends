using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VfxDestroyRootParent : MonoBehaviour
{
    private void OnDestroy()
    {
        Destroy(gameObject.transform.parent.gameObject);
    }
}
