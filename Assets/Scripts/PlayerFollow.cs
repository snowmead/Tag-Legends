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
    private float timeCounter = 0; 
    private float touchDir = 0.0f;
    public float radius;

    // Start is called before the first frame update
    void Start()
    {
        cameraOffset = transform.position - PlayerTransform.position;
        float radius = cameraOffset.y + 2.0f;
        speed = 0.5f;
        screenWidth = Screen.width;
        screenHeight = Screen.height;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        mousePos = Input.mousePosition.y;
        Vector3 newPos = PlayerTransform.position + cameraOffset;

        transform.position = Vector3.Slerp(transform.position, newPos, SmoothFactor);

        if (LookAtPlayer)
            transform.LookAt(PlayerTransform);

        if(Input.GetMouseButton(0) && Input.mousePosition.y > (Screen.height * 0.5f))
        {
            float radius = cameraOffset.y + 2.0f;
            input = Input.GetAxis("Mouse X");
            speed = Mathf.Abs(Input.GetAxisRaw("Mouse X")) + 1.0f;
            
            if (input > 0)
            {
                timeCounter += speed * Time.deltaTime;
                cameraOffset.x = Mathf.Cos(timeCounter) * radius;
                cameraOffset.z = Mathf.Sin(timeCounter) * radius;
            }
            
            if(input < 0)
            {
                timeCounter -= speed * Time.deltaTime;
                cameraOffset.x = Mathf.Cos(timeCounter) * radius;
                cameraOffset.z = Mathf.Sin(timeCounter) * radius;
            }

        }

        if (Input.touchCount > 0)
        {            
            if (Input.GetTouch(0).position.x > 0.0f && Input.GetTouch(0).position.y > (Screen.height * 0.5f))
            {
                touchDir = 1.0f;
            }
            else if (Input.GetTouch(0).position.x < 0.0f && Input.GetTouch(0).position.y > (Screen.height * 0.5f))
            {
                touchDir = -1.0f;
            }
            else
            {
                touchDir = 0.0f;
            }

            if (touchDir > 0.0f)
            {
                speed = Mathf.Abs(touchDir) + 1.0f;
                timeCounter += speed * Time.deltaTime;
                cameraOffset.x = Mathf.Cos(timeCounter) * cameraOffset.y;
                cameraOffset.z = Mathf.Sin(timeCounter) * cameraOffset.y;
            }

            if (touchDir < 0.0f)
            {
                speed = Mathf.Abs(touchDir) + 1.0f;
                timeCounter -= speed * Time.deltaTime;
                cameraOffset.x = Mathf.Cos(timeCounter) * cameraOffset.y;
                cameraOffset.z = Mathf.Sin(timeCounter) * cameraOffset.y;
            }

        }
    }
}
