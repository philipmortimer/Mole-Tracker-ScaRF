using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that stores the static variables for sending emails. Variables are set in the diary scene and accessed by the email scene. 
/// </summary>
public class EmailVariables : MonoBehaviour
{
    public static string patientName = "";
    public static string patientDob = "";
    public static string patientEmail = "";
    public static string moleName = "";
    public static string farMolePhotoPath = "";
    public static string gender = "";
    public static List<(string photoPath, string dateTaken)> moleImagesToSend = new List<(string, string)>();
    public static List<(string question, string answer)> questionnaire = new List<(string, string)>();
}
