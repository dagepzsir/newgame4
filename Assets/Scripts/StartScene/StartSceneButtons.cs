using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StartSceneButtons : MonoBehaviour {

    public Canvas FolderDialogCanvas;
    public Text OpenSaveButtonText;
    void Start () {
        FolderDialogCanvas.enabled = false;
	}
	
	public void ButtonScript(string buttonText)
    {
        OpenSaveButtonText.text = buttonText;
        FolderDialogCanvas.enabled = true;
    }
}
