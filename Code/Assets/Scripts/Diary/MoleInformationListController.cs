using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mono.Data.Sqlite;
using System.Data;

/// <summary>
/// Class to handle the population of the list of near-shot images when in the MoleInfoCard scene. 
/// </summary>
public class MoleInformationListController : MonoBehaviour
{
    public GameObject moleButton;

    private List<MoleListButton> moleButtons;
    private List<(string path, string date)> nearShots;
    //private string dbName = "URI=file:Database.s3db";

    void Awake()
    {
        moleButtons = new List<MoleListButton>();
    }

    void Start()
    {
        GetNearShots();
        PopulateList(); 
    }

    public void ButtonPressed () {
        foreach (var b in moleButtons) {
            if( b.Selected() ) b.SetSelectedColor();
            else b.SetUnselectedColor();
        }
    }

    public List<MoleListButton> GetSelectedButtons() {
        return moleButtons.FindAll(x => x.Selected() ); 
    }

    private void GetNearShots() {
        nearShots = new();
        var connection = new SqliteConnection(DeviceVariables.database);

        connection.Open();
        var command = connection.CreateCommand();

        command.CommandText =   @"SELECT near_shot_path, near_shot_date
                                FROM individual_mole_photos
                                INNER JOIN moles
                                ON individual_mole_photos.mole_id = moles.id
                                WHERE moles.id='" + DBId.moleId  + "';";

        using(IDataReader reader = command.ExecuteReader()) {
            while(reader.Read()) {
                string path = reader.GetString(0);
                string date = reader.GetString(1);

                nearShots.Add((path, date));
            }
            reader.Close();
        }
        connection.Close();
    }

    private void PopulateList() {

        nearShots.Sort((a, b) => b.Item2.CompareTo(a.Item2));

        foreach(var nearShot in nearShots) {
            GameObject button = Instantiate(moleButton) as GameObject;
            moleButtons.Add(button.GetComponent<MoleListButton>() );
            button.SetActive(true);

            button.GetComponent<MoleListButton>().SetDate(nearShot.date);
            button.GetComponent<MoleListButton>().SetImage(nearShot.path);

            button.transform.SetParent(moleButton.transform.parent, false);
        }
    }

}
