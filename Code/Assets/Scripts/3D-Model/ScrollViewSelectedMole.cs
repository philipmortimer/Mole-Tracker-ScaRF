using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Class to handle upding the scrollview when either a mole button is clicked
/// or a mole marker on the model is clicked. 
/// </summary>
public class ScrollViewSelectedMole : MonoBehaviour
{
    private Button btn;
    private CameraController camController;
    private GameObject addMoleButton, moleActions;
    private MoleInfo mi;
    private MoleActionHandler mah;
    
    void Start()
    {
        btn = transform.Find("Button Object").GetComponent<Button>();
        btn.onClick.AddListener(SetSelectedMole);
        camController = GameObject.Find("Cam Controller").GetComponent<CameraController>();
        mi = gameObject.GetComponent<MoleInfo>();
    }

    private void OnEnable()
    {
        var canvas = GameObject.Find("Canvas");
        addMoleButton = canvas.transform.Find("Add Mole Button").gameObject;
        moleActions = canvas.transform.Find("MoleActionOptions").gameObject;

        mah = moleActions.transform.Find("MoleActionHandler").GetComponent<MoleActionHandler>();
        camController = GameObject.Find("Cam Controller").GetComponent<CameraController>();
        mi = gameObject.GetComponent<MoleInfo>();
    }

    /// <summary>
    /// Sets the static variable of selectedMole in <see cref="HighlightMole"/> to
    /// the id of the clicked mole. Saves current camera transform information, then
    /// moves camera to focus on mole. 
    /// </summary>
    void SetSelectedMole()
    {
        HighlightMole.selectedMole = mi.moleId;
        addMoleButton.SetActive(false);
        moleActions.SetActive(true);


        Transform camTransform = Camera.main.transform;
        mah.StoreOldCamDetails(camTransform.position, camTransform.eulerAngles);
        SetCameraToMole();
    }


    /// <summary>
    /// Calls <see cref="GetCameraDetails(int)"/>and sets new camera target/position.
    /// </summary>
    private void SetCameraToMole()
    {
        (Vector3 position, Vector3 rotation, Transform target) = GetCameraDetails(mi.moleId);
        camController.SetCameraTarget(target, Math.Abs(position.z));
        Camera.main.transform.eulerAngles = rotation;
        Camera.main.transform.position = position;
    }

    /// <summary>
    /// Queries the database for the camera transform details, saved when mole marker was intially added.
    /// </summary>
    /// <param name="mole_id">The id of the mole</param>
    /// <returns>Camera transform information</returns>
    private (Vector3 position, Vector3 rotation, Transform target) GetCameraDetails(int mole_id)
    {
        Transform target = camController.GetCameraTarget();
        Vector3 pos = Vector3.one;
        Vector3 rot = Vector3.one;

        using (var connection = new SqliteConnection(DeviceVariables.database))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM cam_details WHERE mole_id == " + mole_id + ";";
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        pos = new Vector3(Convert.ToSingle(reader["xPos"]), Convert.ToSingle(reader["yPos"]), Convert.ToSingle(reader["zPos"]));
                        rot = new Vector3(Convert.ToSingle(reader["xRot"]), Convert.ToSingle(reader["yRot"]), Convert.ToSingle(reader["zRot"]));
                        target.transform.position = new Vector3(Convert.ToSingle(reader["targetx"]), Convert.ToSingle(reader["targety"]), Convert.ToSingle(reader["targetz"]));
                    }
                }
            }
        }
        return (pos, rot, target);
    }
}
