using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public float speed = 5;

    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("isRunning", false);

        if (Input.GetKey("w"))
        {
            anim.SetBool("isRunning", true);
            transform.position += Vector3.forward * speed * Time.deltaTime;
        }
        if (Input.GetKey("a"))
        {
            anim.SetBool("isRunning", true);
            transform.position += Vector3.left * speed * Time.deltaTime;
        }
        if (Input.GetKey("s"))
        {
            anim.SetBool("isRunning", true);
            transform.position += Vector3.back * speed * Time.deltaTime;
        }
        if (Input.GetKey("d"))
        {
            anim.SetBool("isRunning", true);
            transform.position += Vector3.right * speed * Time.deltaTime;
        }
    }
}
