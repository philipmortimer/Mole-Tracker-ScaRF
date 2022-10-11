using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class to handle flashing of help icon on timed basis.
/// Parameters set in the editor. 
/// </summary>
public class HelpIconTimer : MonoBehaviour
{
    public Sprite normal, highlighted;
    public Button helpButton;
    public int timeToFlash, numFlashes, totalReminders;
    public float flashSpeed;

    private int reminderCount;


    void Start()
    {
        reminderCount = 0;
        helpButton.GetComponent<Image>().sprite = normal;
        StartCoroutine(FlashHelpLogo(timeToFlash));
    }

    IEnumerator FlashHelpLogo(int delay)
    {
        yield return new WaitForSeconds(delay);
        for (int i = 0; i < numFlashes; i++)
        {
            helpButton.GetComponent<Image>().sprite = highlighted;
            yield return new WaitForSeconds(flashSpeed);
            helpButton.GetComponent<Image>().sprite = normal;
            yield return new WaitForSeconds(flashSpeed);
        }

        reminderCount++;
        if (reminderCount < totalReminders)
        {
            StartCoroutine(FlashHelpLogo(timeToFlash));
        }

    }
}
