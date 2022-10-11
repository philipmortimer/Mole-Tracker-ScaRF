using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// TODO: Confirm if this class is actually in use. I have a feeling it's old/redundant

public class ButtonHandler_Photo : MonoBehaviour
{
    public Button home;
    // Start is called before the first frame update
    void Start()
    {
        home.onClick.AddListener(GoToHome);
    }

    void GoToHome()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
