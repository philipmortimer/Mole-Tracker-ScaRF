using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TorsoContextControls : MonoBehaviour
{
    public Button buttocks, zoomOut, front, back, groin, ready;
    public ScrollViewContent svc;
    public float initialZoom = -5f;
    public float finalZoom = -2f;

    private Camera cam;
    [SerializeField] private CameraController camControl;
    [SerializeField] private float moleMarkerScale = 0.5f;
    //[SerializeField] private float nextMoleMarkerScale = 0.25f;

    public GameObject model;
    private GameObject modelInstance;

    // Start is called before the first frame update
    void Start()
    {
        //zoomIn.onClick.AddListener(ZoomIn);
        zoomOut.onClick.AddListener(() => svc.ZoomOut());
        front.onClick.AddListener(OrientFront);
        back.onClick.AddListener(OrientBack);
        groin.onClick.AddListener(ZoomToGroin);
        buttocks.onClick.AddListener(ZoomToButtocks);
        ready.onClick.AddListener(() => svc.SwitchContext("Final Zoom Depth"));
    }

    private void ZoomToButtocks()
    {
        camControl.Orientation("Behind");
        camControl.SetCameraTarget(modelInstance.transform.Find("Groin"), initialZoom);
    }

    private void ZoomToGroin()
    {
        camControl.Orientation("Front");
        camControl.SetCameraTarget(modelInstance.transform.Find("Groin"), initialZoom);
    }

    private void OrientBack()
    {
        camControl.Orientation("Behind");
        camControl.SetCameraTarget(modelInstance.transform.Find("Torso"), initialZoom);
    }

    private void OrientFront()
    {
        camControl.Orientation("Front");
        camControl.SetCameraTarget(modelInstance.transform.Find("Torso"), initialZoom);
    }

    private void OnEnable()
    {
        cam = Camera.main;
        GetActiveModel();
        cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, initialZoom);
        camControl.SetCameraTarget(modelInstance.transform.Find("Torso"), initialZoom);
        svc.UpdateMoleMarkerSize(moleMarkerScale);
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
}
