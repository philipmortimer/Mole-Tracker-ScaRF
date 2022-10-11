using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Set of classes to handle scene changes via button interact
/// on the info section of the app. Section could benefit from a re-design
/// to one scene, with one class to manage it. 
/// </summary>
public class ButtonHandling_BackButton : MonoBehaviour
{
    public Button btn_goBack;
    void Start()
    {
        btn_goBack.onClick.AddListener(GoToBtn_GoBack);
        
    }

    void GoToBtn_GoBack()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
