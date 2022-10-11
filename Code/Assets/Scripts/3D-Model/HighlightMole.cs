using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Class to maintain information about the currently selected mole. 
/// </summary>
public class HighlightMole : MonoBehaviour
{
    public static int selectedMole;
    private static int selectedMoleCheck;
    public Material defaultMole;
    public Material highlightedMole;
    public GameObject content;
    public Color defaultColor, selectedColor;

    [System.Obsolete]
    void Update()
    {
        if (selectedMole != selectedMoleCheck)
        {
            int children = gameObject.transform.childCount;
            for (int i = 0; i < children; ++i)
            {
                if (gameObject.transform.GetChild(i).GetComponent<MoleProperties>().id == selectedMole)
                {
                    gameObject.transform.GetChild(i).GetComponent<Renderer>().material = highlightedMole;
                }
                else {
                    gameObject.transform.GetChild(i).GetComponent<Renderer>().material = defaultMole;
                }
            }
            selectedMoleCheck = selectedMole;
        }
    }
}
