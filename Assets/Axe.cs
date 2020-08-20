using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : MonoBehaviour
{
    public float lifeTime = 5f;
    public float startTime;

    private void Awake()
    {
        startTime = Time.deltaTime;    
    }

    private void Update()
    {
        // destroy axe after lifetime exceeded
        if(Time.deltaTime - startTime > lifeTime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.transform.root.gameObject.CompareTag("Player"))
        {
            Debug.Log("Hit a player!");
            PlayerManager playerManager = other.gameObject.transform.root.gameObject.GetComponent<PlayerManager>();
            playerManager.AxeStunned();

            Destroy(gameObject);
        }
    }
}
