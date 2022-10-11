using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Handles the behaviour of setting the icon to bold in navbar for the current scene. 
/// </summary>
public class NavbarBold : MonoBehaviour
{
    public int fontSize;
    public TMP_FontAsset boldFont;

    // Start is called before the first frame update
    void Start()
    {
        GameObject a = gameObject.transform.Find("Navigator Shadow").gameObject;
        GameObject b = a.transform.Find("Navigation Bar").gameObject;

        SetBold(WhichBold(), b);
    }

    void SetBold(string buttonName, GameObject navBar)
    {
        if (buttonName != "null")
        {
            TMP_Text t = navBar.transform.Find(buttonName).GetComponentInChildren<TMP_Text>();
            t.font = boldFont;
            t.fontStyle = FontStyles.Bold;
            t.fontSize = fontSize;
        }
    }

    private string WhichBold()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "Main Menu")
        {
            return "Home Button";
        }
        else if (sceneName == "Diary" || sceneName == "Mole Information")
        {
            return "Diary Button";
        }
        else if (sceneName == "User Info")
        {
            return "Info Button";
        }
        return "null";
    } 
}
