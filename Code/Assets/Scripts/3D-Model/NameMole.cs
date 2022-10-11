using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// <para>
/// Class to handle naming and setting a number of days as a reminder for a new mole. 
/// </para>
/// <para>
/// Launches camera to take mole photos on completion. 
/// </para>
/// </summary>
public class NameMole : MonoBehaviour
{
    public GameObject ui, hb, sv, amb, nb, exit, error;
    public Button input;
    public TMPro.TMP_InputField mole_name, reminder;

    void Start()
    {
        input.onClick.AddListener(Name);
    }

    void Update() {
        if (PhotoVariables.nameMole)
        {   
            PhotoVariables.nameMole = false;
            ui.SetActive(true);
            hb.SetActive(false);
            sv.SetActive(false);
            amb.SetActive(false);
            nb.SetActive(false);
            exit.SetActive(false);
        }
    }

    void Name()
    {
       if (CheckComplete())
        {
            error.SetActive(false);
            ui.SetActive(false);
            PhotoVariables.date = DateTime.Today.ToString("dd-MM-yyyy");
            PhotoVariables.moleName = mole_name.text;
            PhotoVariables.reminder = Convert.ToInt32(reminder.text);
            PhotoVariables.openCamera = true;
            this.transform.GetComponent<MeshRenderer>().enabled = true;
        }
    }

    private bool CheckComplete()
    {
        if (mole_name.text == "")
        {
            error.SetActive(true);
            error.GetComponent<TMPro.TMP_Text>().text = "Error: Please enter a name for this mole.";
            return false;
        }
        else if (!int.TryParse(reminder.text, out int val))
        {
            error.SetActive(true);
            error.GetComponent<TMPro.TMP_Text>().text = "Error: Please enter a number, for example, 30.";
            return false;
        }
        return true;
        
    }
}
