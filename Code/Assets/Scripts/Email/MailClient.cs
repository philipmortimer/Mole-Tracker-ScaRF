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

    void Start () {
        send.onClick.AddListener(SendEmail);
        cancel.onClick.AddListener(CancelEmail);
    }
    
    /// <summary>
    /// Function that retrieves the user input from the email scene, creates an SMTP server and sends the email.
    /// </summary>
    private void SendEmail()
    {
        /*NativeShare share = new NativeShare().SetSubject(subject.text).SetTitle("Mole Photos Sharing").SetText(body.text).AddEmailRecipient(targetEmail.text);
        foreach (string path in EmailVariables.moleImagesToSend)
        {
            share = share.AddFile(DeviceVariables.imagesPath + path);
        }
        share.Share();*/

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
        Debug.Log("PAGE SIZE");
        doc.SetPageSize(PageSize.A4);
        Debug.Log(doc.PageSize);
        doc.Open();
        doc.Add(new Paragraph("Hello pls work"));
        //Image handling
        doc.NewPage();
        doc.Add(getImage(DeviceVariables.imagesPath + EmailVariables.moleImagesToSend[0], 
            doc.PageSize.Width - doc.LeftMargin - doc.RightMargin, doc.PageSize.Height - doc.TopMargin - doc.BottomMargin));
        doc.Close();
        new NativeShare().AddFile(ReportPath).Share();
        //Application.OpenURL(ReportPath);

        Debug.Log("success");
        SceneManager.LoadScene("Diary");
    }

    /// <summary>
    /// Converts the given image to a format where it can be inserted into a pdf document.
    /// If image is dimensions are greater than the maximum dimensions, it is scaled down till it fits the dimensions.
    /// </summary>
    private iTextSharp.text.Image getImage(string imagePath, float maxWidth, float maxHeight)
    {
        // Loads image
        Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        byte[] bytes = File.ReadAllBytes(DeviceVariables.imagesPath + EmailVariables.moleImagesToSend[0]);
        texture.LoadImage(bytes);
        texture.Apply();
        byte[] b = texture.EncodeToPNG();
        iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(b);
        // Scales image
        float h = img.ScaledHeight;
        float w = img.ScaledWidth;
        img.SetAbsolutePosition(0, 0);
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
