using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RightArmContextControls : MonoBehaviour
{
    public Button rightHand, rightElbow, rightShoulder, zoomOut, ready;
    public ScrollViewContent svc;
    public float initialZoom = -5f;
    public float finalZoom = -2f;

    public GameObject model;
    private GameObject modelInstance;
    private Camera cam;
    [SerializeField] private CameraController camControl;
    [SerializeField] private float moleMarkerScale = 0.5f;
    [SerializeField] private float nextMoleMarkerScale = 0.25f;


    void Start()
    {
        rightHand.onClick.AddListener(() => ZoomToLocation("Right Hand"));
        rightElbow.onClick.AddListener(() => ZoomToLocation("Right Elbow"));
        rightShoulder.onClick.AddListener(() => ZoomToLocation("Right Shoulder"));
        zoomOut.onClick.AddListener(svc.ZoomOut);
        ready.onClick.AddListener(() => svc.SwitchContext("Final Zoom Depth"));
    }

    private void OnEnable()
    {
        cam = Camera.main;
        GetActiveModel();
        svc.UpdateMoleMarkerSize(moleMarkerScale);
        cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, initialZoom);
        camControl.SetCameraTarget(modelInstance.transform.Find("Right Arm"), initialZoom);
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
        svc.SwitchContext("Ready To Add Mole");
    }

    private void ZoomToLocation(string loc)
    {
        ActivateReadyToAddMoleContext();
        svc.UpdateMoleMarkerSize(nextMoleMarkerScale);
        camControl.Orientation("Front");
        camControl.SetCameraTarget(modelInstance.transform.Find("Right Arm").Find(loc), finalZoom);
    }
}
