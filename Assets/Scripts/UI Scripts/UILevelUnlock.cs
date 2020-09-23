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
        if (GameManager.isLoggedIn)
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
        for (int i = 0; i < GameManager.levelCamp1; i++)
        {
            Camp1Buttons[i].interactable = true;
        }

        for (int i = 0; i < GameManager.levelCamp2; i++)
        {
            Camp2Buttons[i].interactable = true;
        }
    }
}
