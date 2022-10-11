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
public class ButtonHandling_infoCommonCancer : MonoBehaviour
{

    public Button basalCell, squamousCell, malignant, back;
    
    void Start()
    {
        basalCell.onClick.AddListener(GoToBasalCell);
        squamousCell.onClick.AddListener(GoToSquamousCell);
        malignant.onClick.AddListener(GoToMalignant);
        back.onClick.AddListener(() => SceneManager.LoadScene("1_what cancer"));
    }

    void GoToBasalCell()
    {
        SceneManager.LoadScene("1_Basal");
    }

     void GoToSquamousCell()
    {
        SceneManager.LoadScene("2_Squamous");
    }

     void GoToMalignant()
    {
        SceneManager.LoadScene("3_Malignant");
    }
}
