using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using sharpPDF;

/// <summary>
/// Class that handles emails.
/// </summary>
public class MailClient : MonoBehaviour {

    public TMPro.TMP_InputField email, password, targetEmail, subject, body;
    public Button send, cancel;

    void Start () {
        send.onClick.AddListener(SendEmail);
        cancel.onClick.AddListener(CancelEmail);
    }
    
    /// <summary>
    /// Function that retrieves the user input from the email scene, creates an SMTP server and sends the email.
    /// </summary>
    private void SendEmail()
    {
        NativeShare share = new NativeShare().SetSubject(subject.text).SetTitle("Mole Photos Sharing").SetText(body.text).AddEmailRecipient(targetEmail.text);
        foreach (string path in EmailVariables.moleImagesToSend)
        {
            share = share.AddFile(DeviceVariables.imagesPath + path);
        }
        share.Share();

        /*
         * Dummy code for writing pdf with text file
        string ReportPath = DeviceVariables.imagesPath + "MolesReport.pdf"; // Filename for PDF to be saved

        pdfDocument myDoc = new pdfDocument("TUTORIAL", "ME");
        pdfPage myPage = myDoc.addPage(100, 100);
        myPage.addText("Hello World!", 200, 450, myDoc.getFontReference(sharpPDF.Enumerators.predefinedFont.csHelvetica), 10);
        myDoc.createPDF(ReportPath);
        myPage = null;
        myDoc = null;

        */

        Debug.Log("success");
        SceneManager.LoadScene("Diary");
    }

    /// <summary>
    /// Function that brings the user back to the diary scene.
    /// </summary>
    private void CancelEmail()
    {
        SceneManager.LoadScene("Diary");
    }
}