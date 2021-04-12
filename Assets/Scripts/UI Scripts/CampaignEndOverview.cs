using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Obsolete]
public class CampaignEndOverview : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField]
    private GameObject levelButton;

    [Header("Parameters")]
    [SerializeField]
    private Text endMessageText;
    [SerializeField]
    private CampaignType campaign;

    [SerializeField]
    private string completeMessage, incompleteMessage;

    enum CampaignType { Topografie1, Topografie2};

    private GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        //SetUI();
    }
    /*
    void SetUI()
    {
        bool complete = true;

        int nrOfLevels = 0;
        int levelToLoad = 1;
        int[] scores = new int[0];
        bool[] compLevels = new bool[0];

        switch (campaign)
        {
            case CampaignType.Topografie1:
                compLevels = GameManager.compLevelCamp1;
                nrOfLevels = compLevels.Length;
                scores = GameManager.scoreCamp1;
                levelToLoad = gm.firstCamp1Level;
                
                break;

            case CampaignType.Topografie2:
                compLevels = GameManager.compLevelCamp2;
                nrOfLevels = compLevels.Length;
                scores = GameManager.scoreCamp2;
                levelToLoad = gm.firstCamp2Level;

                break;

            default:
                compLevels = new bool[0];
                nrOfLevels = 0;
                scores = new int[0];
                break;

        }

        float percentPerButton = 1 /(nrOfLevels>0? (float)nrOfLevels : 1);

        for (int i = 0; i < nrOfLevels; i++)
        {
            if (!compLevels[i])
            {
                complete = false;
            }

            GameObject newButton = Instantiate(levelButton, transform);
            newButton.GetComponent<CampaignEndButton>().SetValues(
                i * percentPerButton,(i+1) * percentPerButton,
                i,
                scores[i],
                compLevels[i],
                levelToLoad + i,
                SceneManager.GetSceneByBuildIndex(levelToLoad + i).name
            );
        }

        endMessageText.text = complete ? completeMessage : incompleteMessage;
    }
    */
}
