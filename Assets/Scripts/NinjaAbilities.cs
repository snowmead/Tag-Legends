using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaAbilities : MonoBehaviour
{
    public Rigidbody rig;
    public const string NINJA_ABILTIES_RESOURCE_LOCATION = "Character/Ninja/";
    private AbilityCooldownManager abilityCooldownManager;

    [Header("Poof Ability Config")]
    private const int POOF_ABILITY_INDEX = 0;
    private const float POOF_COOLDOWN = 5f;

    [Header("Shuriken Ability Config")]
    private const int SHURIKEN_ABILITY_INDEX = 1;
    private const float SHURIKEN_COOLDOWN = 5f;
    public const float SHURIKEN_DURATION_EFFECT = 5f;

    [Header("Tele Kunai Ability Config")]
    private const int TELE_KUNAI_ABILITY_INDEX = 2;
    private const float TELE_KUNAI_COOLDOWN = 5f;

    [Header("Blade Wind Ability Config")]
    private const int BLADE_WIND_ABILITY_INDEX = 3;
    private const float BLADE_WIND_COOLDOWN = 15f;
    public const float BLADE_WIND_DURATION_EFFECT = 10f;
    public const string BLADE_WIND_ACTIVE_ANIMATOR_FLOAT_VAR = "BladeWind";

    public AudioSource poofAudioSource;
    public AudioSource shurikenAudioSource;
    public AudioSource teleKunaiAudioSource;
    public AudioSource bladeWindAudioSource;

    public static NinjaAbilities instance;

    private void Awake()
    {
        instance = this;
        abilityCooldownManager = gameObject.GetComponent<AbilityCooldownManager>();
    }

    public void Poof()
    {
        // start the ability cooldown
        abilityCooldownManager.StartCooldown(POOF_ABILITY_INDEX, POOF_COOLDOWN);

        poofAudioSource.Play();
    }

    public void Shuriken()
    {
        // start the ability cooldown
        abilityCooldownManager.StartCooldown(SHURIKEN_ABILITY_INDEX, SHURIKEN_COOLDOWN);

        shurikenAudioSource.Play();
    }

    public void TeleKunai()
    {
        // start the ability cooldown
        abilityCooldownManager.StartCooldown(TELE_KUNAI_ABILITY_INDEX, TELE_KUNAI_COOLDOWN);

        teleKunaiAudioSource.Play();
    }

    public void BladeWind()
    {
        // start the ability cooldown
        abilityCooldownManager.StartCooldown(BLADE_WIND_ABILITY_INDEX, BLADE_WIND_COOLDOWN);

        bladeWindAudioSource.Play();
    }
}
