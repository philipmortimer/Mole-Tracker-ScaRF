using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that stores all variables needed for taking photos of moles.
/// </summary>
public class PhotoVariables : MonoBehaviour
{
    public static int moleID, reminder;
    public static bool nameMole = false;
    public static bool openCamera = false;
    public static string moleName;
    public static string date;
    public static float x, y, z;
    public static float camX, camY, camZ;
    public static float camRotX, camRotY, camRotZ;
    public static float targetX, targetY, targetZ;
    public static float camFOV;
    public static float scale;
}
