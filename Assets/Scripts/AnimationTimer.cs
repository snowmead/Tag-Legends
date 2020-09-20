using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTimer : MonoBehaviour
{
    private float currentTime;
    public GameObject vfxAnimation;
    public float delay = 3f;

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;

        if (currentTime > delay)
        {
            vfxAnimation.SetActive(true);
        }
    }
}
