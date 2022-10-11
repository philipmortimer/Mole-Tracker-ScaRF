using System.Collections;
using System.Collections.Generic;
using System.Data;
using Mono.Data.Sqlite;
using System;
using UnityEngine;
using UnityEngine.UI;

// This class is most likely redundant but I'm not 100% sure.
public class LoadMoles : MonoBehaviour
{
    public GameObject canvas;
    public GameObject myPrefab;
    public Image oldImage;
    public TMPro.TMP_Text mole_name;
    public TMPro.TMP_Text date_taken;
    //private string dbName = "URI=file:Database.s3db";

    void Start() 
    {
        Load();

    }

    void Load()
    {
        using (var connection = new SqliteConnection(DeviceVariables.database))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM moles;";
                using (IDataReader reader = command.ExecuteReader())
                {
                    int currentY = 800;
                    while (reader.Read())
                    {   // spawn in mole prefab and make it child of canvas
                        GameObject m = Instantiate(myPrefab, new Vector3(0, currentY, 0), Quaternion.identity);
                        m.transform.SetParent(canvas.transform, false);


                        GameObject shadow = m.transform.Find("Shadow").gameObject;
                        GameObject infoBar = shadow.transform.Find("InfoBar").gameObject;
                        GameObject moleImage = shadow.transform.Find("MoleImage").gameObject;
                        GameObject btn = infoBar.transform.Find("btn").gameObject;
                        GameObject moleName = btn.transform.Find("MoleName").gameObject;
                        GameObject dateTaken = btn.transform.Find("DateTaken").gameObject;

                        string image_path = "";
                        image_path += reader["far_shot_path"];
                        moleImage.GetComponent<Image>().sprite = Resources.Load<Sprite>(image_path);
                        moleName.GetComponent<TMPro.TMP_Text>().text += reader["name"];
                        dateTaken.GetComponent<TMPro.TMP_Text>().text += reader["far_shot_date"];

                        m.SetActive(true);
                        currentY -= 200;
                    }
                    reader.Close();
                }
            }
            connection.Close();
        } 
    }
}
