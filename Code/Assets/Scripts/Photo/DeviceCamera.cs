using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeviceCamera : MonoBehaviour
{
	//private WebCamTexture cam;
 //   private WebCamDevice device;
	//private GameObject plane;

	void Start()
	{
        WebCamTexture webcamTexture = new WebCamTexture();
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = webcamTexture;
        webcamTexture.Play();
    }

    //IEnumerator Start()
    //{
    //    yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
    //    device = WebCamTexture.devices[0];
    //    plane = GameObject.FindWithTag("Plane");
    //    cam = new WebCamTexture();
    //    Debug.Log("Camera is", cam);
    //    plane.GetComponent<Renderer>().material.mainTexture = cam;
    //    cam.Play();
    //}
}
