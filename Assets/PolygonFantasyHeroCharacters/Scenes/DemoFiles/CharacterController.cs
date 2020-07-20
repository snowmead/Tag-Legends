using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public float speed = 4;
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

    private Vector3 forwardDiagLeft = new Vector3(-0.1f, 0, 0.1f);
    private Vector3 forwardDiagRight = new Vector3(0.1f, 0, 0.1f);
    private Vector3 backwardDiagLeft = new Vector3(-0.1f, 0, -0.1f);
    private Vector3 backwardDiagRight = new Vector3(0.1f, 0, -0.1f);

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
            transform.position += Vector3.forward * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
        {
            runForwardAnim.SetBool("isRunningDiagLeft", true);
            transform.position += forwardDiagLeft * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
        {
            runForwardAnim.SetBool("isRunningDiagRight", true);
            transform.position += forwardDiagRight * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            runLeftAnim.SetBool("isRunningLeft", true);
            transform.position += Vector3.left * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            runBackAnim.SetBool("isRunningBack", true);
            transform.position += Vector3.back * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
        {
            runForwardAnim.SetBool("isRunningBackDiagLeft", true);
            transform.position += backwardDiagLeft * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))
        {
            runForwardAnim.SetBool("isRunningBackDiagRight", true);
            transform.position += backwardDiagRight * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            runRightAnim.SetBool("isRunningRight", true);
            transform.position += Vector3.right * speed * Time.deltaTime;
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
}
