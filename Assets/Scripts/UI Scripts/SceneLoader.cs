using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SceneLoader : MonoBehaviour
{
    [SerializeField] private bool setUpAtStart = true;
    [SerializeField][Scene] private string sceneObject;
    [SerializeField]
    private TextLocaliser buttonText;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        if (setUpAtStart) SetButton(sceneObject, true, -1);
    }

    public void SetButton(string sceneString, bool active, int nr)
    {
        sceneObject = sceneString;

        if (buttonText && sceneObject != "")
        {
            buttonText.UpdateText(sceneObject, nr); //+ Regex.Replace(sceneObject, "(\\B[A-Z])", " $1"));
        }
        if(!button) button = GetComponent<Button>();
        button.interactable = active;
       
    }

    public void LoadScene()
    {
        GameManager.campaignMode = false;
        SceneManager.LoadScene(sceneObject);
    }

}
