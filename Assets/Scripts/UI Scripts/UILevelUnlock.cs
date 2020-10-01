using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILevelUnlock : MonoBehaviour
{
    [SerializeField]
    private Button[] Camp1Buttons;
    [SerializeField]
    private Button[] Camp2Buttons;

    // Start is called before the first frame update
    void Start()
    {
        if(Camp1Buttons.Length != GameManager.compLevelCamp1.Length || Camp2Buttons.Length != GameManager.compLevelCamp2.Length)
        {
            Debug.Log("ERROR: non matching nr of buttons on display and in Gamemanager / Custominfo");
        }
        else if (GameManager.isLoggedIn)
        {
            DeactivateAllButtons();
            ActivateButtons();
        }
      
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DeactivateAllButtons()
    {
        foreach (var button in Camp1Buttons)
        {
            button.interactable = false;
        }
        foreach (var button in Camp2Buttons)
        {
            button.interactable = false;
        }
    }

    void ActivateButtons()
    {
        for (int i = 0; i < GameManager.compLevelCamp1.Length; i++)
        {
            Camp1Buttons[i].interactable = GameManager.compLevelCamp1[i];
        }

        for (int i = 0; i < GameManager.compLevelCamp2.Length; i++)
        {
            Camp2Buttons[i].interactable = GameManager.compLevelCamp2[i];
        }
    }
}
