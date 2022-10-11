using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeviceVariables : MonoBehaviour
{

/// <summary>
/// Class that sores the static variables for the database path and the image path. Accessed by all other classes.
/// </summary>

    public static string database =  "URI=file:" + Application.persistentDataPath + "/SCaRF_dB.s3db";
    public static string imagesPath = Application.persistentDataPath + "/";
}
