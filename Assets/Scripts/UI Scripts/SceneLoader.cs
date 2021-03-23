using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The reference to the SceneObject, make sure it is in the BuildSettings")]
    [Scene]
    private string sceneObject;
    [SerializeField]
    private Text buttonText;

    private void Awake()
    {
        if(buttonText && sceneObject !="")
        {
            buttonText.text = Regex.Replace(sceneObject, "(\\B[A-Z])", " $1");
        }
    }


    public void LoadScene()
    {
        SceneManager.LoadScene(sceneObject);
    }
}
