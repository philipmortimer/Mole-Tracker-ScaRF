using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RootCCButtonHandler : MonoBehaviour
{
    public Button head, torso, rightArm, leftArm, rightLeg, leftLeg;
    public ScrollViewContent svc;
    public GameObject model;
    public float initialZoom = -12f;

    private GameObject modelInstance;
    private Camera cam;
    [SerializeField] private CameraController camControl;
    [SerializeField] private float moleMarkerScale = 1f;
    

    void Start()
    {
        head.onClick.AddListener(() =>  svc.SwitchContext("Head Context Controls"));
        torso.onClick.AddListener(() => svc.SwitchContext("Torso Context Controls"));
        rightArm.onClick.AddListener(() => svc.SwitchContext("Right Arm Context Controls"));
        leftArm.onClick.AddListener(() => svc.SwitchContext("Left Arm Context Controls"));
        rightLeg.onClick.AddListener(() => svc.SwitchContext("Right Leg Context Controls"));
        leftLeg.onClick.AddListener(() => svc.SwitchContext("Left Leg Context Controls"));
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

    private void OnEnable()
    {
        GetActiveModel();
        svc.UpdateMoleMarkerSize(moleMarkerScale);
        cam = Camera.main;
        camControl.Orientation("Front");
        cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, initialZoom);
        camControl.SetCameraTarget(modelInstance.transform.Find("Root"), initialZoom);

        svc.contextPath = new();
        svc.contextPath.Add(0);
    }
}
