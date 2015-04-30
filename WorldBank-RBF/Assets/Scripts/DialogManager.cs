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
	public NPCDialogBox dialogBox;

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
	public NPCDialogBox CreateNPCDialog(string strDialogTxt, NPCBehavior npc) {

	    dialogBox = ObjectPool.Instantiate<NPCDialogBox> ();
	    dialogBox.Open (npc);
	    dialogBox.Content = strDialogTxt;
	    return dialogBox;
	}

	/// <summary>
	/// Open specified dialog for a given character
	/// </summary>
	/// <param name="currNpc">Instance of Models.NPC for this NPC</param>
	/// <param name="strDialogueKey">The key corresponding to the dialogue to show</param>
	/// <param name="returning">Specify whether player is returning to previous dialog</param>
	public void OpenCharacterDialog(Models.NPC currNpc, string strDialogueKey, NPCBehavior npc, bool returning=false) {//, NPCDialogBox dialogBox=null) {

		string strDialogTxt = currNpc.dialogue[strDialogueKey]["text"];
		
		// Match any characters in between [[ and ]]
		string strKeywordRegex = "(\\[)(\\[)(.*?)(\\])(\\])";

		// Does this dialogue unlock something?
		if(currNpc.dialogue[strDialogueKey].ContainsKey("unlocks"))
		{
			strDialogTxt += "\n\n<color=yellow>Unlocks</color>: " + currNpc.dialogue[strDialogueKey]["unlocks"];

			// Unlock this implementation option for player
			PlayerData.UnlockImplementation(currNpc.dialogue[strDialogueKey]["unlocks"]);
		}

		string strToDisplay = strDialogTxt.Replace("[[", "<color=orange>").Replace("]]", "</color>");

		if (dialogBox == null) {
			dialogBox = CreateNPCDialog (strToDisplay, npc);
		} else {
			dialogBox.Content = strToDisplay;
		}

		Transform choiceGroup = dialogBox.choiceGroup;
		foreach (Transform child in choiceGroup) {
			ObjectPool.Destroy<NPCDialogButton> (child);
		}
		
		Button backButton = dialogBox.backButton;
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

			Button btnChoice = ObjectPool.Instantiate<NPCDialogButton> ().Button;
			btnChoice.transform.SetParent (choiceGroup);
			btnChoice.transform.localScale = new Vector3(1, 1, 1);
			btnChoice.transform.localPosition = Vector3.zero;
			btnChoice.transform.localEulerAngles = Vector3.zero;
			
			Text btnTxt = btnChoice.transform.FindChild("Text").GetComponent<Text>();
			btnTxt.text = choiceName;

			btnChoice.onClick.RemoveAllListeners ();
			string t = choice; // I don't understand why this is necessary, but if you just pass in 'choice' below, it will break
			btnChoice.onClick.AddListener (() => currentDialogueChoices.Remove (t));
			btnChoice.onClick.AddListener(() => OpenCharacterDialog(currNpc, choiceName, npc, false));//, dialogBox));
		}

		// Setup back button
		if (strDialogueKey == "Initial") {
			backButton.onClick.RemoveAllListeners ();
			// backButton.onClick.AddListener (() => dialogBox.Close ());
			// backButton.onClick.AddListener (() => dialogBox = null);
			backButton.onClick.AddListener (() => CloseCharacterDialog ());
		} else {
			backButton.onClick.RemoveAllListeners ();
			backButton.onClick.AddListener(() => OpenCharacterDialog(currNpc, "Initial", npc, true));//, dialogBox));
		}

		if(currentDialogueChoices.Count > 0) {
			choiceGroup.gameObject.SetActive (true);
		}
	}

	public void CloseCharacterDialog () {
		if (dialogBox != null) {
			dialogBox.Close ();
			dialogBox = null;
		}
	}
}