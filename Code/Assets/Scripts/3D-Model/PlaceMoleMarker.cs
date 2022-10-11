using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


/// <summary>
/// <para>
/// Class to handle detection and positioning of a new mole marker on the model.
/// </para>
/// Must be 'active' for raycasting to occur. Generally activated by <see cref="FinalZoomButtonHandler"/>
/// </summary>
public class PlaceMoleMarker : MonoBehaviour
{
    public bool active = false;
    public GameObject parent;
    public ScrollViewContent sv;
    public Camera mc;

    /// <summary>
    /// If active and raycast hits the model, generate a new mole marker at the collision point with some default properties.
    /// These properties will be updated at later stages before saving to the DB. 
    /// </summary>
    void Update()
    {
        if (active) {
            if (Input.touchCount == 1)  {
                Touch screenTouch = Input.GetTouch(0);
                Ray ray = mc.ScreenPointToRay(screenTouch.position);
                RaycastHit hit;
                if(Physics.Raycast(ray, out hit, 100) && (hit.collider.gameObject.name == "male" || hit.collider.gameObject.name == "female")
                    && EventSystem.current.currentSelectedGameObject == null){

                    Vector3 position = new(hit.point.x, hit.point.y, hit.point.z);
                    Transform camTransform = mc.transform;
                    float markerScale = camTransform.position.z < -8 ? 0.2f : camTransform.position.z < -2 ? 0.1f : 0.05f;

                    AddMoleMarker(position, markerScale, 9999, "mole");

                    active = false;
                    PhotoVariables.x = hit.point.x;
                    PhotoVariables.y = hit.point.y;
                    PhotoVariables.z = hit.point.z;
                    CheckPosition();
                }   
            }
        }
    }

    /// <summary>
    /// Adds a new marker on the model as the provided location. 
    /// </summary>
    /// <param name="position">The location of the model in world space</param>
    /// <param name="scale">The scale of the mole marker</param>
    /// <param name="id">The id of the mole</param>
    /// <param name="name">The name of the mole</param>
    public void AddMoleMarker(Vector3 position, float scale, int id, string name)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        MoleProperties moleProperties = sphere.AddComponent<MoleProperties>();
        moleProperties.SetOriginalScale(scale);
        moleProperties.id = id;
        sphere.transform.SetParent(parent.transform, false);
        sphere.transform.position = position;
        sphere.transform.localScale = new Vector3(scale, scale, scale);
        sphere.GetComponent<Renderer>().material.color = Color.red;
        sphere.tag = "Mole";
        sphere.name = name;
    }

    /// <summary>
    /// Activates the context to confirm the user is happy with the positioning of the mole.
    /// </summary>
    private void CheckPosition()
    {
        sv.DeactivateContextControls();
        int index = sv.FindIndex("Confirm Mole Selection");
        sv.ActivateContext(index);
    }
}
