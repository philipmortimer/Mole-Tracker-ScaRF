using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Data;
using System;
using Mono.Data.Sqlite;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Class to handle the user account details and Account scene.
/// </summary>
public class Account : MonoBehaviour
{

    public Button save, delete, maleModelButton, femaleModelButton, survey;
    
    [SerializeField] private TMPro.TMP_InputField firstName, lastName, age;
    [SerializeField] private TMPro.TMP_Dropdown gender;
    [SerializeField] private GameObject maleBase, femaleBase, maleSelected, femaleSelected;
    [SerializeField] private GameObject errorMessage;
    [SerializeField] private TMPro.TMP_Text errorDescription;

    public int id;
    public String modelChoice = "null";


    void Start()
    {
        GetUserDetails();
        save.onClick.AddListener(UpdateUser);
        delete.onClick.AddListener(DropTable);
        maleModelButton.onClick.AddListener(SelectedMale);
        femaleModelButton.onClick.AddListener(SelectedFemale);
        survey.onClick.AddListener(() => SceneManager.LoadScene("SCQOLIT Survey"));
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
    /// Function to query database for user information. 
    /// </summary>
    public void GetUserDetails()
    {
        using (var connection = new SqliteConnection(DeviceVariables.database))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM account;";
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {   
                        firstName.text += reader["firstname"];
                        lastName.text += reader["surname"];
                        age.text += reader["age"];
                        string label = reader["gender"].ToString();
                        gender.value = gender.options.FindIndex(option => option.text == label);
                        if (reader["model"].ToString() == "male") SelectedMale();
                        else SelectedFemale();
                    }
                    reader.Close();
                }
            }
            connection.Close();
        } 
    }

    /// <summary>
    /// Function to update user details in the database when save button is pressed. 
    /// </summary>
   void UpdateUser()
   {
       if (ConfirmFieldsCompleted()) 
       {
            using (var connection = new SqliteConnection(DeviceVariables.database))
            {
                connection.Open();
                DateTime dt = DateTime.Now;
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "UPDATE account SET firstname = '" + firstName.text + "', surname = '" + lastName.text + "', age = '" + age.text + "', gender = '" + 

                    gender.options[gender.value].text + "', model = '" + modelChoice + "' WHERE i = 1;";

                    command.ExecuteNonQuery();
                }
                connection.Close();
            } 
            SceneManager.LoadScene("Main Menu");
       }
   }

    /// <summary>
    /// Checks that the fields have all been completed and with the correct types of information. 
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
            errorDescription.text = "Age must be an integer value.";
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

    void DropTable()
   {
       using (var connection = new SqliteConnection(DeviceVariables.database))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "DROP TABLE database;";
                command.ExecuteNonQuery();
            }
            connection.Close();
        } 
        SceneManager.LoadScene("Start Screen");
   }
}


