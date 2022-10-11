using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinalZoomButtonHandler : MonoBehaviour
{
    public GameObject addMoleButton;
    public Button goBack, startAgain;
    public PlaceMoleMarker pmm;
    public ScrollViewContent svc;

    void Start()
    {
        goBack.onClick.AddListener(GoBackOne);
        startAgain.onClick.AddListener(StartAgain);
    }

    private void OnEnable()
    {
        pmm.active = true;
    }

    private void StartAgain()
    {
        svc.DeactivateContextControls();
        pmm.active = false;
        addMoleButton.SetActive(true);
        svc.contextPath = new();
        svc.ActivateContext(0);
    }

    private void GoBackOne()
    {
        pmm.active = false;
        addMoleButton.SetActive(true);
        svc.ZoomOut();
    }
}
