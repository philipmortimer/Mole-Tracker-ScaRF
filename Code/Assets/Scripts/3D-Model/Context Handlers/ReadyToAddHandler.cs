using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadyToAddHandler : MonoBehaviour
{
    public Button ready, zoomOut;
    public ScrollViewContent svc;

    void Start()
    {
        ready.onClick.AddListener(Ready);
        zoomOut.onClick.AddListener(svc.ZoomOut);
    }

    private void Ready()
    {
        svc.DeactivateContextControls();
        int index = svc.FindIndex("Final Zoom Depth");
        svc.ActivateContext(index);
    }
}
