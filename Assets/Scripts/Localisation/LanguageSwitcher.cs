using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LanguageSwitcher : MonoBehaviour
{
    [SerializeField]
    private Image languageImage;
    [SerializeField]
    private Text text;

    [SerializeField]
    private Image[] countryImages;

    // Start is called before the first frame update
    void Start()
    {
        if (text) text.text = LocalisationManager.GetLanguage();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeLanguage()
    {
        LocalisationManager.ChangeLanguage();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
