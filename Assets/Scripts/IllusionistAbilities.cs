using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class IllusionistAbilities : MonoBehaviour
{
    public string illusionistAbilityResourceLocation = "Character/Illusionist/";

    [Header("Invisible Config")]
    public GameObject body;
    public float invisibleTime = 5f;
    public float endInvisibleTime = 0f;

    public float currentTime;

    private void Update()
    {
        currentTime += Time.deltaTime;
        if(currentTime > endInvisibleTime)
        {
            body.SetActive(true);
        }
    }

    public void Invisible()
    {
        body.SetActive(false);
        endInvisibleTime = currentTime + invisibleTime;
    }

    public void MindMelt()
    {

    }

    public void Charm()
    {

    }

    public void MultiClone()
    {

    }
}
