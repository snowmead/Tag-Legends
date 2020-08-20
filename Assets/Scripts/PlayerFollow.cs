using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollow : MonoBehaviour
{

    public Transform PlayerTransform;
    public Vector3 cameraOffset;

    [Range(0.01f, 1.0f)]
    public float SmoothFactor = 1f;

    public bool LookAtPlayer = true;
    public float speed;
    public float input;
    public float mousePos;
    public float screenWidth;
    public float screenHeight;

    // Start is called before the first frame update
    void Start()
    {
        cameraOffset = new Vector3(0, 10f, -7.5f);
        speed = 0.5f;
        screenWidth = Screen.width;
        screenHeight = Screen.height;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 newPos = PlayerTransform.position + cameraOffset;

        transform.position = Vector3.Slerp(transform.position, newPos, SmoothFactor);

        if (LookAtPlayer)
            transform.LookAt(PlayerTransform);
    }
}
