using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System;

/// <summary>
/// Class that handles emails.
/// </summary>
public class MailClient : MonoBehaviour {

    public TMPro.TMP_InputField email, password, targetEmail, subject, body;
    public Button send, cancel;

    const string STARS = "******************************************************************************************";

    void Start () {
        send.onClick.AddListener(SendEmail);
        cancel.onClick.AddListener(CancelEmail);
    }
    
    /// <summary>
    /// Function that retrieves the user input from the email scene, creates an SMTP server and sends the email.
    /// </summary>
    private void SendEmail()
    {   // Creates PDF file
        string ReportPath = DeviceVariables.imagesPath + "moles.pdf";
        Document doc = new Document();
        try
        {
            if (File.Exists(ReportPath))
            {
                File.Delete(ReportPath);
            }
            PdfWriter.GetInstance(doc, new FileStream(ReportPath, FileMode.Create));
        }
        catch (System.Exception e)
        {
            Debug.Log("Error creating pdf " + e);
        }
        doc.SetPageSize(PageSize.A4);
        doc.Open();
        setPdfText(doc);
        // Adds all images
        foreach (string path in EmailVariables.moleImagesToSend)
        {
            doc.Add(getImage(DeviceVariables.imagesPath + path,
            doc.PageSize.Width - doc.LeftMargin - doc.RightMargin, doc.PageSize.Height - doc.TopMargin - doc.BottomMargin));
        }
        addQuestionnaireResults(doc);
        doc.Close();
        new NativeShare().AddFile(ReportPath).Share(); // Shares file

        Debug.Log("success");
        SceneManager.LoadScene("Diary");
    }

    /// <summary>
    /// Adds questionaire results to document if included.
    /// </summary>
    private void addQuestionnaireResults(Document doc)
    {
        if (EmailVariables.questionnaire.Count > 0)
        {
            string text = "Included below is the results of the SCQOLIT survery, a survey designed to measure wellbeing of patients.\n" + STARS + "\n";
            foreach ((string q, string a) in EmailVariables.questionnaire)
            {
                text += q + "  " + a + "\n";
            }
            doc.Add(new Paragraph(text));
        }
    }

    /// <summary>
    /// Sets the text contents of the report before the images.
    /// </summary>
    private void setPdfText(Document doc)
    {
        string text = STARS + "\nTHIS DOCUMENT CONTAINS SENSITIVE MEDICAL DATA. IF YOU ARE NOT A MEDICAL PROFESSIONAL OR BELIEVE THAT THIS" +
            " DOCUMENT ISN'T INTENDED FOR YOU, PLEASE DELETE THIS CORRESPONDENCE AND INFORM THE SENDER OF THEIR MISTAKE. ALL CONTENTS" +
            " MUST BE TREATED AS CONFIDENTIAL. THE INTENDED RECIPIENT IS " + targetEmail.text + "\n" + STARS;
        doc.Add(new Paragraph(text));
        text = "This document is partially autogenerated by an app developed by the Skin Cancer Research Fund https://www.skincancerresearch.org/ .\n" +
            "Patient Email Address: " + email.text + "\nSubject: " + subject.text +"\nPatient Comments: " + body.text;
        doc.Add(new Paragraph(text));
        text = STARS + "\nBelow are the images taken by the user of their mole. Please review them and respond to the patient.\n" + STARS + "\n";
        doc.Add(new Paragraph(text));
    }

    /// <summary>
    /// Converts the given image to a format where it can be inserted into a pdf document.
    /// If image is dimensions are greater than the maximum dimensions, it is scaled down till it fits the dimensions.
    /// </summary>
    private iTextSharp.text.Image getImage(string imagePath, float maxWidth, float maxHeight)
    {
        // Loads image
        Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        byte[] bytes = File.ReadAllBytes(imagePath);
        texture.LoadImage(bytes);
        texture.Apply();
        byte[] b = texture.EncodeToPNG();
        iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(b);
        // Scales image
        float h = img.ScaledHeight;
        float w = img.ScaledWidth;
        if (h > maxHeight || w > maxWidth) {
            float widthScale = (maxWidth / w);
            float heightScale = (maxHeight / h);
            float scale = Math.Min(widthScale, heightScale);
            img.ScaleAbsolute(w * scale, h * scale);
        }
        return img;
    }

    /// <summary>
    /// Function that brings the user back to the diary scene.
    /// </summary>
    private void CancelEmail()
    {
        SceneManager.LoadScene("Diary");
    }
}
