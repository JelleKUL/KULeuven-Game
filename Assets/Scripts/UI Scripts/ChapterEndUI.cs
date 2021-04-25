using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChapterEndUI : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject endLevelElement;

    [Header("SceneElements")]
    [SerializeField] private Transform levelListParent;
    [SerializeField] private Text titleText;
    [SerializeField] private TextLocaliser explainText;

    private GameManager gm;
    

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        SetUI();
    }

    void SetUI()
    {
        if (!gm) return;
        if (GameManager.currentChapter < 0) return;

        titleText.text = LocalisationManager.GetLocalisedValue("ID_results") + " " + LocalisationManager.GetLocalisedValue(gm.chapterList.chapters[GameManager.currentChapter].chapterName) + ":";

        bool allcomplete = true;
        for (int i = 0; i < gm.chapterList.chapters[GameManager.currentChapter].levels.Count; i++)
        {
            ChapterEndElement endElement = Instantiate(endLevelElement, levelListParent).GetComponent<ChapterEndElement>();

            int score = GameManager.chaptersInfos[GameManager.currentChapter].scores[i];
            if (score == 0) allcomplete = false;
            endElement.SetUI(score, score > 0, gm.chapterList.chapters[GameManager.currentChapter].levels[i]);
        }

        if (allcomplete)
        {
            explainText.UpdateText("ID_all_levels_complete");
        }
        else
        {
            explainText.UpdateText("ID_not_all_levels_complete");
        }
        
    }
}
