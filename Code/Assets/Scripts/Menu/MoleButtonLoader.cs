using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.IO;
using System.Globalization;


/// <summary>
/// Class to handle loading (up to four) of the mole in the DB which require
/// new photos soonest.  
/// </summary>
public class MoleButtonLoader : MonoBehaviour
{
    public GameObject addNewMole, seeMore, reminderText;
    public List<GameObject> mole_buttons;
    [SerializeField] private Button addNewMoleButton;

    private List<(int id, string name, string image_path, int reminder)> tempMoles;
    private List<(int id, string name, string date, string image_path, int remaining)> moles;
    private (string name, int days) nextMole;

    void Start()
    {
        addNewMoleButton.onClick.AddListener(() => SceneManager.LoadScene("3DModelScene"));
        seeMore.GetComponent<Button>().onClick.AddListener(() => SceneManager.LoadScene("Diary"));

        nextMole.days = 100;
        GetMoles();
        DeactivateAllButtons();
        UpdateButtons();
        UpdateNextMoleReminder();
    }

    /// <summary>
    /// Loads all moles currently in DB, calculates the num days left
    /// between the most recent image of each mole and the associated reminder
    /// days and stores all relevant info in <see cref="moles"/>, which is
    /// sorted by least days remaining. 
    /// </summary>
    private void GetMoles()
    {
        tempMoles = new();
        moles = new();
        using (var connection = new SqliteConnection(DeviceVariables.database))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM moles;";
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id =  Convert.ToInt32(reader["id"]);
                        string name = reader["name"].ToString();
                        string im_path = reader["far_shot_path"].ToString();
                        int reminder = Convert.ToInt32(reader["reminder"]);
                        tempMoles.Add((id, name, im_path, reminder));
                    }
                    reader.Close();
                }
            }


            foreach (var mole in tempMoles)
            {
                List<DateTime> nearShotDates = new();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"SELECT near_shot_date
                                FROM individual_mole_photos
                                INNER JOIN moles
                                ON individual_mole_photos.mole_id = moles.id
                                WHERE moles.id='" + mole.id + "';";
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string date = reader["near_shot_date"].ToString();
                            System.DateTime.TryParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateFormat);
                            nearShotDates.Add(dateFormat);
                        }
                    }
                }
                nearShotDates.Sort((a, b) => b.CompareTo(a));

                float date_diff = Mathf.Floor((float)(System.DateTime.Now - nearShotDates[0]).TotalDays);
                int remaining = mole.reminder - (int)date_diff;
                moles.Add((mole.id, mole.name, nearShotDates[0].ToString("dd-MM-yyyy"), mole.image_path, remaining));
            }
            connection.Close();
        }

        moles.Sort((a, b) => a.remaining.CompareTo(b.remaining));
    }

    /// <summary>
    /// Loads up to four mole buttons onto the home screen, with associated farshot image as thumbnail.
    /// Additionaly generates an onClick function to open the correct mole in the diary.
    /// </summary>
    private void UpdateButtons()
    {
        int numButtons = moles.Count >= 4 ? 4 : moles.Count;
        for (int i = 0; i < numButtons; i++)
        {
            Transform mb_button = mole_buttons[i].transform.Find("Button Object");
            Transform mole_text = mb_button.transform.Find("Mole Name");
            Transform days_text = mb_button.transform.Find("Days Remaining");
            Transform image = mb_button.transform.Find("Mole Image");

            mole_buttons[i].SetActive(true);

            mole_buttons[i].GetComponent<MoleInfo>().moleId = moles[i].id;
            mb_button.GetComponent<Button>().onClick.AddListener(() => OpenInDiary(mb_button.gameObject));

            TMPro.TextMeshProUGUI mole_text_string = mole_text.GetComponent<TMPro.TextMeshProUGUI>();
            mole_text_string.text = moles[i].name;

            TMPro.TextMeshProUGUI days_text_string = days_text.GetComponent<TMPro.TextMeshProUGUI>();
            days_text_string.richText = true;

            GetFarShotImage(i, image);

            if (moles[i].remaining < 0)
            {
                days_text_string.text = "OVERDUE by " + (moles[i].remaining * -1).ToString() + " days";
            }
            else
            {
                days_text_string.text = moles[i].remaining.ToString() + " days remaining";
            }

            if (moles[i].remaining < nextMole.days)
            {
                nextMole.name = moles[i].name;
                nextMole.days = moles[i].remaining;
            }
        }

        if (numButtons < 4)
        {
            addNewMole.SetActive(true);
            addNewMole.transform.position = mole_buttons[moles.Count].transform.position;
        }
        else if (numButtons >= 4)
        {
            seeMore.SetActive(true);
        }
    }

    private void DeactivateAllButtons()
    {
        foreach (var mb in mole_buttons)
        {
            mb.SetActive(false);
        }
        seeMore.SetActive(false);
    }

    /// <summary>
    /// Function called by onClick event to open the selected mole in the diary. 
    /// </summary>
    /// <param name="button"> The button which has been clicked. </param>
    private void OpenInDiary(GameObject button)
    {
        string moleName = button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text;
        var mole = moles.Find(m => m.name.Equals(moleName));
        DBId.moleId = mole.id.ToString();
        SceneManager.LoadScene("Mole Information");
    }

    /// <summary>
    /// Function to retrieve the correct far shot image from the DB.
    /// </summary>
    /// <param name="i"> The index of the mole in <see cref="moles"/></param> 
    /// <param name="image"> The transform of the RawImage to apply the retrieved texture to.</param>
    private void GetFarShotImage(int i, Transform image)
    {
        RawImage rawIm = image.GetComponent<RawImage>();
        Texture2D imageTexture = new(1, 1);
        byte[] bytes = File.ReadAllBytes(DeviceVariables.imagesPath + moles[i].image_path);
        imageTexture.LoadImage(bytes);
        imageTexture.Apply();
        rawIm.texture = imageTexture;
    }

    /// <summary>
    /// Updates the reminder text at the top of the home screen to reflect the details of the most urgent mole.
    /// </summary>
    private void UpdateNextMoleReminder()
    {
        if (moles.Count == 0)
        {
            reminderText.GetComponent<TMPro.TextMeshProUGUI>().text = "<size=24>You have not added any moles yet. "
                + " You can add a new mole by clicking 'Add New Mole' below.</size>";

        }

        else if (nextMole.days >= 0)
        {
            reminderText.GetComponent<TMPro.TextMeshProUGUI>().text = "You have <b><size=28>"
                + nextMole.days + "</size></b> days remaining to take a new photo of <b>"
                + nextMole.name + "</b>";

        } else
        {
            reminderText.GetComponent<TMPro.TextMeshProUGUI>().text = "You are OVERDUE by <b><size=28>"
                + nextMole.days * -1 + "</size></b> days to take a new photo of <b>"
                + nextMole.name + "</b>";
        }
    }
}
