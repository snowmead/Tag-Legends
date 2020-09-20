using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeThrow : MonoBehaviour
{
    public GameObject axePrefab;
    private GameObject instantiatedAxe;
    private Rigidbody rig;
    public float speed;

    private void Start()
    {
        // instantiate the axe prefab to be at the starting point
        instantiatedAxe = Instantiate(axePrefab, transform.position, transform.rotation);
        rig = instantiatedAxe.transform.GetChild(0).GetComponent<Rigidbody>();
        rig.velocity = transform.forward * speed;
    }

    void Update()
    {
        instantiatedAxe.transform.Rotate(0, 0, 1000 * Time.deltaTime); //rotates 50 degrees per second around z axis
    }
}
