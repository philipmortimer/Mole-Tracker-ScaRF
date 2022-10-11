using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;

public class ConfirmMolePosButtonHandler : MonoBehaviour
{
    public Button confirmMolePos, discardMolePos;
    public GameObject add;
    public ScrollViewContent svc;
    //public ButtonHandling_3DBody bh;
    public PlaceMoleMarker pmm;

    private CameraController camController;

    void Start()
    {
        camController = GameObject.Find("Cam Controller").GetComponent<CameraController>();

        confirmMolePos.onClick.AddListener(ConfirmPosition);
        discardMolePos.onClick.AddListener(DiscardPosition);
        add.SetActive(false);
    }

    private void DiscardPosition()
    {

#if !UNITY_EDITOR
        Analytics.CustomEvent("Not happy with mole placement");
#endif
        pmm.active = false;
        // Delete mole sphere which had just been added. 
        GameObject moles = GameObject.Find("HumanBody").transform.Find("Moles").gameObject;
        GameObject lastMole = moles.transform.GetChild(moles.transform.childCount - 1).gameObject;
        GameObject.Destroy(lastMole);

        // Return context to ready to add mole
        svc.DeactivateContextControls();
        svc.RemoveLastContextPathElement();
        svc.RemoveLastContextPathElement();

        int index = svc.FindIndex("Ready To Add Mole");
        svc.contextControlsTree[index].SetActive(true);
        add.SetActive(true);
    }

    private void ConfirmPosition()
    {
#if !UNITY_EDITOR
        Analytics.CustomEvent("Happy with mole placement");
#endif
        // Save camera settings to be saved to DB. 
        StoreCameraSettings();

        svc.DeactivateContextControls();
        int index = svc.FindIndex("Scale Mole");
        svc.ActivateContext(index);
    }

    private void StoreCameraSettings()
    {
        Transform camTransform = Camera.main.transform;
        PhotoVariables.camX = camTransform.position.x;
        PhotoVariables.camY = camTransform.transform.position.y;
        PhotoVariables.camZ = camTransform.transform.position.z;
        PhotoVariables.camRotX = camTransform.eulerAngles.x;
        PhotoVariables.camRotY = camTransform.eulerAngles.y;
        PhotoVariables.camRotZ = camTransform.eulerAngles.z;
        PhotoVariables.camFOV = Camera.main.fieldOfView;

        Transform target = camController.GetCameraTarget();
        PhotoVariables.targetX = target.transform.position.x;
        PhotoVariables.targetY = target.transform.position.y;
        PhotoVariables.targetZ = target.transform.position.z;
    }
}
