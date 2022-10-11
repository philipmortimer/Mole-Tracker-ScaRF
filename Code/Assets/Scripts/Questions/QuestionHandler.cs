using System;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Mono.Data.Sqlite;

/// <summary>
/// Handles the behaviour of the new user questionnaire section of the app. 
/// </summary>
public class QuestionHandler : MonoBehaviour
{   
    public TextMeshProUGUI questionNo;
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI nextText;
    public Button yesBtn, noBtn, unsureBtn, rnsBtn, nextQBtn;

    private int currentQNo;
    private List<string> questions;
    private List<string> givenAnswers;

    [SerializeField] private GameObject nextQuestion;
    
    private Boolean yesSellected, noSellected, unsureSellected, rnsSellected;

    void Start() {
        nextQBtn.onClick.AddListener(NextQPressed);
        yesBtn.onClick.AddListener(YesBtnPressed);
        noBtn.onClick.AddListener(NoBtnPressed);
        unsureBtn.onClick.AddListener(UnsureBtnPressed);
        rnsBtn.onClick.AddListener(RnsBtnPressed);

        nextQuestion.SetActive(false);
    }

    private void OnEnable()
    {
        Initialise();
    }

    /// <summary>
    /// Hard coded questions, could be re-written to be more flexible down the line. 
    /// </summary>
    private void Initialise() {
        questions = new List<string>{
            "Have you ever had skin cancer?",
            "Has anyone in your family had a skin cancer?",
            "Have you ever had sunburn?",
            "Have you ever used a sunbed?",
            "Have you ever had a job that involved working outside?",
            "Are you immunosuppressed? (Suppression of the immune response, as by drugs or radiation)",
            "Have you got a large number of moles on your skin surface?",
            "Have you ever been exposed to any chemicals during your occupation?",
            "Have you ever been exposed to any radiation during your occupation?"
        };

        givenAnswers = new List<string>();
        yesSellected = noSellected = unsureSellected = rnsSellected = false;

        currentQNo = 1;
        SetQuestionNo();
        SetQuestionText(questions[currentQNo - 1]);  
    }

    private void SetQuestionNo() {
        questionNo.text = "Question number " + currentQNo.ToString() + " out of " + questions.Count();
    }

    private void SetQuestionText (string question) {
        questionText.text = question;
    }

    private void YesBtnPressed() {
        yesSellected = true;
        noSellected = unsureSellected = rnsSellected = false;

        nextQuestion.SetActive(true);
    }   

    private void NoBtnPressed() {
        noSellected = true;
        yesSellected = unsureSellected = rnsSellected = false;

        nextQuestion.SetActive(true);
    }   

    private void UnsureBtnPressed() {
        unsureSellected = true;
        yesSellected = noSellected = rnsSellected = false;

        nextQuestion.SetActive(true);
    }   

    private void RnsBtnPressed() {
        rnsSellected = true;
        yesSellected = noSellected = unsureSellected = false;

        nextQuestion.SetActive(true);
    }

    private void NextQPressed() {
        if(currentQNo >= questions.Count() ) {
            WriteAnswer();
            SaveAnswers();
            SceneManager.LoadScene("Main Menu");
        }
        else UpdateQuestion();
    }

    /// <summary>
    /// Saves answers into DB for potential future reference. 
    /// </summary>
    private void SaveAnswers() {
        
        using (var connection = new SqliteConnection(DeviceVariables.database)) {
            connection.Open();
            var command = connection.CreateCommand();

            command.CommandText = "INSERT INTO questionnaire (date) VALUES ( '" + DateTime.Today.ToString("dd-MM-yyyy") + "' );";
            command.ExecuteNonQuery();

            for (int i = 0; i < questions.Count(); i++)
            {
                command.CommandText = @"INSERT INTO questions (questionnaire_id, question ,answer)
                                        VALUES  ((SELECT id FROM questionnaire
                                                    WHERE date = '" + DateTime.Today.ToString("dd-MM-yyyy") + "'), '" + 
                                                questions[i] + "', '" + 
                                                givenAnswers[i] + "' );";
                command.ExecuteNonQuery();
            }
            connection.Close();
        }
    }

    private void WriteAnswer() {
        if(yesSellected) givenAnswers.Add("Yes");
        if(noSellected) givenAnswers.Add("No");
        if(unsureSellected) givenAnswers.Add("Unsure");
        if(rnsSellected) givenAnswers.Add("Rather Not Say");

        yesSellected = noSellected = unsureSellected = rnsSellected = false;
    }

    private void UpdateQuestion() {
        if(questions[currentQNo] == questions.Last()){
            nextText.text = "Finish";
        }
        currentQNo ++;
        nextQuestion.SetActive(false);

        WriteAnswer();

        SetQuestionNo();
        SetQuestionText(questions[currentQNo - 1]);
    }

}
