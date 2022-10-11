using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mono.Data.Sqlite;
using System.Data;


/// <summary>
/// Class to handle the population of the mole list on the Diary scene. 
/// </summary>
public class DiaryListController : MonoBehaviour
{
    public GameObject moleButton;

    private List<(string id, string name, string date, string image_path)> moles;

    void Start()
    {
        GetMoles();
        PopulateList();
    }

    private void GetMoles() {
        moles = new();
        var connection = new SqliteConnection(DeviceVariables.database);

        connection.Open();
        var command = connection.CreateCommand();

        command.CommandText = "SELECT id, name, far_shot_path, far_shot_date FROM moles;";

        using (IDataReader reader = command.ExecuteReader()) {
            while(reader.Read()) {
                int id = reader.GetInt32(0);
                string name = reader.GetString(1);
                string image_path = reader.GetString(2);
                string date = reader.GetString(3);

                moles.Add((id.ToString(), name, date, image_path));
            }
            reader.Close();
        }
        connection.Close();

    }

    private void PopulateList() {

        moles.Sort((a, b) => a.date.CompareTo(b.date));

        foreach(var mole in moles) {
                GameObject button = Instantiate(moleButton) as GameObject;
                button.SetActive(true);

                button.GetComponent<MoleListButton>().SetId(mole.id);
                button.GetComponent<MoleListButton>().SetName(mole.name);
                button.GetComponent<MoleListButton>().SetDate(mole.date);
                button.GetComponent<MoleListButton>().SetImage(mole.image_path);

                button.transform.SetParent(moleButton.transform.parent, false);
        }
    }

}

