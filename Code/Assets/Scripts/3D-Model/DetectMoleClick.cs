using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Class to handle detection of click on existing mole.
/// </summary>
public class DetectMoleClick : MonoBehaviour
{

    Vector3 touchPosWorld;
 
     //Change me to change the touch phase used.
     TouchPhase touchPhase = TouchPhase.Ended;

    void Update()
    {
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == touchPhase) {
         Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
         RaycastHit hit;
         Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow, 100f);
         if(Physics.Raycast(ray, out hit))
         {
             if (hit.transform.tag == "Mole") {
                 
                 HighlightMole.selectedMole = hit.transform.GetComponent<MoleProperties>().id;
             }
         }
  }
    }
}
