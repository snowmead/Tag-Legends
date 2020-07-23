using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmimatorCollider : MonoBehaviour
{
    public GameObject bodyPart;
    public SphereCollider sphereCollider;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        sphereCollider.transform.position = bodyPart.transform.position;
    }
}
