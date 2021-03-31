using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField]
    private GameObject chapterUIPrefab;

    [SerializeField]
    private ChapterListScriptableObject chapterList;

    [Header("Scene Objects")]
    [SerializeField]
    private Transform chapterUIParent;

    // Start is called before the first frame update
    void Start()
    {
        SetChapterUI();
    }

    void SetChapterUI()
    {
        for (int i = 0; i < chapterList.chapters.Count; i++)
        {
            ChapterSelectUI chapterSelectUI = Instantiate(chapterUIPrefab, chapterUIParent).GetComponent<ChapterSelectUI>();
            chapterSelectUI.SetChapterUI(chapterList.chapters[i]);
        }
    }

}
