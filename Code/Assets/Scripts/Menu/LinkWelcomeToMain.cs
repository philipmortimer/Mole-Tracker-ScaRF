using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class LinkWelcomeToMain : MonoBehaviour
{
    public Button getStartedBtn;

    void Start()
    {
        getStartedBtn.onClick.AddListener(GetStarted);
    }

    public void GetStarted() 
    {
        SceneManager.LoadScene("WelcomeHelp");
    }
}
