using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Option_Buttons : MonoBehaviour
{
    public Button newMole, exisitngMole;

    // Start is called before the first frame update
    void Start()
    {
        newMole.onClick.AddListener(GoToCamera);
        exisitngMole.onClick.AddListener(GoToMoleList);
    }

    void GoToCamera()
    {
        SceneManager.LoadScene("Camera");
    }

    void GoToMoleList()
    {
        Debug.Log("Existing Mole Clicked");
    }
}
