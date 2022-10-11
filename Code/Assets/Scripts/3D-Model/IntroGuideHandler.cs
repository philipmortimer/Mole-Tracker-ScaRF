using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Handles the display of the 3D model scene guide. Key saved to player prefs
/// to ensure this is only shown on first interaction. 
/// </summary>
public class IntroGuideHandler : MonoBehaviour
{
    public Button exit, next, previous;
    public GameObject introPanel, screens;
    public GameObject scrollView, addMoleButton;
    int screen = 0;
    int screenCheck = -1;
    int screenCount;


    void Start()
    {
        if (!PlayerPrefs.HasKey("seen3dhelpscreen")) 
        {
            introPanel.SetActive(true);
            scrollView.SetActive(false);
            addMoleButton.SetActive(false);
            PlayerPrefs.SetInt("seen3dhelpscreen", 1);
        }
        exit.onClick.AddListener(Exit);
        next.onClick.AddListener(Next);
        previous.onClick.AddListener(Previous);

        screenCount = screens.transform.childCount;
    }

    void Update()
    {
        if (screen > screenCount - 1)
        {
            screen = 0;
        }
        else if (screen < 0)
        {
            screen = screenCount - 1;
        }

        if (screen != screenCheck)
        {
            for (int i = 0; i < screenCount; i++)
            {
                screens.transform.GetChild(i).gameObject.SetActive(false);
            }
            screens.transform.GetChild(screen).gameObject.SetActive(true);
            screenCheck = screen;
        }
    }

    void Exit()
    {
        introPanel.SetActive(false);
        scrollView.SetActive(true);
        addMoleButton.SetActive(true);
    }

    void Next()
    {
        screen += 1;
    }

    void Previous()
    {
        screen = screen - 1;
    }
}
