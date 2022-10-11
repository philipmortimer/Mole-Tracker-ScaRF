using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;
using System;
using Mono.Data.Sqlite;
using System.Data;

/// <summary>
/// Main class handling button interation for the persistent buttons on 3D model scene.  
/// </summary>

public class ButtonHandling_3DBody : MonoBehaviour
{
    public Button add, exit;
    public GameObject body, addMolePrefab, exitMolePrefab;
    private ScrollViewContent svc;
    [SerializeField] private Color baseColour, activeColour;
    public bool inProgress = false;


    void Start()
    {
        svc = Canvas.FindObjectOfType<ScrollViewContent>();

        add.image.color = baseColour;
        add.GetComponentInChildren<TMPro.TMP_Text>().text = "Add Mole";

        exitMolePrefab.SetActive(false);
        addMolePrefab.SetActive(true);

        add.onClick.AddListener(AddMole);
        exit.onClick.AddListener(Exit);   
    }

    private void Update()
    {
        if (inProgress)
        {
            exitMolePrefab.SetActive(true);
            add.GetComponentInChildren<TMPro.TMP_Text>().text = "Site Mole";
            add.image.color = activeColour;
        }
    }

    private void Exit()
    {
#if !UNITY_EDITOR
        Analytics.CustomEvent("Used Exit button when partway through adding mole");
#endif

        inProgress = false;
        SceneManager.LoadScene("3DModelScene");
    }

    /// <summary>
    /// Allows a user to begin the process of adding a new mole, or place a mole marker on the model,
    /// depending on current scene state indicated by the 'inProgress' bool. 
    /// </summary>
    public void AddMole() 
    {
        if (!inProgress)
        {
#if !UNITY_EDITOR
            Analytics.CustomEvent("Adding mole");
#endif
            exitMolePrefab.SetActive(true);
            svc.MoleListActivation(false);
            svc.DeactivateContextControls();

            int index = svc.FindIndex("Root Context Controls");
            svc.ActivateContext(index);
            add.GetComponentInChildren<TMPro.TMP_Text>().text = "Place Marker";
            add.image.color = activeColour;
            inProgress = true;
        }
        else
        {
            addMolePrefab.SetActive(false);
            svc.DeactivateContextControls();
            int index = svc.FindIndex("Final Zoom Depth");
            svc.ActivateContext(index);
        }
    }
}
