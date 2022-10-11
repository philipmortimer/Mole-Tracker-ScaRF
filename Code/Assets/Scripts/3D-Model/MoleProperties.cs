using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to handle properties of each mole. Instantiated on each mole when loaded from DB. 
/// </summary>
public class MoleProperties : MonoBehaviour
{
    private float originalScale;
    public int id;

    public void SetOriginalScale(float scale)
    {
        originalScale = scale;
    }

    public float GetOriginalScale()
    {
        return originalScale;
    }
}
