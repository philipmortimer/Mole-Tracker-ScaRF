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

public class CameraScript : MonoBehaviour
{
    public GameObject canvas, ghostImage, advicetext;
    public Button savePhoto, takePhoto, retakePhoto;
    public Color inActive, active, defaultColor;
    private WebCamTexture webcamTexture;
    private Renderer rend;
    private Texture2D newImageTexture;
    private bool imageTaken = false;
    public static string previousScene = "Main Menu";
    

    byte[] bytes;
    private string newFilePath;   

    void Start()
    {
        savePhoto.onClick.AddListener(SavePhoto);
        takePhoto.onClick.AddListener(TakePhoto);
        retakePhoto.onClick.AddListener(RetakePhoto);

        savePhoto.GetComponent<Image>().color = inActive;
        retakePhoto.GetComponent<Image>().color = inActive;
        takePhoto.GetComponent<Image>().color = defaultColor;

        advicetext.GetComponent<TMPro.TMP_Text>().text = "For better image consistency, please use the outline of your previous photo to line up your next one.";

        webcamTexture = new WebCamTexture();
        rend = GetComponent<Renderer>();
        rend.material.mainTexture = webcamTexture;
        webcamTexture.Play();


        Texture2D texture = new Texture2D(1, 1);
        bytes = File.ReadAllBytes(DeviceVariables.imagesPath + getNearShotImagePath());
        texture.LoadImage(bytes);
        texture.Apply();
        ghostImage.GetComponent<RawImage>().texture = texture;

        DetailsToSave();
    }

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
        //Save the image to the Texture2D
        newImageTexture = new Texture2D(rend.material.mainTexture.width, rend.material.mainTexture.height, TextureFormat.ARGB32, false);
        newImageTexture.SetPixels(webcamTexture.GetPixels());
        newImageTexture.Apply();
        webcamTexture.Pause();

        imageTaken = true;
        advicetext.GetComponent<TMPro.TMP_Text>().text = "Please save your photo, or click retry if you are not happy with it.";
        savePhoto.GetComponent<Image>().color = active;
        retakePhoto.GetComponent<Image>().color = defaultColor;
        takePhoto.GetComponent<Image>().color = inActive;
    }

    void SavePhoto()
    {     
        if (imageTaken)
        {
            //Encode it as a PNG.
            byte[] bytes = newImageTexture.EncodeToPNG();

            //Save it in a file.
            File.WriteAllBytes(DeviceVariables.imagesPath + newFilePath, bytes);

#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif

            SaveDetailsToDatabase();

            webcamTexture.Stop();
            imageTaken = false;
            SceneManager.LoadScene(previousScene);
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
        takePhoto.GetComponent<Image>().color = defaultColor;

        webcamTexture.Play();
    }
}
