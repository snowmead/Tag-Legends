using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeThrow : MonoBehaviour
{
    public GameObject axePrefab;

    public float speed;

    private void Awake()
    {
        transform.position = new Vector3(0, 1.2f, 1.2f);
        Debug.Log(transform.position);
    }

    private void Start()
    {
        Debug.Log(transform.position);

        // instantiate the axe prefab to be at the starting point
        GameObject axe = Instantiate(axePrefab, transform.position, transform.rotation);
        axe.transform.GetChild(0).GetComponent<Rigidbody>().velocity = transform.forward * speed;
    }
}
