using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;
using UnityEngine.UI;

/// <summary>
/// Handles the behaviour of the NavBar prefab.
/// </summary>
public class NavigationButtonHandler : MonoBehaviour
{
    public Button home, info, moles, diary;

    void Start()
    {
        home.onClick.AddListener(GoToHome);
        info.onClick.AddListener(GoToInfo);
        moles.onClick.AddListener(GoToBody);
        diary.onClick.AddListener(GoToDiary);
    }

    void GoToHome()
    {
#if !UNITY_EDITOR
        Analytics.CustomEvent("Used home button on Navbar");
#endif
        SceneManager.LoadScene("Main Menu");
    }


    void GoToInfo()
    {
#if !UNITY_EDITOR
        Analytics.CustomEvent("Used info button on Navbar");
#endif
        SceneManager.LoadScene("User Info");
    }

    void GoToBody()
    {
#if !UNITY_EDITOR
        Analytics.CustomEvent("Used moles button on Navbar");
#endif
        SceneManager.LoadScene("3DModelScene");
    }

    void GoToDiary()
    {
#if !UNITY_EDITOR
        Analytics.CustomEvent("Used diary button on Navbar");
#endif
        SceneManager.LoadScene("Diary");
    }
}
