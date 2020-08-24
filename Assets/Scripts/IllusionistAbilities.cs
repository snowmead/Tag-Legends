using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class IllusionistAbilities : MonoBehaviour
{
    public GameObject body;
    public string illusionistAbilityResourceLocation = "Character/Illusionist/";
    private AbilityCooldownManager abilityCooldownManager;

    [Header("Invisible Ability Config")]
    private const int INVISIBLE_ABILITY_INDEX = 0;
    private const float INVISIBLE_COOLDOWN = 5f;
    public const float INVISIBLE_TIME_DURATION_EFFECT = 5f;
    public float endInvisibleTime = 0f;

    [Header("Mind Melt Ability Config")]
    private const int MIND_MELT_ABILITY_INDEX = 1;
    private const float MIND_MELT_COOLDOWN = 5f;
    public const float MIND_MELT_DURATION_EFFECT = 5f;

    [Header("Charm Ability Config")]
    private const int CHARM_ABILITY_INDEX = 2;
    private const float CHARM_COOLDOWN = 5f;
    public const float CHARM_DURATION_EFFECT = 5f;

    [Header("MultiClone Ability Config")]
    private const int MULTI_CLONE_ABILITY_INDEX = 3;
    private const float MULTI_CLONE_COOLDOWN = 15f;
    public const float MULTI_CLONE_DURATION_EFFECT = 10f;

    public AudioSource invisibleAudioSource;
    public AudioSource mindMeltAudioSource;
    public AudioSource charmAudioSource;
    public AudioSource multiCloneAudioSource;

    public float currentTime;

    private void Awake()
    {
        abilityCooldownManager = gameObject.GetComponent<AbilityCooldownManager>();
    }

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
        abilityCooldownManager.StartCooldown(INVISIBLE_ABILITY_INDEX, INVISIBLE_COOLDOWN);
        invisibleAudioSource.Play();

        body.SetActive(false);
        endInvisibleTime = currentTime + INVISIBLE_TIME_DURATION_EFFECT;
    }

    public void MindMelt()
    {
        abilityCooldownManager.StartCooldown(MIND_MELT_ABILITY_INDEX, MIND_MELT_COOLDOWN);
        mindMeltAudioSource.Play();
    }

    public void Charm()
    {
        abilityCooldownManager.StartCooldown(CHARM_ABILITY_INDEX, CHARM_COOLDOWN);
        charmAudioSource.Play();
    }

    public void MultiClone()
    {
        abilityCooldownManager.StartCooldown(MULTI_CLONE_ABILITY_INDEX, MULTI_CLONE_COOLDOWN);
        multiCloneAudioSource.Play();
    }
}
