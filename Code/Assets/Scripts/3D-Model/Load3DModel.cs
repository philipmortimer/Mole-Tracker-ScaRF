using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;


/// <summary>
/// Class to handle loading the correct 3D model when scene launches. 
/// </summary>
public class Load3DModel : MonoBehaviour
{
    public GameObject male;
    public GameObject female;
    private string gender = "null";


    void Awake()
    {
        GetGender();
        if (gender == "male")
        {
            male.SetActive(true);
            female.SetActive(false);
        }
        else if (gender == "female")
        {
            male.SetActive(false);
            female.SetActive(true);
        }
        else
        {
            Debug.Log("Gender is set as: " + gender);
        }
    }

    /// <summary>
    /// Query database to return the model selected by the user. 
    /// </summary>
    private void GetGender()
    {
        using (var connection = new SqliteConnection(DeviceVariables.database))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT model FROM account;";
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        gender = (string)reader["model"];
                    }
                    reader.Close();
                }
            }
            connection.Close();
        }
    }

}
