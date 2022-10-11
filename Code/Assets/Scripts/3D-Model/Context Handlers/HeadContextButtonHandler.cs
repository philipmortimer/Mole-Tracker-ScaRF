using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeadContextButtonHandler : MonoBehaviour
{
    public Button leftEye, rightEye, nose, leftEar, rightEar, zoomOut;
    public ScrollViewContent svc;
    public float initialZoom = -3f;
    public float finalZoom = -1f;

    public GameObject model;
    private GameObject modelInstance;
    private Camera cam;
    [SerializeField] private CameraController camControl;
    [SerializeField] private float moleMarkerScale = 0.5f;
    [SerializeField] private float nextMoleMarkerScale = 0.25f;

    void Start()
    {
        leftEye.onClick.AddListener(() => ZoomToLocation("Left Eye", "Front"));
        rightEye.onClick.AddListener(() => ZoomToLocation("Right Eye", "Front"));
        leftEar.onClick.AddListener(() => ZoomToLocation("Left Ear", "Right"));
        rightEar.onClick.AddListener(() => ZoomToLocation("Right Ear", "Left"));
        nose.onClick.AddListener(() => ZoomToLocation("Nose", "Front"));
        zoomOut.onClick.AddListener(svc.ZoomOut);
    }

    private void OnEnable()
    {
        cam = Camera.main;
        GetActiveModel();
        svc.UpdateMoleMarkerSize(moleMarkerScale);
        cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, initialZoom);
        camControl.SetCameraTarget(modelInstance.transform.Find("Head"), initialZoom);
    }


    private void GetActiveModel()
    {
        for (int i = 0; i < model.transform.childCount; i++)
        {
            GameObject child = model.transform.GetChild(i).gameObject;
            if (child.activeSelf == true && child.tag == "3D Model")
            {
                modelInstance = model.transform.GetChild(i).gameObject;
            }
        }
    }
    private void ActivateReadyToAddMoleContext()
    {
        svc.DeactivateContextControls();
        int index = svc.FindIndex("Ready To Add Mole");
        svc.ActivateContext(index);
        svc.UpdateMoleMarkerSize(nextMoleMarkerScale);
    }

    private void ZoomToLocation(string loc, string orientation)
    {
        ActivateReadyToAddMoleContext();
        camControl.Orientation(orientation);
        camControl.SetCameraTarget(modelInstance.transform.Find("Head").Find(loc), finalZoom);
    }
}
