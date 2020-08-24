using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityCooldownManager : MonoBehaviour
{
    public Ability[] abilities;

    private void Update()
    {
        foreach (Ability ability in abilities)
        {
            if (ability.cooldown > 0)
            {
                ability.cooldownImage.fillAmount = ability.cooldown;

                ability.cooldown -= Time.deltaTime;
            }
        }
    }

    public void StartCooldown(int abilityIndex, float cooldown)
    {
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