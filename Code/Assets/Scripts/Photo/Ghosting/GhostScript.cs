using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using Mono.Data.Sqlite;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;
using UnityEditor;
using TMPro;

/// <summary>
/// <para>
/// Class that handles adding a new mole. This class is very similar to the class responsible for adding a new mole (CameraScritNew) but it has some additions.
/// </para>
/// Handles the near and far shot. Most of the functions are the same as CameraScriptNew.
/// </summary>
public class GhostScript : MonoBehaviour
{
   
    public GameObject canvas, humanBody, redRing;
    private GameObject modelInstance;
    public Camera mainCam;
    public Material ghostMaterial;
    public Button savePhoto, takePhoto, retakePhoto, home;
    public Image takePhotoImage;
    public Color inActive, active, defaultColor;
    private WebCamTexture backCam, frontCam, currentCam;
    Renderer rend;
    public AspectRatioFitter fit;
    private GameObject humanBodyMeshForMaterial;
    private Texture2D farTexture, nearTexture;
    private bool imageTaken;
    public TMP_Text adviceText;
    public GameObject switchCamera;
    public Button switchCameraButton;
    private bool taken, farShotDone = false;
    private bool multipleCameras = true;
    private string FarFilePath, NearFilePath;


    void Start()
    {
        SetupCameras();

        savePhoto.onClick.AddListener(SaveFarShot);
        takePhoto.onClick.AddListener(TakeFarShot);
        retakePhoto.onClick.AddListener(RetakePhoto);

        savePhoto.GetComponent<Image>().color = inActive;
        retakePhoto.GetComponent<Image>().color = inActive;
        takePhotoImage.GetComponent<Image>().color = defaultColor;

        GetActiveModel();
        redRing.SetActive(false);
    }


    void Update()
    {
        if (PhotoVariables.openCamera)
        {
            PhotoVariables.openCamera = false;
            mainCam.clearFlags = CameraClearFlags.Depth;
            LaunchCamera();
        }

        if (taken) 
        {
            SceneManager.LoadScene("3DModelScene");
        }


        if (currentCam == frontCam)
        {
            this.gameObject.GetComponent<Transform>().localEulerAngles = new Vector3(0f,90f,270f);
        }
        else {
            this.gameObject.GetComponent<Transform>().localEulerAngles = new Vector3(180f,90f,270f);
        }
    }

    /// <summary>
    /// Function that gets the game object for the active 3D model so that the new material can be applied.
    /// </summary>
    private void GetActiveModel()
    {
        for (int i = 0; i < humanBody.transform.childCount; i++)
        {
            GameObject child = humanBody.transform.GetChild(i).gameObject;
            if (child.activeSelf == true && child.CompareTag("3D Model"))
            {
                modelInstance = humanBody.transform.GetChild(i).gameObject;
            }
        }
    }
    void SetupCameras()
    {
        switchCameraButton.onClick.AddListener(SwitchCamera);
        home.onClick.AddListener(GoBack);

        WebCamDevice[] devices = WebCamTexture.devices;
        //Debug.Log(devices.Length);

        if (devices.Length == 0)
        {
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
    }
    void SwitchCamera()
    {
        if (currentCam == backCam){
            currentCam.Stop();
            currentCam = frontCam;
            currentCam.Play();
            rend.material.mainTexture = currentCam;
        }
        else
        {
            currentCam.Stop();
            currentCam = backCam;
            currentCam.Play();
            rend.material.mainTexture = currentCam;
        }
    }
    void LaunchCamera()
    {
        DeactivateScrollViewElements();
        humanBody.transform.Find("Moles").gameObject.SetActive(false);

        GameObject cb = canvas.transform.Find("CameraButtons").gameObject;
        cb.SetActive(true);

        SetGhostMaterial();
        humanBody.SetActive(true);

        rend = GetComponent<Renderer>();
        currentCam.Play();
        rend.material.mainTexture = currentCam;

        farTexture = new Texture2D(rend.material.mainTexture.width, rend.material.mainTexture.height, TextureFormat.ARGB32, false);
        nearTexture = new Texture2D(rend.material.mainTexture.width, rend.material.mainTexture.height, TextureFormat.ARGB32, false);

        adviceText.text = "You now need to take a far-shot picture of your mole. Please use the body section as a guide to take the photo.";

        FarFilePath = PhotoVariables.moleName + ".png";

    }

    /// <summary>
    /// Function that deactivates the canvas elements of the 3D model scene when taking a photo.
    /// </summary>
    private void DeactivateScrollViewElements()
    {
        GameObject sv = canvas.transform.Find("Scroll View").gameObject;
        sv.SetActive(false);
        GameObject ni = canvas.transform.Find("Navigation Icons").gameObject;
        ni.SetActive(false);
        GameObject amb = canvas.transform.Find("Add Mole Button").gameObject;
        amb.SetActive(false);
    }

    /// <summary>
    /// Function that applies the transparent material to the 3D model to be used as a ghost image.
    /// </summary>
    private void SetGhostMaterial()
    {
        foreach (Transform t in modelInstance.transform)
        {
            if (t.CompareTag("ModelMesh"))
            {
                humanBodyMeshForMaterial = t.gameObject;
            }
        }
        humanBodyMeshForMaterial.GetComponent<Renderer>().material = ghostMaterial;
    }

    void TakeFarShot()
    {
        farTexture = new Texture2D(rend.material.mainTexture.width, rend.material.mainTexture.height, TextureFormat.ARGB32, false);
        farTexture.SetPixels(currentCam.GetPixels());
        farTexture.Apply();
        currentCam.Pause();

        imageTaken = true;
        adviceText.GetComponent<TMPro.TMP_Text>().text = "Please save your photo, or click 'Retry' if you are not happy with it.";
        savePhoto.GetComponent<Image>().color = active;
        retakePhoto.GetComponent<Image>().color = defaultColor;
        takePhotoImage.GetComponent<Image>().color = inActive;
    }

    void SaveFarShot()
    {
        if (imageTaken)
        {
            //Encode it as a PNG.
            byte[] bytes;
            if (currentCam == backCam)
            {
                //bytes = rotate(newImageTexture).EncodeToPNG();
                bytes = RotateTexture(farTexture, true).EncodeToPNG();
            }
            else {
                //bytes = rotate(rotate(rotate(newImageTexture))).EncodeToPNG();
                bytes = RotateTexture(farTexture, false).EncodeToPNG();
            }

            //Save it in a file.
            File.WriteAllBytes(DeviceVariables.imagesPath + FarFilePath, bytes);

#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif

            currentCam.Play();
            savePhoto.onClick.RemoveListener(SaveFarShot);
            takePhoto.onClick.RemoveListener(TakeFarShot);

            farShotDone = true;
            imageTaken = false;

            savePhoto.onClick.AddListener(SaveNearShot);
            takePhoto.onClick.AddListener(TakeNearShot);
            humanBody.SetActive(false);
            redRing.SetActive(true);

            savePhoto.GetComponent<Image>().color = inActive;
            retakePhoto.GetComponent<Image>().color = inActive;
            takePhotoImage.GetComponent<Image>().color = defaultColor;

            adviceText.text = "You now need to take a close-up of your mole. Please use the red circle as a guide.";
        }
        else
        {
            adviceText.GetComponent<TMPro.TMP_Text>().text = "Please take an image using the middle button before saving.";
        }
        
    }

    void RetakePhoto()
    {
        string analyticsRecord = farShotDone ? "Retried near shot photo" : "Retried far shot photo";
#if !UNITY_EDITOR
        Analytics.CustomEvent(analyticsRecord);
#endif
        adviceText.GetComponent<TMPro.TMP_Text>().text = farShotDone ?
            "You now need to take a close-up of your mole. Please use the red circle as a guide." :
            "You now need to take a far-shot picture of your mole. Please use the body section as a guide to take the photo.";
        savePhoto.GetComponent<Image>().color = inActive;
        retakePhoto.GetComponent<Image>().color = inActive;
        takePhotoImage.GetComponent<Image>().color = defaultColor;

        currentCam.Play();
    }

    void TakeNearShot()
    {
        nearTexture = new Texture2D(rend.material.mainTexture.width, rend.material.mainTexture.height, TextureFormat.ARGB32, false);
        nearTexture.SetPixels(currentCam.GetPixels());
        nearTexture.Apply();
        currentCam.Pause();

        imageTaken = true;
        adviceText.GetComponent<TMPro.TMP_Text>().text = "Please save your photo, or click 'Retry' if you are not happy with it.";
        savePhoto.GetComponent<Image>().color = active;
        retakePhoto.GetComponent<Image>().color = defaultColor;
        takePhotoImage.GetComponent<Image>().color = inActive;
    }

    void SaveNearShot()
    {
        if (imageTaken)
        {
            NearFilePath = PhotoVariables.moleName + "NearShot1.png";

            //Encode it as a PNG.
            byte[] bytes;
            if (currentCam == backCam)
            {
                bytes = RotateTexture(nearTexture, true).EncodeToPNG();
            }
            else {
                bytes = RotateTexture(nearTexture, false).EncodeToPNG();
            }

            //Save it in a file.
            File.WriteAllBytes(DeviceVariables.imagesPath + NearFilePath, bytes);

            currentCam.Stop();

#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif

            CompleteAddingMole();
        }
        else
        {
            adviceText.GetComponent<TMPro.TMP_Text>().text = "Please take an image using the middle button before saving.";
        }
    }

    private void CompleteAddingMole()
    {
        AddFarShot();
        AddNearShot();
#if !UNITY_EDITOR
        Analytics.CustomEvent("Successfully added and saved a new mole");
#endif
        SceneManager.LoadScene("Success");
    }

    void AddFarShot()
    {
        using (var connection = new SqliteConnection(DeviceVariables.database))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO moles (name, far_shot_path, coordinate_x, coordinate_y, coordinate_z, marker_scale, far_shot_date, reminder) VALUES ('"
                    + PhotoVariables.moleName + "', '"+ FarFilePath +"', "+PhotoVariables.x+", "+PhotoVariables.y+", "+PhotoVariables.z+", "
                    +PhotoVariables.scale+", '"+PhotoVariables.date+"', "+PhotoVariables.reminder+");";
                command.ExecuteNonQuery();

                command.CommandText = "SELECT id FROM moles WHERE name = '" + PhotoVariables.moleName + "';";
                int moleId = Convert.ToInt32(command.ExecuteScalar());

                command.CommandText = "INSERT INTO cam_details (mole_id, xPos, yPos, zPos, xRot, yRot, zRot, fov, targetx, targety, targetz) VALUES (" + moleId + ", " + PhotoVariables.camX +
                    ", " + PhotoVariables.camY + ", " + PhotoVariables.camZ + ", " + PhotoVariables.camRotX + ", " + PhotoVariables.camRotY + ", " + PhotoVariables.camRotZ +
                    ", " + PhotoVariables.camFOV + ", " + PhotoVariables.targetX + ", " + PhotoVariables.targetY + ", " + PhotoVariables.targetZ + ")";
                command.ExecuteNonQuery();
            }
            connection.Close();
        } 
    }

    void AddNearShot()
    {

        int IDofMoleAdded;

        using (var connection = new SqliteConnection(DeviceVariables.database))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT id FROM moles WHERE name = '" + PhotoVariables.moleName + "';";
                IDofMoleAdded = Convert.ToInt32(command.ExecuteScalar());
            }
            connection.Close();
        }

        using (var connection = new SqliteConnection(DeviceVariables.database))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO individual_mole_photos (mole_id, near_shot_path, near_shot_date) VALUES (" + IDofMoleAdded + ", '" + NearFilePath + "', '" + PhotoVariables.date + "');";
                command.ExecuteNonQuery();
            }
            connection.Close();
        } 
    }

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

    void GoBack()
    {
        SceneManager.LoadScene("3DModelScene");
    }
}
