using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using AS;

public class LeaderBoard : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField]
    private GameObject leaderBoardPart;
    [SerializeField]
    private GameObject leaderBoardDots;
    [SerializeField]
    private GameObject loadingText;


    [Header ("parameters")]
    [SerializeField]
    [Range(3,10)]
    int maxNrOfEntries = 10;

    [SerializeField]
    Color firstColor, secondColor, thirdColor, defaultColor;

    [SerializeField]
    Font defaultFont, playerFont;


    public static string[,] LeaderBoardValues = new string[1,1]; // redunant

    public static string[] names;
    public static int[] scores;

    private float containerHeight;
    private float basicElementHeight;

    private bool dataAvailable;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // starts the score download procedure
    public void LeaderboardsDownloader()
    {
        Debug.Log("LeaderBoard: Attempting download");
        if (!dataAvailable)
        {
            LeaderBoardValues.TryToDownloadLeaderBoard(LeaderboardDownloaded); //sets the leaderboardvalues array
        }

    }

    // this method is called when the data is succesfully downloaded
    void LeaderboardDownloaded(string message)
    {
        Debug.Log("LeaderBoard: " + message);
        dataAvailable = true;

        //LeaderBoardValues = Sort_String(LeaderBoardValues, new int[2, 2] { { 1, 1 }, { 0, 1 } });
        Array.Sort(scores, names);
        Array.Reverse(scores);
        Array.Reverse(names);

        for (int i = 0; i < names.GetLength(0); i++)
        {
            Debug.Log(names[i] + " : " + scores[i]);
        }

        SetElements();
    }

    // lays out the values on the screen
    void SetElements()
    {
        loadingText.SetActive(false);

        containerHeight = GetComponent<RectTransform>().rect.height;
        Debug.Log(containerHeight);
        int nrOfElelementsToPlace = Mathf.Min(maxNrOfEntries, names.Length);

        List<string> namesList = new List<string>(names); //convert to list for easy finding


        if (GameManager.isLoggedIn)
        {
            if (namesList.Contains(GameManager.userName))
            {
                int playerRank = namesList.IndexOf(GameManager.userName);

                if (playerRank >= nrOfElelementsToPlace)
                {
                    basicElementHeight = containerHeight / (float)(2 + nrOfElelementsToPlace + 1);
                    Debug.Log(basicElementHeight);

                    
                    SpawnPlayerDisplay(playerRank, SpawnParts(nrOfElelementsToPlace));
                }
                else
                {
                    basicElementHeight = containerHeight / (float)(2 + nrOfElelementsToPlace);
                    SpawnParts(nrOfElelementsToPlace);
                }
            }
        }

        else
        {
            basicElementHeight = containerHeight / (float)(2 + nrOfElelementsToPlace);
            SpawnParts(nrOfElelementsToPlace);
        }

    }

    float SpawnParts(int amount)
    {
        float offset = 0f;

        for (int i = 0; i < amount; i++)
        {
           

            LeaderBoardElement newElement = Instantiate(leaderBoardPart, transform).GetComponent<LeaderBoardElement>();
            newElement.SetValues(i + 1, names[i], scores[i]);

            switch (i)
            {
                case 0:
                    newElement.SetLayout(firstColor, defaultFont, basicElementHeight * 2, offset);
                    offset += basicElementHeight * 2;
                    break;

                case 1:
                    newElement.SetLayout(secondColor, defaultFont, basicElementHeight * 1.5f, offset);
                    offset += basicElementHeight * 1.5f;
                    break;

                case 2:
                    newElement.SetLayout(thirdColor, defaultFont, basicElementHeight * 1.25f, offset);
                    offset += basicElementHeight * 1.25f;
                    break;

                default:
                    newElement.SetLayout(defaultColor, defaultFont, basicElementHeight, offset);
                    offset += basicElementHeight;
                    break;
            }
        }
        return offset;
    }

    void SpawnPlayerDisplay(int playerPos, float offset)
    {
        LeaderBoardElement newDotElement = Instantiate(leaderBoardDots, transform).GetComponent<LeaderBoardElement>();
        newDotElement.SetLayout(defaultColor, defaultFont, basicElementHeight * 0.25f, offset);

        offset += basicElementHeight * 0.25f;

        LeaderBoardElement newElement = Instantiate(leaderBoardPart, transform).GetComponent<LeaderBoardElement>();
        newElement.SetValues(playerPos+1, names[playerPos], scores[playerPos]);
        newElement.SetLayout(defaultColor, defaultFont, basicElementHeight, offset);
    }

}
