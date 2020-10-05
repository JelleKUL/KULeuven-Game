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
    [SerializeField]
    private Button[] randomButtons;

    private GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        if(Camp1Buttons.Length != GameManager.compLevelCamp1.Length || Camp2Buttons.Length != GameManager.compLevelCamp2.Length)
        {
            Debug.Log("ERROR: non matching nr of buttons on display and in Gamemanager / Custominfo");
        }
        else if (GameManager.isLoggedIn)
        {
            DeactivateAllButtons();
            ActivateButtons();
            ActivateRandomButton();
        }
      
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ActivateRandomButton()
    {

        randomButtons[0].interactable = GameManager.levelCamp1 > 1; //gm.CountBoolTrue(GameManager.compLevelCamp1) > 0;

        int nrOfScenes = 1;
        for (int i = 1; i < randomButtons.Length; i++)
        {
            randomButtons[i].interactable = (GameManager.levelCamp2 > nrOfScenes);

            nrOfScenes += gm.NrOfCamp2ChapterScenes[i - 1];
        }
     
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
