using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CloudOnce;
using TMPro;

public class CloudManager : MonoBehaviour
{
    public static CloudManager instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // If an instance already exists and it's not this one - destroy to avoid duplicate NetworkManager object
        if (instance != null && instance != this)
            gameObject.SetActive(false);
        else
        {
            // Set the instance
            instance = this;
            // Don't destroy NetworkManager game object when switching scenes
            DontDestroyOnLoad(gameObject);

            Cloud.OnInitializeComplete += CloudOnceOnInitializeComplete;
            Cloud.OnCloudLoadComplete += CloudOnceLoadComplete;
            Cloud.Initialize(true, true);
        }
    }

    void CloudOnceOnInitializeComplete()
    {
        //Cloud.OnInitializeComplete -= CloudOnceOnInitializeComplete;
        Cloud.Storage.Load();
    }

    void CloudOnceLoadComplete(bool success)
    {
        Debug.Log("CloudOnce Load Complete");
        Menu.instance.UpdateUI(CloudVariables.RankScore.ToString());
    }

    public void IncreaseRank()
    {
        Debug.Log("Increasing rank");
        CloudVariables.RankScore += 10;
        Save();
    }

    public void DecreaseRank()
    {
        Debug.Log("Decreasing rank");
        CloudVariables.RankScore -= 10;
        Save();
    }

    private void Save()
    {
        Debug.Log("Saving Cloud Storage");
        Cloud.Storage.Save();
    }
}
