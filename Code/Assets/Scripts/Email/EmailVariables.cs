using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that stores the static variables for sending emails. Variables are set in the diary scene and accessed by the email scene. 
/// </summary>
public class EmailVariables : MonoBehaviour
{
    public static List<string> moleImagesToSend = new List<string>();
    public static List<(string question, string answer)> questionnaire = new List<(string, string)>();
}
