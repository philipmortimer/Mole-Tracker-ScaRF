using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;

// This class is most likely redundant but I'm not entirely sure.
public class SendMole : MonoBehaviour
{
    void Start()
    {
        // GameObject shadow = this.transform.Find("Shadow").gameObject;
        // GameObject infoBar = shadow.transform.Find("InfoBar").gameObject;
        // GameObject moleImage = shadow.transform.Find("MoleImage").gameObject;
        // Button btn = infoBar.transform.Find("btn").gameObject;
        this.GetComponent<Button>().onClick.AddListener(Test);
    }

    void Test()
    {
        Application.OpenURL("mailto:");
    }
}
