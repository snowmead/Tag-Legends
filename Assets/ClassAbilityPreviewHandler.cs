using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClassAbilityPreviewHandler : MonoBehaviour
{
    public GameObject AbilityPreviewCanvas;
    public TextMeshProUGUI AbilityPreviewCanvasTitle;
    public TextMeshProUGUI AbilityPreviewCanvasDescription;
    public AbilityPreview[] AbilityPreviews;
    
    private const int MaxAbilities = 4;
    private AbilityPreview currentAbilityPreview;
    
    private void Start()
    {
        for (int i = 0; i < MaxAbilities; i++)
        {
            // set ability preview canvas from child objects
            AbilityPreviews[i].abilityIndex = i;
            AbilityPreviews[i].AbilityDescription = transform.GetChild(i).GetComponent<AbilityDescription>();
        }
    }

    public void OnClickAbilityPreview(int abilityIndex)
    {
        // don't show the ability preview if the user reclicked on the same ability to hide it
        if(currentAbilityPreview != null)
            AbilityPreviewCanvas.SetActive(abilityIndex != currentAbilityPreview.abilityIndex);
        else
            AbilityPreviewCanvas.SetActive(true);
        
        // set new ability preview
        currentAbilityPreview =
            AbilityPreviews.First(x => x.abilityIndex == abilityIndex);

        AbilityPreviewCanvasTitle.text = currentAbilityPreview.AbilityDescription.AbilityTitle;
        AbilityPreviewCanvasDescription.text = currentAbilityPreview.AbilityDescription.AbilityDesc;
    }
}

[Serializable]
public class AbilityPreview
{
    public int abilityIndex;
    public AbilityDescription AbilityDescription;
}
