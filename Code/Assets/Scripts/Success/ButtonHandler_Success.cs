using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Class to handle button behviour on the 'Success' scene, reached after the addition of a new mole. 
/// </summary>
public class ButtonHandler_Success : MonoBehaviour
{
    public Button survey, backToBody;
    
    void Start()
    {
        survey.onClick.AddListener(()=> SceneManager.LoadScene("SCQOLIT Survey"));
        backToBody.onClick.AddListener(()=> SceneManager.LoadScene("3DModelScene"));
    } 
}
