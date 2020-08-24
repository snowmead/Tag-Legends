using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityCooldownManager : MonoBehaviour
{
    public Ability[] abilities;

    private void Update()
    {
        // iterate through all the abilities and set their cooldowns
        foreach (Ability ability in abilities)
        {
            if (ability.cooldown > 0)
            {
                // show cooldown circle mask image
                ability.cooldownImage.gameObject.SetActive(true);
                // make the ability button not interactable
                ability.ability.GetComponent<Button>().interactable = false;

                // fill the amount depending on the abilities time
                ability.cooldownImage.fillAmount -= (Time.deltaTime / ability.cooldown);

                // check if we are done the time of the cooldown
                if (ability.cooldownImage.fillAmount <= 0)
                {
                    // set the cooldown to 0
                    ability.cooldown = 0;
                }
            }
            else
            {
                // make the ability button interactable
                ability.ability.GetComponent<Button>().interactable = true;
                // set the cooldown image amount to 1
                ability.cooldownImage.fillAmount = 1;
                // make the cooldown image not visible
                ability.cooldownImage.gameObject.SetActive(false);
            }
        }
    }

    // start an abilities cooldown - called from a [class]Abilities.cs
    public void StartCooldown(int abilityIndex, float cooldown)
    {
        // set the ability cooldown
        GetAbility(abilityIndex).cooldown = cooldown;
    }

    private Ability GetAbility(int abilityIndex)
    {
        return abilities[abilityIndex];
    }
}

[System.Serializable]
public class Ability
{
    public GameObject ability;
    public Image cooldownImage;
    public float cooldown = 0;
}