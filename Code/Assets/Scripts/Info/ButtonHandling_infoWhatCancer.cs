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
public class ButtonHandling_infoWhatCancer : MonoBehaviour
{
    public Button commonCancer, cancerTriggered, cancerLooksLike, treatments, btn_goBack; 

    void Start()
    {
        commonCancer.onClick.AddListener(GoToCommonCancer); 
        cancerTriggered.onClick.AddListener(GoToCancerTriggered);
        cancerLooksLike.onClick.AddListener(GoToCancerLooksLike);
        treatments.onClick.AddListener(GoToTreatments);

        btn_goBack.onClick.AddListener(GoToBtn_GoBack); 
    }

    void GoToCommonCancer()
    {
        SceneManager.LoadScene("1_CommonCancer");
    }

    void GoToCancerTriggered()
    {
        SceneManager.LoadScene("2_CancerTriggered");
    }

    void GoToCancerLooksLike()
    {
        SceneManager.LoadScene("3_CancerLooksLike");
    }

    void GoToTreatments()
    {
        SceneManager.LoadScene("4_Treatments");
    }

    void GoToBtn_GoBack()
    {
        SceneManager.LoadScene("User Info");
    }
}
