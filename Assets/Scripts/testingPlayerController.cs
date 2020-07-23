using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testingPlayerController : MonoBehaviour
{
    public Animator animator;
    private Vector3 inputVector;

    [SerializeField]
    private bool grounded;
    [SerializeField]
    private LayerMask groundLayer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        inputVector = new Vector3(Input.GetAxis("Horizontal"), transform.position.y, Input.GetAxis("Vertical"));
        animator.SetFloat("Speed", inputVector.z);
        animator.SetFloat("Turn", -inputVector.x);
        grounded = Physics.Raycast(transform.position + Vector3.up, Vector3.down, 1.1f, groundLayer);
        
        if (Input.GetButtonDown("Jump") && grounded)
        {
            Jump();
        }
    }

    private void Jump()
    {
        animator.SetTrigger("Jump");
    }
}
