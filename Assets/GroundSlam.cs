using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSlam : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("IN ENTER");
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("IN STAY");
        other.gameObject.GetComponent<Rigidbody>().angularDrag = 5;
    }
}
