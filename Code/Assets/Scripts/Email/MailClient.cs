using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
 
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
        MailMessage mail = new MailMessage();
         
        mail.From = new MailAddress("rhodriowend@gmail.com");
        mail.To.Add(targetEmail.text);
        mail.Subject = subject.text;
        mail.Body = body.text;

        foreach (var path in EmailVariables.moleImagesToSend) {
            mail.Attachments.Add(new Attachment(DeviceVariables.imagesPath + path));
        }
        
         
        SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
        smtpServer.Port = 587;
        smtpServer.Credentials = new System.Net.NetworkCredential("email", "password")as ICredentialsByHost;
        smtpServer.EnableSsl = true;
        ServicePointManager.ServerCertificateValidationCallback = 
            delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) 
        { return true; };
 
        smtpServer.Send(mail);
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