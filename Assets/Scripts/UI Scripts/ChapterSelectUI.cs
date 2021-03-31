using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChapterSelectUI : MonoBehaviour
{
    [Header("Prefabs")]
    [Tooltip("Make sure it has a sceneLoader script attached to it")]
    [SerializeField] private GameObject levelSelectButtonPrefab;

    [Header("SceneObjects")]
    [SerializeField] private Transform buttonParent;
    [SerializeField]
    private Text chapterNameText;
    [SerializeField]
    private Text chapterExplanationText;
    [SerializeField]
    private Image coverImage;
    [SerializeField]
    private Text chapterLevel, chapterScore;

    private ChapterScriptableObject chapterObject;
    private int chapterinfoNr;
    private bool hasData = false;

    //set up all the info and buttons for the chapter UI
    public void SetChapterUI(ChapterScriptableObject chapter)
    {
        hasData = true;
        chapterObject = chapter;

        if (chapterNameText) chapterNameText.text = chapter.chapterName;
        if (chapterExplanationText) chapterExplanationText.text = chapter.chapterExplanation;
        if (coverImage) coverImage.sprite = chapter.coverImage;

        GetChapterInfo(chapter);
        SetLevelAndScoreUI();

        for (int i = 0; i < chapter.levels.Count; i++)
        {
            bool active = false;
            if (!GameManager.isLoggedIn)
            {
                active = true;
            }
            else if(i <= GameManager.chaptersInfos[chapterinfoNr].scores.Count)
            {
                active = GameManager.chaptersInfos[chapterinfoNr].scores[i] > 0;
            }
            SetButton(chapter.levels[i], active, i+1);
        }
        

    }

    //get the corresponding playerinfo to check scores and unlocks
    void GetChapterInfo(ChapterScriptableObject chapter)
    {
        for (int i = 0; i < GameManager.chaptersInfos.Count; i++)
        {
            if (GameManager.chaptersInfos[i].UID == chapter.UID)
            {
                chapterinfoNr = i;
                return;
            }
        }
    }

    //set a button to a chapter scene including if it should be unlocked or not
    void SetButton(string scene, bool active, int nr)
    {
        GameObject button =  Instantiate(levelSelectButtonPrefab, buttonParent);
        button.GetComponent<SceneLoader>().SetButton(scene, active, nr);
    }

    // sets the text fields to the correct values from the gamemanager's static variables from the login
    void SetLevelAndScoreUI()
    {
        if (chapterLevel && chapterScore) //if added
        {
            chapterLevel.text = "Complete";
            for (int i = 0; i < GameManager.chaptersInfos[chapterinfoNr].scores.Count; i++)
            {
                if(GameManager.chaptersInfos[chapterinfoNr].scores[i] == 0)
                {
                    chapterLevel.text = i.ToString() + " / " + GameManager.chaptersInfos[chapterinfoNr].scores.Count;
                    break;
                }
            }

            int scoreTotal = 0;
            foreach (var score in GameManager.chaptersInfos[chapterinfoNr].scores)
            {
                scoreTotal += score;
            }
            chapterScore.text = scoreTotal.ToString();
        }

    }
    
    //loads a random scene that is unlocked, or if not logged in, just a random one
    public void LoadRandomScene()
    {
        if (!hasData) return;
        GameManager.campaignMode = false;
        List<string> availableScenes = new List<string>();
        availableScenes.Add(chapterObject.levels[0]);

        for (int i = 1; i < chapterObject.levels.Count; i++)
        {
            if (!GameManager.isLoggedIn || GameManager.chaptersInfos[chapterinfoNr].scores[i] != 0)
            {
                availableScenes.Add(chapterObject.levels[i]);
            }
        }

        SceneManager.LoadScene(availableScenes[Random.Range(0, availableScenes.Count)]);
        
    }

    //loads the first scene where the player has zero points
    public void LoadChapterScene()
    {
        if (!hasData) return;

        GameManager.campaignMode = true;
        for (int i = 0; i < GameManager.chaptersInfos[chapterinfoNr].scores.Count; i++)
        {
            if (GameManager.chaptersInfos[chapterinfoNr].scores[i] == 0)
            {

                SceneManager.LoadScene(chapterObject.levels[i]);
                return;
            }
        }
        SceneManager.LoadScene(chapterObject.levels[0]);
        return;
    }
    
}
