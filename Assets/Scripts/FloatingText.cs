using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    private new Camera camera;
    
    private void Start()
    {
        camera = Camera.main;
    }

    private void Update ()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - camera.transform.position);
    }
}
