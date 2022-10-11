using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;
using UnityEngine.UI;
using System;


/// <summary>
/// Class to handle the creation of a new user and the New User scene.
/// </summary>
public class NewUser : MonoBehaviour
{

    public Button save, maleModelButton, femaleModelButton;
    
    [SerializeField] private TMPro.TMP_InputField firstName, lastName, age;
    [SerializeField] private TMPro.TMP_Dropdown gender;
    [SerializeField] private TMPro.TMP_Text errorDescription;
    [SerializeField] private GameObject maleBase, femaleBase, maleSelected, femaleSelected;
    [SerializeField] private GameObject errorMessage;

    public String modelChoice = "null";

    void Start()
    {
        save.onClick.AddListener(SaveUser);
        maleModelButton.onClick.AddListener(SelectedMale);
        femaleModelButton.onClick.AddListener(SelectedFemale);
    }


    private void SelectedFemale()
    {
        modelChoice = "female";
        maleBase.SetActive(true);
        maleSelected.SetActive(false);
        femaleBase.SetActive(false);
        femaleSelected.SetActive(true);
    }

    private void SelectedMale()
    {
        modelChoice = "male";
        maleBase.SetActive(false);
        maleSelected.SetActive(true);
        femaleBase.SetActive(true);
        femaleSelected.SetActive(false);
    }

    /// <summary>
    /// Function to insert a new user into the database.
    /// </summary>
    void SaveUser()
    {
         if (ConfirmFieldsCompleted())
         {
             using (var connection = new SqliteConnection(DeviceVariables.database))
             {
                 connection.Open();
    
                 using (var command = connection.CreateCommand())
                 {
                     command.CommandText = "INSERT INTO account (firstname, surname, age, gender, model) VALUES ('" +
                         firstName.text + "', '" + lastName.text + "', '" + age.text + "', '" + gender.options[gender.value].text + "', '" +
                         modelChoice + "');";
                     command.ExecuteNonQuery();
                 }
                 connection.Close();
             }
             SceneManager.LoadScene("Questions Scene");
         } 
    }

    /// <summary>
    /// Checks that the fields have all been completed with the correct types of information. 
    /// </summary>
    /// <returns></returns>
    private bool ConfirmFieldsCompleted()
    {
        if ((firstName.text == "") || (lastName.text == ""))
        {
            errorMessage.SetActive(true);
            errorDescription.text = "Please enter your name.";
            return false;
        }
        else if (!int.TryParse(age.text, out int val))
        {
            errorMessage.SetActive(true);
            errorDescription.text = "Age must be a number. e.g. 37";
            return false;
        }
        else if (gender.options[gender.value].text == "Select Gender")
        {
            errorMessage.SetActive(true);
            errorDescription.text = "Please select your preferred gender identity from the dropdown.";
            return false;
        }

        else if (modelChoice != "male" && modelChoice != "female") 
        {
            errorMessage.SetActive(true);
            errorDescription.text = "Please select a model which best represents your body.";
            return false;
        }

        errorMessage.SetActive(false);
        return true;
    }
}
