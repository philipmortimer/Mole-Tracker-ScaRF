using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_ANDROID
    using UnityEngine.Android;
#endif

/// <summary>
/// Class that runs when the application is first launched. Checks to see if the application has the necessary permssions.
/// </summary>
public class GetPermissions : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        CheckCameraPermission();
    }

    void CheckCameraPermission()
    {
        #if PLATFORM_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                Permission.RequestUserPermission(Permission.Camera);
            }
        #elif UNITY_IOS
            if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                Application.RequestUserAuthorization(UserAuthorization.WebCam);
            }
        #endif
        
    }
}

