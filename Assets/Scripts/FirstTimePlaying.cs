using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstTimePlaying : MonoBehaviour
{
    public GameObject tutorial;
    public GameObject[] tutorialPages;
    private int pageTracker = 0;
    
    public GameObject backButton;
    public GameObject nextButton;
    public GameObject exitButton;
    
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetInt("Tutorial") != 1)
        {
            PlayerPrefs.SetInt("Tutorial", 1);
            tutorial.SetActive(true);
            // set the class character behind the view of the custom game view
            Menu.instance.characterChosen.transform.position = 
                new Vector3(
                    Menu.instance.characterChosen.transform.position.x,
                    Menu.instance.characterChosen.transform.position.y,
                    -10);
        }
    }

    public void BackButton()
    {
        tutorialPages[pageTracker].SetActive(false);
        pageTracker -= 1;
        tutorialPages[pageTracker].SetActive(true);

        if (pageTracker == 0)
        {
            backButton.GetComponent<Button>().interactable = false;
        } else if(pageTracker < 4)
            nextButton.GetComponent<Button>().interactable = true;
    }
    
    public void NextButton()
    {
        tutorialPages[pageTracker].SetActive(false);
        pageTracker += 1;
        tutorialPages[pageTracker].SetActive(true);
        
        if (pageTracker == 4)
        {
            nextButton.GetComponent<Button>().interactable = false;
            exitButton.SetActive(true);
        } else if(pageTracker > 0)
            backButton.GetComponent<Button>().interactable = true;
    }

    public void ExitButton()
    {
        tutorial.SetActive(false);
        
        // set the class character back to a visible position
        Menu.instance.characterChosen.transform.position = 
            new Vector3(Menu.instance.characterChosen.transform.position.x,
                Menu.instance.characterChosen.transform.position.y,
                0.5f);
    }
}
