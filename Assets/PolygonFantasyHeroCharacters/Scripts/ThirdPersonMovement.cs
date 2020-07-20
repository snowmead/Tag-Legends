using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;

    public float gravity = 1f;
    public float speed = 4f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    public Animator runForwardAnim;
    public Animator runLeftAnim;
    public Animator runBackAnim;
    public Animator runRightAnim;
    public Animator runDiagLeft;
    public Animator runDiagRight;
    public Animator runBackDiagLeft;
    public Animator runBackDiagRight;
    public Animator jump;
    public Animator runJump;
    public Animator backJump;

    // Start is called before the first frame update
    void Start()
    {
        runForwardAnim = GetComponent<Animator>();
        runLeftAnim = GetComponent<Animator>();
        runBackAnim = GetComponent<Animator>();
        runRightAnim = GetComponent<Animator>();
        runDiagLeft = GetComponent<Animator>();
        runDiagRight = GetComponent<Animator>();
        runBackDiagLeft = GetComponent<Animator>();
        runBackDiagRight = GetComponent<Animator>();
        jump = GetComponent<Animator>();
        runJump = GetComponent<Animator>();
        backJump = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        Vector3 moveDir = Vector3.zero;

        if (controller.isGrounded)
        {
            if (direction.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                controller.Move(moveDir.normalized * speed * Time.deltaTime);
            }

            runForwardAnim.SetBool("isRunningForward", false);
            runLeftAnim.SetBool("isRunningLeft", false);
            runBackAnim.SetBool("isRunningBack", false);
            runRightAnim.SetBool("isRunningRight", false);
            runDiagLeft.SetBool("isRunningDiagLeft", false);
            runDiagRight.SetBool("isRunningDiagRight", false);
            runBackDiagLeft.SetBool("isRunningBackDiagLeft", false);
            runBackDiagRight.SetBool("isRunningBackDiagRight", false);
            jump.SetBool("isJump", false);
            runJump.SetBool("isRunJump", false);
            backJump.SetBool("isBackJump", false);

            if (Input.GetKey(KeyCode.W))
            {
                runForwardAnim.SetBool("isRunningForward", true);
            }
            if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
            {
                runForwardAnim.SetBool("isRunningDiagLeft", true);
            }
            if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
            {
                runForwardAnim.SetBool("isRunningDiagRight", true);
            }
            if (Input.GetKey(KeyCode.A))
            {
                runLeftAnim.SetBool("isRunningForward", true);
            }
            if (Input.GetKey(KeyCode.S))
            {
                runBackAnim.SetBool("isRunningForward", true);
            }
            if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
            {
                runForwardAnim.SetBool("isRunningDiagRight", true);
            }
            if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))
            {
                runForwardAnim.SetBool("isRunningDiagLeft", true);
            }
            if (Input.GetKey(KeyCode.D))
            {
                runRightAnim.SetBool("isRunningForward", true);
            }
            if (Input.GetKey(KeyCode.Space))
            {
                jump.SetBool("isJump", true);
            }
            if (Input.GetKey(KeyCode.Space) && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)))
            {
                runJump.SetBool("isRunJump", true);
            }
            if (Input.GetKey(KeyCode.Space) && Input.GetKey(KeyCode.S))
            {
                backJump.SetBool("isBackJump", true);
            }
        }

        moveDir.y -= gravity * Time.deltaTime;
        controller.Move(moveDir * Time.deltaTime);
    }
}
