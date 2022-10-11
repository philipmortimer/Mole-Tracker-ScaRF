using UnityEngine;
using UnityEngine.UI;

public class ScaleMoleContextHandler : MonoBehaviour
{
    [SerializeField] private Button accept;
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject deviceCam;
    [SerializeField] private ScrollViewContent svc;
    [SerializeField] private CameraController cc;
    //[SerializeField] private float defaultMoleSize = 0.02f;

    private GameObject mole;

    void Start()
    {
        accept.onClick.AddListener(FinishAddingMole);

        GameObject model = GameObject.Find("HumanBody");

        GameObject moles = model.transform.Find("Moles").gameObject;
        mole = moles.transform.Find("mole").gameObject;

        if (Camera.main.transform.position.z < -5)
        { 
            cc.SetCameraTarget(mole.transform, 5);
            slider.value = 0.05f;
        } else
        {
            slider.value = mole.transform.localScale.x;
        }

        
    }

    private void FinishAddingMole()
    {
        mole.GetComponent<MoleProperties>().SetOriginalScale(slider.value);
        svc.DeactivateContextControls();
        deviceCam.SetActive(true);
        PhotoVariables.scale = slider.value;
        PhotoVariables.nameMole = true;
    }

    void Update()
    {
        mole.transform.localScale = new(slider.value, slider.value, slider.value);
    }
}