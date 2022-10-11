using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mono.Data.Sqlite;
using System.Data;
using TMPro;

/// <summary>
/// Class to handle the behviour of the MoleListButton prefab.
/// </summary>
public class MoleListButton : MonoBehaviour
{
    public Button moleButton;
    public RawImage image;

    [SerializeField] GameObject buttonList;
    [SerializeField] TextMeshProUGUI moleName;
    [SerializeField] TextMeshProUGUI date;
    [SerializeField] Color selectedColor;
    [SerializeField] Color unselectedColor;

    private string im_path;
    private string moleId;
    private bool selected;

    void Awake () {
        selected = false;
    }

    void Start () {
        moleButton.onClick.AddListener(TaskOnClick);
    }

    public void SetId (string text) {
        moleId = text;
    }

    public void SetSelectedColor () {
        moleButton.GetComponent<Image>().color = selectedColor;
    }

    public void SetUnselectedColor () {
        moleButton.GetComponent<Image>().color = unselectedColor;
    }

    public void SetName (string text) {

        moleName.text = text;
    }

    public string GetName () {
        return moleName.text;
    }

    public void SetDate (string text) {
        date.text = text;
    }

    public string GetDate () {
        return date.text;
    }

    public void SetImage (string im_path) {
        this.im_path = im_path;

        Texture2D imageTexture = new Texture2D(1, 1);

        byte[] bytes = File.ReadAllBytes(DeviceVariables.imagesPath + im_path);

        imageTexture.LoadImage(bytes);
        imageTexture.Apply();

        image.GetComponent<RawImage>().texture = imageTexture;
    }

    public string GetPath () {
        return im_path;
    }
    
    public void Select () {
        selected = !selected;
    }

    public bool Selected () {
        return selected;
    }

    private void TaskOnClick() {
        Scene scene = SceneManager.GetActiveScene();

        switch(scene.name) {
            case "Diary":
                DBId.moleId = moleId;
                SceneManager.LoadScene("Mole Information");
                break;
            case "Mole Information":
                Select();
                var lc = buttonList.GetComponent<MoleInformationListController>();
                lc.ButtonPressed();
                break;
            default:
                break;
        }
    } 
}
