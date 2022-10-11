using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Database script - runs with welcome screen (on app launch).
/// /// Launches the introduction tutorial if there is no existing user. 
/// </summary>
public class Database : MonoBehaviour
{
    bool newUser = false;

    void Start()
    {
        CreateDB();
        AccessDB();
        // sleeps for 1.5 seconds to show the SCaRF logo
        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1.5));
        if (newUser)
        {
            SceneManager.LoadScene("Welcome");
        } else 
        {
            SceneManager.LoadScene("Main Menu");
        }
        
    }

    public void CreateDB()
    {
        using (var connection = new SqliteConnection(DeviceVariables.database))
        {
            connection.Open();
            
            // creates account table
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE IF NOT EXISTS account (id INTEGER PRIMARY KEY AUTOINCREMENT, firstname VARCHAR(40), surname VARCHAR(40), age INTEGER, gender VARCHAR(40), model VARCHAR(40), email VARCHAR(40), password VARCHAR(40));";
                command.ExecuteNonQuery();
            }

            // creates mole table
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE IF NOT EXISTS moles (id INTEGER PRIMARY KEY AUTOINCREMENT, name VARCHAR(50), far_shot_path VARCHAR(40), coordinate_x REAL, coordinate_y REAL, coordinate_z REAL, marker_scale REAL, far_shot_date VARCHAR(40), reminder INTEGER);";
                command.ExecuteNonQuery();
            }

            // creates camera details table
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE IF NOT EXISTS cam_details (id INTEGER PRIMARY KEY AUTOINCREMENT, mole_id INTEGER, xPos REAL, yPos REAL, zPos REAL, xRot REAL, yRot REAL, zRot REAL, fov REAL, targetx REAL, targety REAL, targetz REAL);";
                command.ExecuteNonQuery();
            }


            // creates individual mole photos table
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE IF NOT EXISTS individual_mole_photos (id INTEGER PRIMARY KEY AUTOINCREMENT, mole_id INTEGER, near_shot_path VARCHAR(40), near_shot_date VARCHAR(40));";
                command.ExecuteNonQuery();
            }

            // creates questionnaire table
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE IF NOT EXISTS questionnaire (id INTEGER PRIMARY KEY AUTOINCREMENT, date VARCHAR(40));";
                command.ExecuteNonQuery();
            }

            // create question and answer table
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE IF NOT EXISTS questions (id INTEGER PRIMARY KEY AUTOINCREMENT, questionnaire_id INTEGER,question TEXT, answer VARCHAR(16));";
                command.ExecuteNonQuery();
            }

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE IF NOT EXISTS survey (id INTEGER PRIMARY KEY AUTOINCREMENT, question TEXT, answer VARCHAR(30), date VARCHAR(40));";
                command.ExecuteNonQuery();
            }

            connection.Close();
        } 
    }

    public void AccessDB()
    {
        using (var connection = new SqliteConnection(DeviceVariables.database))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM account;";
                using (IDataReader reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        newUser = true;
                    }
                    reader.Close();
                }
            }
            connection.Close();
            
        } 
    }

}
