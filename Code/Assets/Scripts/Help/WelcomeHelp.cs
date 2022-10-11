using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Class to handle the behaviour of the Welcome tutorial presented to new users.
/// </summary>
/// TODO: Create a way for users to skip forwards through the panels/exit tutorial. 
public class WelcomeHelp : MonoBehaviour
{

    int waitTime = 4;
    int promptTime = 1;

    public GameObject Welcome, Goodbye;
    // Objects for the account button help
    public GameObject accountButton, accountHelp;
    // Objects for the home button help
    public GameObject homeButton, homeHelp;
    public GameObject sceneName, sceneHelp;
    public GameObject navBar, navHelp;
    public GameObject navHome, navHomeHelp;
    public GameObject navInfo, navInfoHelp;
    public GameObject navMoles, navMolesHelp; 
    public GameObject navDiary, navDiaryHelp;
    public GameObject helpButton, helpHelp;
    public GameObject helpScreen, helpScreenHelp;
    public GameObject helpScreenNext, helpScreenNextHelp;
    public GameObject helpScreenPrevious, helpScreenPreviousHelp;
    public GameObject helpScreenExit, helpScreenExitHelp;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PageHelp());
        
    }

    IEnumerator PageHelp()
    {
        
        Welcome.SetActive(true);
        yield return new WaitForSeconds(waitTime);
        Welcome.SetActive(false);
        yield return new WaitForSeconds(promptTime);

        // Activating and deactivating the account button help
        accountButton.SetActive(true);
        yield return new WaitForSeconds(promptTime);
        accountHelp.SetActive(true);
        yield return new WaitForSeconds(waitTime);
        accountHelp.SetActive(false);
        yield return new WaitForSeconds(promptTime);


        // Activating and deactivating the home button help
        homeButton.SetActive(true);
        yield return new WaitForSeconds(promptTime);
        homeHelp.SetActive(true);
        yield return new WaitForSeconds(waitTime);
        homeHelp.SetActive(false);
        yield return new WaitForSeconds(promptTime);

        // Activating and deactivating the scene text help
        sceneName.SetActive(true);
        yield return new WaitForSeconds(promptTime);
        sceneHelp.SetActive(true);
        yield return new WaitForSeconds(waitTime);
        sceneHelp.SetActive(false);
        yield return new WaitForSeconds(promptTime);

        // Activating and deactivating the navbar help
        navBar.SetActive(true);
        yield return new WaitForSeconds(promptTime);
        navHelp.SetActive(true);
        yield return new WaitForSeconds(waitTime);
        navHelp.SetActive(false);
        yield return new WaitForSeconds(promptTime);

        // Activating and deactivating the navbar home help
        navHome.SetActive(true);
        yield return new WaitForSeconds(promptTime);
        navHomeHelp.SetActive(true);
        yield return new WaitForSeconds(waitTime);
        navHomeHelp.SetActive(false);
        yield return new WaitForSeconds(promptTime);

        // Activating and deactivating the navbar info help
        navInfo.SetActive(true);
        yield return new WaitForSeconds(promptTime);
        navInfoHelp.SetActive(true);
        yield return new WaitForSeconds(waitTime);
        navInfoHelp.SetActive(false);
        yield return new WaitForSeconds(promptTime);

        // Activating and deactivating the navbar moles help
        navMoles.SetActive(true);
        yield return new WaitForSeconds(promptTime);
        navMolesHelp.SetActive(true);
        yield return new WaitForSeconds(waitTime);
        navMolesHelp.SetActive(false);
        yield return new WaitForSeconds(promptTime);

        // Activating and deactivating the navbar diary help
        navDiary.SetActive(true);
        yield return new WaitForSeconds(promptTime);
        navDiaryHelp.SetActive(true);
        yield return new WaitForSeconds(waitTime);
        navDiaryHelp.SetActive(false);
        yield return new WaitForSeconds(promptTime);

        // Activating and deactivating the help button
        helpButton.SetActive(true);
        yield return new WaitForSeconds(promptTime);
        helpHelp.SetActive(true);
        yield return new WaitForSeconds(waitTime);
        helpHelp.SetActive(false);
        yield return new WaitForSeconds(promptTime);

        // Activating and deactivating the help screen 
        helpScreen.SetActive(true);
        yield return new WaitForSeconds(promptTime);
        helpScreenHelp.SetActive(true);
        yield return new WaitForSeconds(waitTime);
        helpScreenHelp.SetActive(false);
        yield return new WaitForSeconds(promptTime);

        // Activating and deactivating the help screen next button
        helpScreenNext.SetActive(true);
        yield return new WaitForSeconds(promptTime);
        helpScreenNextHelp.SetActive(true);
        yield return new WaitForSeconds(waitTime);
        helpScreenNextHelp.SetActive(false);
        yield return new WaitForSeconds(promptTime);

        // Activating and deactivating the help screen previous button
        helpScreenPrevious.SetActive(true);
        yield return new WaitForSeconds(promptTime);
        helpScreenPreviousHelp.SetActive(true);
        yield return new WaitForSeconds(waitTime);
        helpScreenPreviousHelp.SetActive(false);
        yield return new WaitForSeconds(promptTime);

        // Activating and deactivating the help button
        helpScreenExit.SetActive(true);
        yield return new WaitForSeconds(promptTime);
        helpScreenExitHelp.SetActive(true);
        yield return new WaitForSeconds(waitTime);
        helpScreenExitHelp.SetActive(false);
        yield return new WaitForSeconds(promptTime);

        DeactivateEverything();
        yield return new WaitForSeconds(promptTime);

        Goodbye.SetActive(true);
        yield return new WaitForSeconds(waitTime);
        Goodbye.SetActive(true);
        yield return new WaitForSeconds(promptTime);

        SceneManager.LoadScene("New User");


    }

    void DeactivateEverything()
    {
        accountButton.SetActive(false);
        homeButton.SetActive(false);
        sceneName.SetActive(false);
        navBar.SetActive(false);
        navHome.SetActive(false);
        navInfo.SetActive(false);
        navMoles.SetActive(false);
        navDiary.SetActive(false);
        helpButton.SetActive(false);
        helpScreen.SetActive(false);
        helpScreenNext.SetActive(false);
        helpScreenPrevious.SetActive(false);
        helpScreenExit.SetActive(false);
    }
}
