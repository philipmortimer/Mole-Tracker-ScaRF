using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Class to handle camera position and orientation through touch controls and some predetermined
/// positioning. 
/// </summary>
public class CameraController : MonoBehaviour
{
    public Vector3 faceFrontRot, faceLeftRot, faceRightRot, faceBehindRot, faceTopRot, faceBottomRot;
    public Vector3 faceFrontPos, faceLeftPos, faceRightPos, faceBehindPos, faceTopPos, faceBottomPos;
    public float testZdist = 0.5f;

    [SerializeField] private float distanceToTarget, initialCameraDistance = 12f;
    [SerializeField] private float pinchThreshold;
    [SerializeField] private float rotationScalar, zoomScalar, swipeScalar;
    [SerializeField] private Camera cam;
    [SerializeField] private Transform target;
    [SerializeField] private ButtonHandling_3DBody bh;
    [SerializeField] private GameObject moleActionsOptions, content;

    private void Start()
    {
        SetCameraTarget(target, initialCameraDistance);
    }

    void Update()
    {
        if (Input.touchCount == 1 && EventSystem.current.currentSelectedGameObject == null)
        {
            // Enable place marker option
            if (!bh.inProgress && content.activeInHierarchy && !moleActionsOptions.activeInHierarchy)
            {
                bh.AddMole();
            }

            Touch screenTouch = Input.GetTouch(0);

            if (screenTouch.phase == TouchPhase.Moved)
            {
                float rotationAroundYAxis = -screenTouch.deltaPosition.x * rotationScalar * Time.deltaTime; // camera moves horizontally
                float rotationAroundXAxis = screenTouch.deltaPosition.y * rotationScalar * Time.deltaTime; // camera moves vertically

                cam.transform.position = target.position;

                cam.transform.Rotate(new Vector3(1, 0, 0), rotationAroundXAxis);
                cam.transform.Rotate(new Vector3(0, 1, 0), rotationAroundYAxis, Space.World);

                cam.transform.Translate(new Vector3(0, 0, -distanceToTarget));
            }
        }
        
        else if (Input.touchCount == 2 && Input.GetTouch(0).phase == TouchPhase.Moved && Input.GetTouch(1).phase == TouchPhase.Moved
            && EventSystem.current.currentSelectedGameObject == null)
        {
            // Enable place marker option
            if (!bh.inProgress && content.activeInHierarchy && !moleActionsOptions.activeInHierarchy)
            {
                bh.AddMole();
            }

            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            // Check if the fingers are moving away from each other or coming together (ie. a pinch)
            if (Math.Abs(deltaMagnitudeDiff) > pinchThreshold)
            {
                float pinchAmount = -deltaMagnitudeDiff * zoomScalar * Time.deltaTime;
                cam.transform.Translate(new Vector3(0, 0, pinchAmount));
                distanceToTarget = Math.Abs(cam.transform.position.z);
            }

            // Fingers maintain distance between each other: swipe gesture
            else
            {
                float avDeltaY = (touchOne.deltaPosition.y + touchZero.deltaPosition.y) / 2;
                float avDeltaX = (touchOne.deltaPosition.x + touchZero.deltaPosition.x) / 2;
                float swipeAmountY = -(avDeltaY * swipeScalar * Time.deltaTime);
                float swipeAmountX = -(avDeltaX * swipeScalar * Time.deltaTime);
                cam.transform.Translate(new Vector3(swipeAmountX, swipeAmountY, 0));
                target.position = new Vector3(target.position.x + swipeAmountX, target.position.y + swipeAmountY, target.position.z);
            }
        }
    }

    public Transform GetCameraTarget()
    {
        return target;
    }

    /// <summary>
    /// Public function to reposition cameras target point and distance from target.
    /// </summary>
    /// <param name="newTarget">The GameObject to target.</param> 
    /// <param name="distance">The distance from target.</param> 
    public void SetCameraTarget(Transform newTarget, float distance)
    {
        distanceToTarget = Math.Abs(distance);
        if (newTarget == null)
        {
            Debug.Log("ERROR: No target set");
        }
        cam.transform.position = newTarget.position;
        cam.transform.Translate(new Vector3(0, 0, -distanceToTarget));

        target.transform.position = newTarget.transform.position;
    }

    /// <summary>
    /// Used by the context controls to set camera at pre-determined orientations.
    /// </summary>
    /// <param name="direction">String describing the desired orientation one of: Front, Left, Right, Behind, Top, Bottom</param>
    public void Orientation(string direction)
    {
        if (direction == "Front")
        {
            cam.transform.position = faceFrontPos;
            cam.transform.eulerAngles = faceFrontRot;
        }
        else if (direction == "Left")
        {
            cam.transform.position = faceLeftPos;
            cam.transform.eulerAngles = faceLeftRot;
        }
        else if (direction == "Right")
        {
            cam.transform.position = faceRightPos;
            cam.transform.eulerAngles = faceRightRot;
        }
        else if (direction == "Behind")
        {
            cam.transform.position = faceBehindPos;
            cam.transform.eulerAngles = faceBehindRot;
        }
        else if (direction == "Top")
        {
            cam.transform.position = faceTopPos;
            cam.transform.eulerAngles = faceTopRot;
        }
        else if (direction == "Bottom")
        {
            cam.transform.position = faceBottomPos;
            cam.transform.eulerAngles = faceBottomRot;
        }
        else
        {
            Debug.Log("Argument must be one of: Left, Right, Front, Behind, Top, Bottom");
        }
    }
}
