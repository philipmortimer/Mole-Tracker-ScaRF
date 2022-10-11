using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using Mono.Data.Sqlite;
using System.Data;
using System.IO;

/// <summary>
/// <para>
/// Class is activated when a mole is selected, either in the scrollview or
/// by clicking on a marker.
/// </para>
/// Restores the camera position and orientation as saved when marker was
/// initially placed and enables context relevant options and buttons. 
/// </summary>
public class MoleActionHandler : MonoBehaviour
{
    public Button openDiary, newPhoto, close, moleButton;

    public GameObject moleActions, addMoleButton, scrollView;

    private Vector3 oldCamPos, oldCamRot;

    void Start()
    {
        openDiary.onClick.AddListener(GoToDiary);
        newPhoto.onClick.AddListener(TakeANewImage);
        close.onClick.AddListener(ClosePopUp);
    }


    private void OnEnable()
    {
        scrollView.SetActive(false);
        SetMoleButton();
    }

    /// <summary>
    /// Queries DB for the mole which has been selected (recorded in
    /// HighlightMole class) and updates the button with the correct info.
    /// </summary>
    private void SetMoleButton()
    {
        using (var connection = new SqliteConnection(DeviceVariables.database))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM moles WHERE id='" + HighlightMole.selectedMole + "';";

                using (IDataReader reader = command.ExecuteReader())
                {
                    reader.Read();

                    moleButton.transform.Find("Mole Name").GetComponent<TMPro.TMP_Text>().text = reader["name"].ToString();
                    moleButton.transform.Find("Days Remaining").GetComponent<TMPro.TMP_Text>().text = reader["far_shot_date"].ToString();
                    string im_path = reader["far_shot_path"].ToString();

                    reader.Close();

                    Texture2D imageTexture = new Texture2D(1, 1);

                    byte[] bytes = File.ReadAllBytes(DeviceVariables.imagesPath + im_path);

                    imageTexture.LoadImage(bytes);
                    imageTexture.Apply();

                    moleButton.transform.Find("Mole Image").GetComponent<RawImage>().texture = imageTexture;
                }
            }
        }
    }


    /// <summary>
    /// Store camera position and rotation
    /// </summary>
    /// <param name="pos">Position of cameras transform</param>
    /// <param name="rot">Rotation (Euler Angles) of cameras transform</param>
    public void StoreOldCamDetails(Vector3 pos, Vector3 rot)
    {
        oldCamPos = pos;
        oldCamRot = rot;
    }


    /// <summary>
    /// Restores prior camera position and rotation
    /// </summary>
    public void RestoreCameraPosition()
    {
        Camera.main.transform.position = oldCamPos;
        Camera.main.transform.eulerAngles = oldCamRot;

    }


    /// <summary>
    /// Restores the scene to previous status and set's HighlightMole to an
    /// arbitrarily large number.
    /// </summary>
    private void ClosePopUp()
    {
        moleActions.SetActive(false);
        addMoleButton.SetActive(true);
        HighlightMole.selectedMole = 9999;

        scrollView.SetActive(true);

        RestoreCameraPosition();

    }

    private void TakeANewImage()
    {
        PhotoVariables.moleID = HighlightMole.selectedMole;
        CameraScriptNew.previousScene = "3DModelScene";
        SceneManager.LoadScene("Camera");
    }

    private void GoToDiary()
    {
        DBId.moleId = HighlightMole.selectedMole.ToString();
        SceneManager.LoadScene("Mole Information");
    }
}
