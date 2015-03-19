using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;
using Assets.Scripts;


public class OpenDialogPanelScript : MonoBehaviour {

	public InputField folder;
	public GameObject canvas;
	public GameObject folderButton;
	public InputField newFolderName;
	void Start()
	{
		UpdateFolders(folder.text);
	}

	public void UpdateFolders(string path)
	{
		string[] dirs = Directory.GetDirectories(path);
		int y = 0;
		int x = 0;
		for(int i = 0; i < dirs.Length; i++)
		{
			GameObject go = folderButton;
			string[] splitted = dirs[i].Split('/');

			x++;
			if(i % 6 == 0)
			{
				y++;
				x = 0;
			}
			go.GetComponent<RectTransform>().position = new Vector3(-267 + x * 104, 140 - y * 35);
			go.transform.GetChild(0).gameObject.GetComponent<Text>().text = splitted[splitted.Length - 1];

			Instantiate(go);
		}
		GameObject[] objects = GameObject.FindGameObjectsWithTag("FolderButton");

		foreach(GameObject obj in objects)
		{
			obj.transform.SetParent(canvas.transform, false);
			string asd = obj.transform.GetChild(0).gameObject.GetComponent<Text>().text;
			obj.GetComponent<Button>().onClick.AddListener(delegate {buttonClick(asd);});
		}
	}

	public void buttonClick(string text)
	{
		string path = folder.text + text + "/";
		folder.text = path;

		foreach(GameObject obj in GameObject.FindGameObjectsWithTag("FolderButton"))
			Destroy(obj);

		UpdateFolders(path);
	}

	public void UpButton()
	{
		string[] splitted = folder.text.Split('/');
		string path = "";
		foreach(string str in splitted)
			Debug.Log(str);
		for(int i = 0; i < splitted.Length - 2; i++)
			path += splitted[i] + "/";
		Debug.Log(path);
		foreach(GameObject obj in GameObject.FindGameObjectsWithTag("FolderButton"))
		{
			Destroy(obj);

		}
		folder.text = path;
		UpdateFolders(path);
	}
	public int Size;

	public void NewFolder(Canvas newFolderCanvas)
	{
		newFolderCanvas.enabled = true;
		newFolderName.Select();
	}
	public void CreateNewFolder(InputField foldername)
	{
		Directory.CreateDirectory(folder.text + foldername.text + "/");
		foldername.transform.parent.parent.GetComponent<Canvas>().enabled = false;
		folder.text = folder.text + foldername.text + "/";
		foreach(GameObject obj in GameObject.FindGameObjectsWithTag("FolderButton"))
		{
			Destroy(obj);
		}
		UpdateFolders(folder.text);
	}
	public void OpenSaveButton(Text text)
	{
		switch(text.text)
		{
            case "New":
                PlayerPrefs.SetString(PlayerPrefEnum.MainFilePath.ToString(), folder.text);
                Application.LoadLevel("MainScene");
                break;
			case "Save":
			    break;
	    	case "Open":
                PlayerPrefs.SetString(PlayerPrefEnum.MainFilePath.ToString(), folder.text);
                Application.LoadLevel("MainScene");
			    break;

            default:
                Debug.LogError("Misconfigured folderdialog, check text element.");
                break;
		}
	}

}
