using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mono.Data.Sqlite;

/// <summary>
/// Handles the behaviour of the SQOLIT survey section of the app. 
/// </summary>
public class SurveySceneManager : MonoBehaviour
{
    public List<GameObject> scenes;
    public GameObject nextQObj;
    public Button next, back, nextQuestion, yesPlease, noThanks;
    public Button veryMuch, moderately, somewhat, notAtAll;
    public TMPro.TMP_Text questionNumber, questionText, scoreText;
    public Color activeColour, inactiveColour;

    public List<Button> buttons;
    private List<string> questions, answers;
    private int currentSceneIndex, questionIndex;
    private int score = 0;
   
    void Start()
    {
        InitialiseScenes();
        InitialiseQuestionsAndAnswers();
        
        next.onClick.AddListener(LoadNextScreen);
        nextQuestion.onClick.AddListener(LoadNextQuestion);
        back.onClick.AddListener(LoadPreviousQuestion);
        yesPlease.onClick.AddListener(SaveResults);
        noThanks.onClick.AddListener(() => SceneManager.LoadScene("Main Menu"));

        SetUpQuestionButtonListeners();
    }

    /// <summary>
    /// Saves answers to DB for optional sending to clinician. 
    /// </summary>
    private void SaveResults()
    {
        using (var connection = new SqliteConnection(DeviceVariables.database))
        {
            connection.Open();

            for (int i = 0; i < questions.Count(); i++)
            {
                var command = connection.CreateCommand();

                command.CommandText = "INSERT INTO survey (question, answer, date) VALUES ( '" +
                    questions[i] + "', '" + answers[i] + "', '" + DateTime.Today.ToString("dd-MM-yyyy") + "' );";

                command.ExecuteNonQuery();
            }
            connection.Close();
        }
        SceneManager.LoadScene("Main Menu");
    }

    private void InitialiseScenes()
    {
        foreach (var scene in scenes)
        {
            scene.SetActive(false);
        }
        scenes[0].SetActive(true);
        currentSceneIndex = 0;
    }


    /// <summary>
    /// Hard coded questions - could be redesigned for future flexibility.
    /// </summary>
    private void InitialiseQuestionsAndAnswers()
    {
        questions = new List<string> {
            "Over the last week, how much have you been concerned that your skin cancer might come back?",
            "Over the last week, how much have you felt that you needed more information on how to recognize skin cancer or prevent it?",
            "Over the last week, how much have you been worried about covering up your skin and keeping out of the sun?",
            "Over the last week, how much have you felt a need for reassurance from your doctor or nurse, in repect to your skin cancer or its treatment?",
            "Over the last week, how much have you felt emotional, anxious, depressed, guilty or stressed, in repect to your skin cancer or its treatment?",
            "Over the last week, how much have you been bothered about disfigurement or scarring, in respect to your skin cancer or its treatment?",
            "Over the last week, how much have you felt shock or disbelief about having been diagnosed with skin cancer?",
            "Over the last week, how much skin discomfort or inconvenience have you experienced, in respect to your skin cancer or its treatment?",
            "Over the last week, how much have you had concerns about dying from skin cancer?",
            "Over the last week, to what extent have you felt the need for emotional support from your family or friends, in respect to your skin cancer or its treatment?"};

        questionIndex = 0;

        answers = new();
        for (int i = 0; i < questions.Count; i++)
        {
            answers.Add("null");
        }
    }

    private void SetUpQuestionButtonListeners()
    {
        veryMuch.onClick.AddListener(() => SelectButton(veryMuch));
        moderately.onClick.AddListener(() => SelectButton(moderately));
        somewhat.onClick.AddListener(() => SelectButton(somewhat));
        notAtAll.onClick.AddListener(() => SelectButton(notAtAll));
    }

    private void LoadNextQuestion()
    {
        answers[questionIndex] = FindSelectedAnswer();

        if (questionIndex >= questions.Count - 1)
        {
            UpdateScore();
            LoadNextScreen();
        }

        else
        {
            questionIndex++;
            UpdateQuestionDetails();
        }
        nextQObj.SetActive(false);
    }

    private void LoadPreviousQuestion()
    {
        answers[questionIndex] = FindSelectedAnswer();

        if (questionIndex <= 0)
        {
            InitialiseScenes();
        }
        else
        {
            questionIndex--;
            UpdateQuestionDetails();
        }
    }

    private void UpdateQuestionDetails()
    {
        questionNumber.text = "Question " + (questionIndex + 1) + " of " + questions.Count + ":";
        questionText.text = questions[questionIndex];
        nextQuestion.GetComponentInChildren<TMPro.TMP_Text>().text = (questionIndex == questions.Count - 1) ? "Finish" : "Next";
    }

    /// <summary>
    /// Calculates the final SQOLIT score based on stored answers.
    /// </summary>
    private void UpdateScore()
    {
        foreach (string answer in answers)
        {
            score += answer == "Very Much" ? 10 : answer == "Moderately" ? 7 : answer == "Somewhat" ? 4 : 0;
        }
        scoreText.text = "Your score is:   <size=50><color=#C9DFF9><b>" + score + " / 100 </b></color></size>";
    }

    private string FindSelectedAnswer()
    {
        string answer = "null";
        foreach (Button b in buttons)
        {
            if (b.GetComponent<Image>().color == activeColour)
            {
                answer = b.GetComponentInChildren<TMPro.TMP_Text>().text;
            }
            b.GetComponent<Image>().color = inactiveColour;
        }
        return answer;
    }

    private void SelectButton(Button button)
    {
        foreach (Button b in buttons)
        {
            b.GetComponent<Image>().color = (b == button) ? activeColour : inactiveColour;
        }
        nextQObj.SetActive(true);
    }

    private void LoadNextScreen()
    {
        scenes[currentSceneIndex].SetActive(false);
        currentSceneIndex++;
        scenes[currentSceneIndex].SetActive(true);
    }
}
