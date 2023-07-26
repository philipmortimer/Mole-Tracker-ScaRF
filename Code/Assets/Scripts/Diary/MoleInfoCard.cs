using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;
using Mono.Data.Sqlite;
using System.Data;
using TMPro;
using System;

/// <summary>
/// Class to handle the population of the MoleInfoCard and it's associated behaviours.
/// </summary>
public class MoleInfoCard : MonoBehaviour
{


    public Button backButton, sendButton, newPhoto;
    public Button yesButton, noButton, close;
    public TextMeshProUGUI moleName;
    public TextMeshProUGUI date;
    public RawImage image;

    [SerializeField] GameObject buttonList;
    [SerializeField] GameObject questionsChoiceCard;
    [SerializeField] GameObject errorCard;

    private List<(string question, string answer)> questionnaire;
    private string farShotPath;
    //private bool buttonPressed;
    private bool includeQuestionnaire;

    void Start()
    {
        questionsChoiceCard.SetActive(false);
        errorCard.SetActive(false);
        backButton.onClick.AddListener(GoBack);
        newPhoto.onClick.AddListener(NewPhoto);
        sendButton.onClick.AddListener(SendChecks);
        yesButton.onClick.AddListener(delegate { OptionButtonPressed(true); } );
        noButton.onClick.AddListener(delegate { OptionButtonPressed(false); } );
        close.onClick.AddListener(() => errorCard.SetActive(false));


        includeQuestionnaire = false;
        //buttonPressed = false;
        LoadFarShotToMoleCard();
    }

    private void NewPhoto()
    {
        PhotoVariables.moleID = Convert.ToInt32(DBId.moleId);
        CameraScriptNew.previousScene = "Mole Information";
        SceneManager.LoadScene("Camera");
    }

    private void LoadFarShotToMoleCard()
    {
        var connection = new SqliteConnection(DeviceVariables.database);
        connection.Open();
        var command = connection.CreateCommand();

        command.CommandText = "SELECT name, far_shot_path, far_shot_date FROM moles WHERE id='" + DBId.moleId + "';";

        using (IDataReader reader = command.ExecuteReader() )
        {

            reader.Read();

            moleName.text = reader["name"].ToString();
            farShotPath = reader["far_shot_path"].ToString();
            date.text = reader["far_shot_date"].ToString();

            reader.Close();


            Texture2D imageTexture = new Texture2D(1, 1);

            byte[] bytes = File.ReadAllBytes(DeviceVariables.imagesPath + farShotPath);

            imageTexture.LoadImage(bytes);
            imageTexture.Apply();

            image.GetComponent<RawImage>().texture = imageTexture;
        }
        connection.Close();
    }

    private void SendChecks()
    {
        if (ButtonsSelectedCheck())
        {
            QuestionnaireOptions();
        }
    }

    private void GetQuestionnaire()
    {
        questionnaire = new();

        var connection = new SqliteConnection(DeviceVariables.database);
        connection.Open();
        var command = connection.CreateCommand();

        command.CommandText = @"SELECT question, answer 
                                FROM questions
                                WHERE questionnaire_id = (
                                    SELECT id FROM questionnaire
                                    WHERE date = (
                                        SELECT MAX(date) FROM questionnaire
                                    )
                                )"; 

        using (IDataReader reader = command.ExecuteReader() )
        {
            reader.Read();

            while(reader.Read())
            {
                string question = reader["question"].ToString();
                string answer = reader["answer"].ToString();

                questionnaire.Add((question, answer));
            }
        }
        connection.Close();
    }

    private void QuestionnaireOptions()
    {
        using (var connection = new SqliteConnection(DeviceVariables.database))
        {
            connection.Open();
            var command = connection.CreateCommand();

            command.CommandText = "SELECT COUNT(*) AS rowCount FROM questionnaire;";

            using (var reader = command.ExecuteReader())
            {
                reader.Read();

                if (Convert.ToInt32(reader["rowCount"]) > 0)
                {
                    questionsChoiceCard.SetActive(true);
                } else {
                    OptionButtonPressed(false);
                }
            }
        }
    }

    private void OptionButtonPressed(bool include)
    {
        if (include) includeQuestionnaire = true;
        
        questionsChoiceCard.SetActive(false);
        SendEmail();

    }

    private bool ButtonsSelectedCheck()
    {
        var lc = buttonList.GetComponent<MoleInformationListController>();
        if (lc.GetSelectedButtons().Count == 0)
        { 
            errorCard.SetActive(true);
            return false;
        }

        else
        {
            errorCard.SetActive(false);
            return true;
        }
    }

    private void SendEmail()
    {
#if !UNITY_EDITOR
        Analytics.CustomEvent("User tried to send email");
#endif
        // Sets the variables to empty first to ensure they don't stack up over multiple sends
        EmailVariables.questionnaire.Clear();
        EmailVariables.moleImagesToSend.Clear();
        if (includeQuestionnaire)
        {
            GetQuestionnaire();
            foreach (var item in questionnaire)
            {
                EmailVariables.questionnaire.Add((item.question, item.answer));
            }
        }

        var lc = buttonList.GetComponent<MoleInformationListController>();

        foreach (var btn in lc.GetSelectedButtons())
        {
            EmailVariables.moleImagesToSend.Add((btn.GetPath(), btn.GetDate()));
        }
        EmailVariables.moleImagesToSend.Reverse();

        EmailVariables.moleName = moleName.text;
        EmailVariables.farMolePhotoPath = farShotPath;
        setUserNameAndDob();

        SceneManager.LoadScene("Email");
    }

    void GoBack() {
        SceneManager.LoadScene("Diary");
    }

    /// <summary>
    /// Sets name field for email variables by retrieveing name from database
    /// </summary>
    void setUserNameAndDob()
    {
        EmailVariables.patientName = "";
        EmailVariables.patientDob = "";
        string firstName = "";
        string lastName = "";
        string age = "";
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
                        firstName += reader["firstname"];
                        lastName += reader["surname"];
                        age += reader["age"];
                        EmailVariables.gender = reader["gender"].ToString();
                    }
                    reader.Close();
                }
            }
            connection.Close();
        }
        EmailVariables.patientName = firstName + " " + lastName;
        EmailVariables.patientDob = age;
    }

}
