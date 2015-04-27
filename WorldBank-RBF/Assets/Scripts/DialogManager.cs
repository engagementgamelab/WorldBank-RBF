/* 
World Bank RBF
Created by Engagement Lab, 2015
==============
 DialogueManager.cs
 Unity NPC dialog handler.

 Created by Johnny Richardson on 4/14/15.
==============
*/
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Threading;

public class DialogManager : MonoBehaviour {

	private static DialogManager _instance;

	// Singleton Manager
	public static DialogManager instance {

		get {

			if(_instance == null) {
				_instance = GameObject.FindObjectOfType<DialogManager>();

				// Do not destroy on new scene
				DontDestroyOnLoad(_instance.gameObject);
			}

			return _instance;

		}

	}

	// Variable Definitions
	public Button btnPrefab;

	private StringBuilder builder = new StringBuilder();

	private Text currentDialogLabel;
	private int currentDialogIndex;
	private double[] currentDialogueOpacity;
	private List<string> currentDialogueText;
	private List<string> currentDialogueChoices;

	void Awake() {

		if(_instance == null) {
			// This is the singleton
			_instance = this;
			DontDestroyOnLoad(this);
		}
		else {
			// Ensure there is only one reference
			if(this != _instance)
				Destroy(this.gameObject);
		}

	}

	// TODO: Will be used for fading in text
/*	void Update() {

		if(currentDialogueText != null)
		{

			string[] arrTxt = new string[currentDialogueText.Count];
		
			for(int i = 0; i < currentDialogIndex-1; i++)
			{
				// if(currentDialogueOpacity[i] == null)
				// 	currentDialogueOpacity[i] = 00;
				// else
					currentDialogueOpacity[i]++;

				if(currentDialogueText[i].IndexOf("<color") == -1 && currentDialogueOpacity[i] < 100)
					arrTxt[i] = "<color=#ffffff" + currentDialogueOpacity[i] + ">" + currentDialogueText[i] + "</color>";
				else
					arrTxt[i] = currentDialogueText[i];
			}

			// builder.Append();
			currentDialogLabel.text = string.Join("", arrTxt);

			currentDialogIndex++;

		}

	}*/

	/// <summary>
	/// Generate a generic dialog with text
	/// </summary>
	/// <param name="strDialogTxt">Text to show in the dialogue</param>
	public GameObject CreateGenericDialog(string strDialogTxt) {

		GameObject canvasObject = Instantiate(Resources.Load("Prefabs/DialogueBox", typeof(GameObject))) as GameObject;
	    
		Transform diagParent = canvasObject.transform;

	    Text diagText = diagParent.Find("Panel/Dialogue Text").GetComponent<Text>() as Text;
	    diagText.text = strDialogTxt;

	    return canvasObject;

	}

	/// <summary>
	/// Generate an NPC dialog with text
	/// </summary>
	/// <param name="strDialogTxt">Text to show in the dialogue</param>
	/// <param name="showBackBtn">Whether or not to show the back button</param>
	public NPCDialogBox CreateNPCDialog(string strDialogTxt, bool showBackBtn = true) {

		/*GameObject canvasObject = Instantiate(Resources.Load("Prefabs/DialogueNPC", typeof(GameObject))) as GameObject;

	    Text diagText = canvasObject.transform.Find("Layout/Speech/Text").GetComponent<Text>() as Text;
	    diagText.text = strDialogTxt;
		
		canvasObject.transform.Find("Layout/Back Button").gameObject.SetActive(showBackBtn);

	    return canvasObject;*/

	    NPCDialogBox dialog = Instantiate (Resources.Load ("Prefabs/NPCDialogBox", typeof (NPCDialogBox))) as NPCDialogBox;
	    dialog.Content = strDialogTxt;
	    dialog.backButton.gameObject.SetActive (showBackBtn);
	    return dialog;
	}

/*	/// <summary>
	/// Generate an NPC
	/// </summary>
	/// <param name="npcData">Instance of Models.NPC for this NPC</param>
	/// <param name="index">Index of this NPC</param>
	private void GenerateNPC(Models.NPC npcData, int index) {

		// Create NPC prefab instance
		NPCBehavior currentNpc = (NPCBehavior)Instantiate(npcPrefab);

	    // Initialize this NPC
	    // currentNpc.Initialize(npcData);

	}*/

	/// <summary>
	/// Open specified dialog for a given character
	/// </summary>
	/// <param name="currNpc">Instance of Models.NPC for this NPC</param>
	/// <param name="strDialogueKey">The key corresponding to the dialogue to show</param>
	/// <param name="returning">Specify whether player is returning to previous dialog</param>
	public void OpenCharacterDialog(Models.NPC currNpc, string strDialogueKey, bool returning=false) {

		string strDialogTxt = currNpc.dialogue[strDialogueKey]["text"];
		
		// Match any characters in between [[ and ]]
		string strKeywordRegex = "(\\[)(\\[)(.*?)(\\])(\\])";

		// Does this dialogue unlock something?
		if(currNpc.dialogue[strDialogueKey].ContainsKey("unlocks"))
			strDialogTxt += "\n\n<color=yellow>Unlocks</color>: " + currNpc.dialogue[strDialogueKey]["unlocks"];

		string strToDisplay = strDialogTxt.Replace("[[", "<color=orange>").Replace("]]", "</color>");

		// GameObject diagRenderer = CreateNPCDialog(strToDisplay);
		NPCDialogBox dialog = CreateNPCDialog (strToDisplay);

		// currentDialogLabel = diagRenderer.transform.Find("Panel/Dialogue Text").GetComponent<Text>() as Text;
		//-Transform dialogueBtnPanel = diagRenderer.transform.Find("Layout/Choices");
		Transform choiceGroup = dialog.choiceGroup;
		foreach (Transform child in choiceGroup) {
			GameObject.Destroy (child.gameObject);
			// ObjectPool.Destroy<NPCDialogButton> (child);
		}
		// Button dialogueBackBtn = diagRenderer.transform.Find("Layout/Back Button").GetComponent<Button>();
		Button backButton = dialog.backButton;

		/*foreach (Transform child in dialogueBtnPanel.transform)
		    GameObject.Destroy(child.gameObject);*/

		currentDialogueText = new List<string>();
		
		foreach(char c in strDialogTxt)
			currentDialogueText.Add(c.ToString());

		currentDialogueOpacity = new double[currentDialogueText.Count];

		// Search for "keywords" in between [[ and ]]
		Regex regexKeywords = new Regex(strKeywordRegex, RegexOptions.IgnoreCase);
		MatchCollection keyMatches = regexKeywords.Matches(strDialogTxt);
	
		if(strDialogueKey == "Initial" && !returning)
		{
			currentDialogueChoices = new List<string>();

			foreach(Match m in keyMatches) {
			    if (m.Success)
				{
			        string strKeyword = m.Groups[3].ToString();

			        currentDialogueChoices.Add(strKeyword.ToLower());				
				}
			}
		}

		foreach(string choice in currentDialogueChoices) {
			CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
			TextInfo textInfo = cultureInfo.TextInfo;

			string choiceName = textInfo.ToTitleCase(choice);

			Button btnChoice = (Button)Instantiate(btnPrefab);
			// Button btnChoice = ObjectPool.Instantiate<NPCDialogButton> ().button;

			//btnChoice.transform.SetParent (dialogueBtnPanel);
			btnChoice.transform.SetParent (choiceGroup);
			btnChoice.transform.localScale = new Vector3(1, 1, 1);
			btnChoice.transform.localPosition = Vector3.zero;
			
			Text btnTxt = btnChoice.transform.FindChild("Text").GetComponent<Text>();
			btnTxt.text = choiceName;
			
			//btnChoice.onClick.AddListener(() => Destroy(diagRenderer));
			btnChoice.onClick.AddListener(() => Destroy(dialog.gameObject));
			btnChoice.onClick.AddListener(() => currentDialogueChoices.Remove(choice));
			btnChoice.onClick.AddListener(() => OpenCharacterDialog(currNpc, choiceName));
		}

		// Setup back button
		// dialogueBackBtn.onClick.AddListener(() => Destroy(diagRenderer));
		// dialogueBackBtn.onClick.AddListener(() => Destroy(dialog));
		backButton.onClick.AddListener(() => Destroy(dialog.gameObject));
		if(strDialogueKey != "Initial")
			backButton.onClick.AddListener(() => OpenCharacterDialog(currNpc, "Initial", true));
			// dialogueBackBtn.onClick.AddListener(() => OpenCharacterDialog(currNpc, "Initial", true));

		if(currentDialogueChoices.Count > 0)
			choiceGroup.gameObject.SetActive (true);
			// dialogueBtnPanel.gameObject.SetActive (true);

	}
}