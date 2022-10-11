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
public class ButtonHandling_UserInfo : MonoBehaviour
{
    public Button whatCancer, howMoles, whatScarf, howDonate, whoCancer, backBtn;

    void Start()
    {
        whatCancer.onClick.AddListener(GoToWISC);
        howMoles.onClick.AddListener(GoToHDIMM);
        whatScarf.onClick.AddListener(GoToWITCS);
        howDonate.onClick.AddListener(GoToHDID);
        whoCancer.onClick.AddListener(GoToWTTTASC);

        backBtn.onClick.AddListener(GoBack);
    }


    void GoToWISC()
    {

        SceneManager.LoadScene("1_what cancer"); 
    } 

    void GoToHDIMM()
    {
         SceneManager.LoadScene("2_how moles");
    }

    void GoToWITCS()
    {
         SceneManager.LoadScene("3_what scarf");
    }

    void GoToHDID()
    {
         SceneManager.LoadScene("4_how donate");
    }

    void GoToWTTTASC()
    {
         SceneManager.LoadScene("5_who cancer");
    }

    void GoBack()
    {
         SceneManager.LoadScene("Main Menu");
    }
}
