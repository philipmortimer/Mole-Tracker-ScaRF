using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using System.Linq;
using System.Data;
using Mono.Data.Sqlite;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;

/// <summary>
/// <para>
/// Class that is responsible for when the user wants to take a new photo of their mole.
/// </para>
/// This class doesn't handle any of the camera functionality when adding a new mole.
/// </summary>
public class CameraScriptNew : MonoBehaviour
{

    private bool camAvailable;
    private bool multipleCameras = true;
    private bool imageTaken = false;
    private WebCamTexture backCam, frontCam, currentCam;
    public Button savePhoto, takePhoto, retakePhoto, home;
    public Image takePhotoImage;
    public Color inActive, active, defaultColor;
    private Texture defaultBackground;
    private Texture2D newImageTexture;
    public RawImage background;
    public AspectRatioFitter fit, fitGhost;
    public GameObject switchCamera, advicetext, ghostImage;
    public Button switchCameraButton;
    byte[] bytes;
    private string newFilePath;

    public static string previousScene = "Main Menu";

    void Start()
    {
        SetupCameras();
        SetupGhostImage();

        SetupButtons();
        DetailsToSave();
    }

    /// <summary>
    /// Function that retrieves that appropriate mole image from device storage and places it over the camera scene. 
    /// </summary>
    private void SetupGhostImage()
    {
        Texture2D texture = new Texture2D(1, 1);
        bytes = File.ReadAllBytes(DeviceVariables.imagesPath + getNearShotImagePath());
        texture.LoadImage(bytes);
        texture.Apply();
        float ratio = (float)texture.width / (float)texture.height;
        fitGhost.aspectRatio = ratio;
        ghostImage.GetComponent<RawImage>().texture = texture;
    }


    void Update()
    {
        if (!camAvailable) return;

        float ratio = (float)currentCam.width / (float)currentCam.height;
        fit.aspectRatio = ratio;
        float scaleY = currentCam.videoVerticallyMirrored ? 1f: 1f;
        int orient = -currentCam.videoRotationAngle;
        background.rectTransform.localEulerAngles = new Vector3(0,0,orient);
    }

    /// <summary>
    /// Function that first checks which cameras the user has available (front and back) and then attaches them to the application.
    /// </summary>
    void SetupCameras()
    {
        switchCameraButton.onClick.AddListener(SwitchCamera);

        defaultBackground = background.texture;
        WebCamDevice[] devices = WebCamTexture.devices;
        //Debug.Log(devices.Length);

        if (devices.Length == 0)
        {
            //Debug.Log("no camera detected");
            camAvailable = false;
            multipleCameras = false;
            return;
        }

        for (int i = 0; i < devices.Length; i++)
        {
            if (!devices[i].isFrontFacing)
            {
                backCam = new WebCamTexture(devices[i].name, Screen.width, Screen.height);
            }
            if (devices[i].isFrontFacing)
            {
                frontCam = new WebCamTexture(devices[i].name, Screen.width, Screen.height);
            }
        }

        if (backCam == null)
        {
            //Debug.Log("no back camera detected");
            multipleCameras = false;
            currentCam = frontCam;
        }
        else 
        {
            //Debug.Log("no front camera detected");
            currentCam = backCam;
        }

        if (multipleCameras)
        {
            switchCamera.SetActive(true);
        } else {
            switchCamera.SetActive(false);
        }

        currentCam.Play();
        background.texture = currentCam;
        camAvailable = true;
    }

    /// <summary>
    /// Function that is attached to the switch camera button. Changes which camera is being used. 
    /// </summary>
    void SwitchCamera()
    {
        if (currentCam == backCam){
            currentCam.Stop();
            currentCam = frontCam;
            currentCam.Play();
            background.texture = currentCam;
        }
        else
        {
            currentCam.Stop();
            currentCam = backCam;
            currentCam.Play();
            background.texture = currentCam;
        }
    }

    /// <summary>
    /// Function that attaches relevant methods to the camera scenes buttons. Also deals with the starting colours of the buttons and sets the initial text.
    /// </summary>
    void SetupButtons()
    {
        savePhoto.onClick.AddListener(SavePhoto);
        takePhoto.onClick.AddListener(TakePhoto);
        retakePhoto.onClick.AddListener(RetakePhoto);
        home.onClick.AddListener(GoBack);

        savePhoto.GetComponent<Image>().color = inActive;
        retakePhoto.GetComponent<Image>().color = inActive;
        takePhotoImage.GetComponent<Image>().color = defaultColor;

        advicetext.GetComponent<TMPro.TMP_Text>().text = "For better image consistency, please use the outline of your previous photo to line up your next one.";
    }

    /// <summary>
    /// Function that returns the image path of the most recent image of a particular mole. 
    /// </summary>
    string getNearShotImagePath()
    {
        //// TODO: This needs to change, mole id should be set when opening scene!!!!
        //PhotoVariables.moleID = 2;

        string path = "";
        using (var connection = new SqliteConnection(DeviceVariables.database))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM individual_mole_photos WHERE mole_id = " + PhotoVariables.moleID + " ORDER BY id DESC LIMIT 1;";
                using (IDataReader reader = command.ExecuteReader())
                {
                    path += reader["near_shot_path"];
                    reader.Close();
                }
            }
            connection.Close();
        } 
        return path;
    }
    
    /// <summary>
    /// Function to determine the filename of the new image to be saved. 
    /// </summary>
    void DetailsToSave()
    {
        int newId = 0;
        string moleName = "";
        string tempFilePath;

        // Get ID of most recent near shot, increment bt one to determine what the ID of the new mole will be.
        using (var connection = new SqliteConnection(DeviceVariables.database))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT COUNT(id) FROM individual_mole_photos WHERE mole_id = " + PhotoVariables.moleID + " ORDER BY id DESC LIMIT 1;";
                newId = Convert.ToInt32(command.ExecuteScalar());
                newId += 1;
            }
            connection.Close();
        } 

        // Get the name of the mole that the near shot relates to
        using (var connection = new SqliteConnection(DeviceVariables.database))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT name FROM moles WHERE id = " + PhotoVariables.moleID + ";";
                using (IDataReader reader = command.ExecuteReader())
                {
                    moleName += reader["name"];
                }
            }
            connection.Close();
        } 

        tempFilePath = moleName + "NearShot" + newId + ".png";
        newFilePath = String.Concat(tempFilePath.Where(c => !Char.IsWhiteSpace(c)));
    }

    /// <summary>
    /// Function that saves the relevant details about the new image. 
    /// </summary>
    void SaveDetailsToDatabase()
    {
        using (var connection = new SqliteConnection(DeviceVariables.database))
        {
            connection.Open();
            DateTime dt = DateTime.Now;
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO individual_mole_photos (mole_id, near_shot_path, near_shot_date) VALUES (" + PhotoVariables.moleID + ", '" + newFilePath + "', '" + dt.ToString("dd-MM-yyyy") + "');";
                command.ExecuteNonQuery();
            }
            connection.Close();
        } 
    }
    void TakePhoto()
    {
        // TODO: Try rotating camera, saving image and rotating back again?

        //Save the image to the Texture2D
        newImageTexture = new Texture2D(background.texture.width, background.texture.height, TextureFormat.ARGB32, false);
        newImageTexture.SetPixels(currentCam.GetPixels());
        newImageTexture.Apply();
        currentCam.Pause();

        imageTaken = true;
        advicetext.GetComponent<TMPro.TMP_Text>().text = "Please save your photo, or click retry if you are not happy with it.";
        savePhoto.GetComponent<Image>().color = active;
        retakePhoto.GetComponent<Image>().color = defaultColor;
        takePhotoImage.GetComponent<Image>().color = inActive;
    }

    /// <summary>
    /// Function that saves the new image to device storage.
    /// </summary>
    void SavePhoto()
    {     
        if (imageTaken)
        {
            //Encode it as a PNG.
            byte[] bytes;
            if (currentCam == backCam)
            {
                //bytes = rotate(newImageTexture).EncodeToPNG();
                bytes = RotateTexture(newImageTexture, true).EncodeToPNG();
            }
            else {
                //bytes = rotate(rotate(rotate(newImageTexture))).EncodeToPNG();
                bytes = RotateTexture(newImageTexture, false).EncodeToPNG();
            }

            //Save it in a file.
            File.WriteAllBytes(DeviceVariables.imagesPath + newFilePath, bytes);

#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif

            SaveDetailsToDatabase();

            currentCam.Stop();
            imageTaken = false;
            SceneManager.LoadScene("Main Menu");
        }

        else
        {
            advicetext.GetComponent<TMPro.TMP_Text>().text = "Please take an image using the middle button before saving.";
        }
    }
    void RetakePhoto()
    {
#if !UNITY_EDITOR
        Analytics.CustomEvent("Retook new nearshot photo");
#endif
        advicetext.GetComponent<TMPro.TMP_Text>().text = "Please use the outline of your previous photo to line up your next one.";

        savePhoto.GetComponent<Image>().color = inActive;
        retakePhoto.GetComponent<Image>().color = inActive;
        takePhotoImage.GetComponent<Image>().color = defaultColor;

        currentCam.Play();
    }
    void GoBack()
    {
        SceneManager.LoadScene(previousScene);
    }

    /// <summary>
    /// Function that rotates the image texture to be the correct orientation before saving. Unity is weird. 
    /// </summary>
    Texture2D RotateTexture(Texture2D originalTexture, bool clockwise)
    {
        Color32[] original = originalTexture.GetPixels32();
        Color32[] rotated = new Color32[original.Length];
        int w = originalTexture.width;
        int h = originalTexture.height;

        int iRotated, iOriginal;

        for (int j = 0; j < h; ++j)
        {
            for (int i = 0; i < w; ++i)
            {
                iRotated = (i + 1) * h - j - 1;
                iOriginal = clockwise ? original.Length - 1 - (j * w + i) : j * w + i;
                rotated[iRotated] = original[iOriginal];
            }
        }

        Texture2D rotatedTexture = new Texture2D(h, w);
        rotatedTexture.SetPixels32(rotated);
        rotatedTexture.Apply();
        return rotatedTexture;
    }
}