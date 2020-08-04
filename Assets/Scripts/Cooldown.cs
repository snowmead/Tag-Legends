using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cooldown : MonoBehaviour
{
    // cooldown time of ability
    /*public float cooldownTime = 7f;

    // next time we can fire the ability
    public float nextFireTime = 0f;*/

    public GameObject ability;
    public GameObject particle;

    private void Update()
    {
        if(particle == null)
        {
            Destroy(ability);
        }

        // if the current time is greater then nextFireTime - cooldown has expired
        /*if (Time.time > nextFireTime)
        {
            ability.SetActive(true);

            // clicked on ability
            if (Input.GetMouseButtonDown(0))
            {
                // next fire time is the current time plus the ability cooldown
                nextFireTime = Time.time + cooldownTime;
            }
        } 
        else
        {
            ability.SetActive(false);
        }*/
    }
}
