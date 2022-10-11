using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;
using UnityEngine.UI;

/// <summary>
/// Handles the behaviour of the NavIcon prefab.
/// </summary>
public class NavIconButtonHandler : MonoBehaviour
{
    public Button userInfo, home;
    void Start()
    {
        userInfo.onClick.AddListener(GoToAccount);
        home.onClick.AddListener(GoToHome);
    }

    void GoToAccount()
    {
#if !UNITY_EDITOR
        Analytics.CustomEvent("Used account icon in NavIcons");
#endif
        SceneManager.LoadScene("Account");
    }

    void GoToHome()
    {
#if !UNITY_EDITOR
        Analytics.CustomEvent("Used home icon in NavIcons");
#endif
        SceneManager.LoadScene("Main Menu");
    }
}
