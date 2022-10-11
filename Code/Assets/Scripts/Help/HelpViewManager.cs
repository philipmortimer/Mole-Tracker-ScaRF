using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class to handle the behaviour of the help panel screens.
/// </summary>
public class HelpViewManager : MonoBehaviour
{
    public Button help, exit, next, previous;
    public GameObject helpPanel, screens;
    int screen = 0;
    int screenCheck = -1;
    int screenCount;

    void Start()
    {
        help.onClick.AddListener(Help);
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

    void Help()
    {
        helpPanel.SetActive(true);
    }

    void Exit()
    {
        helpPanel.SetActive(false);
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
