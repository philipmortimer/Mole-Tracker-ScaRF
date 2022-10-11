using System.Collections;
using System.Collections.Generic;
using System.Data;
using Mono.Data.Sqlite;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Analytics;


/// <summary>
/// <para>
/// Class to handle the content and management of the context specific scroll view
/// in the 3D model scene.
/// </para>
/// The path a user takes through the various context options is stored and maintained
/// as a series of indexes in the contextPath list. These index reference GameObjects
/// contextControlsTree. 
/// </summary>
public class ScrollViewContent : MonoBehaviour
{
    [SerializeField] PlaceMoleMarker pmm;

    public GameObject content;
    public GameObject molePrefab;
    public GameObject humanBody;

    [SerializeField] private float defaultMoleScalar;

    public List<GameObject> contextControlsTree;

    public List<int> contextPath;
    public int lastContext = 0;
    private List<GameObject> moleList;

    void Start()
    {
        moleList = new();
        contextPath = new();
        LoadMoles();
        UpdateMoleMarkerSize(defaultMoleScalar);
    }

    /// <summary>
    /// Removes most recent index of the context tree traversed so far. 
    /// </summary>
    public void RemoveLastContextPathElement()
    {
        List<int> newPath = new();
        for (int i = 0; i < contextPath.Count - 1; i++)
        {
            newPath.Add(contextPath[i]);
        }
        contextPath = newPath;
    }

    /// <summary>
    /// Deactivates all gameObjects in the scroll view
    /// </summary>
    public void DeactivateContextControls()
    {
        foreach (GameObject c in contextControlsTree)
        {
            c.SetActive(false);
        }
    }

    /// <summary>
    /// Updates the size of mole markers on the  model. Used for small markers when the camera is zoomed out. 
    /// </summary>
    /// <param name="scalar">The value by which to scale the mole marker on all axis.</param>
    public void UpdateMoleMarkerSize(float scalar)
    {
        foreach (Transform child in humanBody.transform)
        {
            if (child.tag == "Mole")
            {
                float originalScale = child.GetComponent<MoleProperties>().GetOriginalScale();
                if (originalScale < 0.05)
                {
                    float newScale = originalScale * scalar;
                    child.transform.localScale = new Vector3(newScale, newScale, newScale);
                }
            }
        }
    }

    /// <summary>
    /// Handles the retrieval of the previous context in the context path, and updates the path list accordingly.
    /// </summary>
    public void ZoomOut()
    {
#if !UNITY_EDITOR
        Analytics.CustomEvent("Zoomed out with context controls"); 
#endif
        int prevIndex = contextPath[contextPath.Count - 2];
        DeactivateContextControls();
        RemoveLastContextPathElement();
        RemoveLastContextPathElement();
        ActivateContext(prevIndex);
    }

    /// <summary>
    /// Public function to activate/deactive the mole markers on the model.
    /// </summary>
    /// <param name="state">true = active</param>
    public void MoleListActivation(bool state)
    {
        foreach (GameObject m in moleList)
        {
            m.SetActive(state);
        }
    }

    /// <summary>
    /// Searches the contextControlsTree list for the GameObject with matching name.
    /// </summary>
    /// <param name="context">The name of the GameObject (must be exact match)</param>
    /// <returns></returns>
    public int FindIndex(string context)
    {
        for (int i = 0; i < contextControlsTree.Count; i++)
        {
            if (contextControlsTree[i].name == context)
            {
                return i;
            }
        }
        return 0;
    }

    /// <summary>
    /// Updates the active context to the one with name matching the passed string.
    /// </summary>
    /// <param name="context">The name of the context (GameObject) to be activated.</param>
    public void SwitchContext(string context)
    {
        DeactivateContextControls();
        int index = FindIndex(context);
        ActivateContext(index);
    }

    /// <summary>
    /// Adds the next context index to the path and actives the GameObject
    /// </summary>
    /// <param name="index"></param>
    public void ActivateContext(int index)
    {
        contextPath.Add(index);
        contextControlsTree[index].SetActive(true);
    }

    /// <summary>
    /// Queries the database for all moles and adds a button to the scrollview
    /// with the farshot image of that mole. Also adds a marker to the model
    /// using the stored scale and position values. 
    /// </summary>
    void LoadMoles()
    {
        using (var connection = new SqliteConnection(DeviceVariables.database))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM moles;";
                using (IDataReader reader = command.ExecuteReader())
                {
                    //int currentY = 800;
                    while (reader.Read())
                    {   // spawn in mole prefab and make it child of canvas
                        GameObject m = Instantiate(molePrefab, new Vector3(0, 0, 0), Quaternion.identity);
                        m.transform.SetParent(content.transform, false);
                        moleList.Add(m);

                        m.AddComponent<ScrollViewSelectedMole>();

                        GameObject button = m.transform.Find("Button Object").gameObject;
                        GameObject moleImage = button.transform.Find("Mole Image").gameObject;
                        GameObject moleName = button.transform.Find("Mole Name").gameObject;
                        GameObject dateTaken = button.transform.Find("Days Remaining").gameObject;

                        string image_path = "";
                        image_path += reader["far_shot_path"];


                        Texture2D imageTexture = new Texture2D(1, 1);
                        byte[] bytes = File.ReadAllBytes(DeviceVariables.imagesPath + image_path);
                        imageTexture.LoadImage(bytes);
                        imageTexture.Apply();
   
                        moleImage.GetComponent<RawImage>().texture = imageTexture;
                        moleName.GetComponent<TMPro.TMP_Text>().text = reader["name"].ToString();
                        dateTaken.GetComponent<TMPro.TMP_Text>().text = reader["far_shot_date"].ToString();

                        m.SetActive(true);
                        //currentY -= 325;

                        // For the highlighting
                        MoleInfo mi = m.GetComponent<MoleInfo>();
             
                        int moleID = Convert.ToInt32(reader["id"]);
                        mi.moleId = moleID;

                        // Add mole marker to the 3D model & save position in moleInfo.
                        Vector3 position = new Vector3(Convert.ToSingle(reader["coordinate_x"]), Convert.ToSingle(reader["coordinate_y"]), Convert.ToSingle(reader["coordinate_z"]));
                        float scale = Convert.ToSingle(reader["marker_scale"]);
                        pmm.AddMoleMarker(position, scale, moleID, name);
                    }
                    reader.Close();
                }
            }
            connection.Close();
        }
    }
}
