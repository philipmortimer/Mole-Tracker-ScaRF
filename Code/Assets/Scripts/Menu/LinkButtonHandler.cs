using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class to handle launching urls when a user clicks on one of the external
/// links on the home page. 
/// </summary>
public class LinkButtonHandler : MonoBehaviour
{
    public Button about, donate;

    void Start()
    {
        about.onClick.AddListener(OpenURLAbout);
        donate.onClick.AddListener(OpenURLDonate);
    }

    public void OpenURLAbout()
    {
        Application.OpenURL("https://www.skincancerresearch.org/about-scarf");
    }

    public void OpenURLDonate()
    {
        Application.OpenURL("https://www.justgiving.com/scrf");
    }
}
