using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

//******************* The manager for all the true false questions *****************//

public class DenkVragen : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField]
    private Text vraagText;
    [SerializeField]
    private Text uitlegText;

    [Header("Questions")]
    [SerializeField]
    [Tooltip("toggle this to remove a question from the list even when it is aswered incorrectly")]
    private bool removeIncorrectAnswers;
    [SerializeField]
    [Tooltip("the time the game waits until it loads the next question")]
    private float waitingTime = 1f;
    [SerializeField]
    private DenkVraag[] vragen;


    private static List<DenkVraag> testVragen;
    private GameManager gm;
    private DenkVraag denkVraag;
    

    private void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();


        if (testVragen == null || testVragen.Count == 0)
        {
            testVragen = vragen.ToList<DenkVraag>();
        }

        SetCurrentQuestion(true);

    }

    // sets 
    public void SetCurrentQuestion(bool correctlyAnswered)
    {
        if (!correctlyAnswered) // reads the incorrect question
        {
            testVragen.Add(denkVraag);
        }

        if(testVragen.Count == 0) testVragen = vragen.ToList<DenkVraag>();
        uitlegText.text = "";
        int randomIndex = Random.Range(0,testVragen.Count);
        denkVraag = testVragen[randomIndex];
        vraagText.text = denkVraag.vraagText;
        Debug.Log(denkVraag.vraagText + " is " + denkVraag.isWaar);
        testVragen.RemoveAt(randomIndex);
      
    }

    // set the question when the player answered correctly
    void SetNextCorrectQuestion()
    {
        SetCurrentQuestion(true);
    }

    // set the question when the player answered false
    void SetNextIncorrectQuestion()
    {
        SetCurrentQuestion(false);
    }


    // a button activates this method, the bool infers true or false option
    public void MultipleChoiceSelection(bool isTrue)
    {
        if(denkVraag.isWaar == isTrue)
        {
            uitlegText.text = "Correct!";
            Debug.Log(denkVraag.vraagText + " is " + denkVraag.isWaar + " -> Correct!");
            gm.IncreaseScore(1, 0);
            Invoke("SetNextCorrectQuestion", waitingTime);
        }
        else
        {
            Debug.Log(denkVraag.vraagText + " is " + denkVraag.isWaar + " -> Fout!");
            uitlegText.text = "Fout: " + denkVraag.uitleg;
            Invoke("SetNextIncorrectQuestion", waitingTime);
        }
    }
    
}
