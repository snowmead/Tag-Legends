using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CloudOnce;

public class CloudManager : MonoBehaviour
{
    public static CloudManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
            gameObject.SetActive(false);
        else { 
            instance = this;
            // Don't destroy NetworkManager game object when switching scenes
            DontDestroyOnLoad(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // initialize and load cloudonce
        Cloud.OnInitializeComplete += CloudOnceOnInitializeComplete;
        Cloud.OnCloudLoadComplete += CloudOnceLoadComplete;

        // initialize cloudonce with cloudsave and cloudload
        Cloud.Initialize(true, true);
    }

    // called when cloudonce is initialized
    void CloudOnceOnInitializeComplete()
    {
        // load cloud storage
        Cloud.Storage.Load();
    }

    // on cloudonce load complete, update the menu ui with the player's rank
    void CloudOnceLoadComplete(bool success)
    {
        Menu.instance.UpdateUI(CloudVariables.RankScore.ToString());
    }

    // get rank of player
    public string GetRank()
    {
        return CloudVariables.RankScore.ToString();
    }

    // increase rank of player who didn't lose the game
    public void IncreaseRank()
    {
        CloudVariables.RankScore += 10;
        Save();
    }

    // decrease rank of player who lost the game
    public void DecreaseRank()
    {
        CloudVariables.RankScore -= 10;
        Save();
    }

    // save data to cloud storage
    private void Save()
    {
        Cloud.Storage.Save();
    }
}
