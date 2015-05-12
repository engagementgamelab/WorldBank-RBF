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
	public ScenarioDialog scenarioDialog;

	public delegate void BackButtonDelegate();

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
	/// Generate a dialog with text and choice buttons
	/// </summary>
	/// <param name="strDialogTxt">Text to show in the dialogue</param>
	public void CreateChoiceDialog(string strDialogTxt, List<NPCDialogButton> btnChoices, BackButtonDelegate backEvent, NPCBehavior npc) {

		if (dialogBox == null) {
			dialogBox = CreateNPCDialog (strDialogTxt, npc);
		} else {
			dialogBox.Content = strDialogTxt;
		}

		Transform choiceGroup = dialogBox.choiceGroup;
		NPCDialogButton[] remove = choiceGroup.GetComponentsInChildren<NPCDialogButton>();

		foreach (NPCDialogButton child in remove)
			ObjectPool.Destroy<NPCDialogButton> (child.transform);

		if(btnChoices != null) {
			foreach(NPCDialogButton btnChoice in btnChoices) {
				btnChoice.transform.SetParent(choiceGroup);
				btnChoice.transform.localScale = Vector3.one;
				btnChoice.transform.localPosition = Vector3.zero;
				btnChoice.transform.localEulerAngles = Vector3.zero;
			}
		}
		
		// Setup back button
		Button backButton = dialogBox.backButton;

		backButton.onClick.RemoveAllListeners ();
		backButton.onClick.AddListener(() => backEvent());

		if(choiceGroup.childCount > 0)
			choiceGroup.gameObject.SetActive (true);
	}

	/// <summary>
	/// Generate an NPC dialog with text
	/// </summary>
	/// <param name="strDialogTxt">Text to show in the dialogue</param>
	/// <param name="npcInstance">Instance of NPCBehavior for this NPC</param>
	public NPCDialogBox CreateNPCDialog(string strDialogTxt, NPCBehavior npc) {

	    dialogBox = ObjectPool.Instantiate<NPCDialogBox> ();
	    dialogBox.Open (npc);
	    dialogBox.Content = strDialogTxt;
	    return dialogBox;
	}

	public GenericDialogBox CreateGenericDialog() {

	    GenericDialogBox dialog = ObjectPool.Instantiate<GenericDialogBox>();
	    dialog.Open ();
	    dialog.Content = "strDialogTxt";

	    return dialog;

	}

	public ScenarioDialog CreateScenarioDialog(string strSymbol) {

		Models.Scenario scenario = DataManager.GetScenarioBySymbol(strSymbol);

	    scenarioDialog = ObjectPool.Instantiate<ScenarioDialog>();
	    // scenarioDialog.Open();

	    Debug.Log(scenario.characters["dep_minister_of_health"]);

	    scenarioDialog.Content = scenario.initiating_dialogue;
	    
	    return scenarioDialog;

	}

	/// <summary>
	/// Open intro dialog for a given character
	/// </summary>
	/// <param name="currNpc">Instance of Models.NPC for this NPC</param>
	/// <param name="npcInstance">Instance of NPCBehavior for this NPC</param>
	public void OpenIntroDialog(Models.NPC currNpc, NPCBehavior npcInstance) {

		NPCDialogButton btnChoice = ObjectPool.Instantiate<NPCDialogButton> ();
		
		Text btnTxt = btnChoice.Button.transform.FindChild("Text").GetComponent<Text>();
		btnTxt.text = "Learn More";

		btnChoice.Button.onClick.RemoveAllListeners ();
		btnChoice.Button.onClick.AddListener(() => npcInstance.DialogFocus());

		CreateChoiceDialog(

			DataManager.GetDataForCharacter(currNpc.character).description, 
			new List<NPCDialogButton>(){ btnChoice },
			delegate { CloseCharacterDialog(false); },
			npcInstance

		);

	}

	/// <summary>
	/// Open specified dialog for a given character
	/// </summary>
	/// <param name="currNpc">Instance of Models.NPC for this NPC</param>
	/// <param name="strDialogueKey">The key corresponding to the dialogue to show</param>
	/// <param name="returning">Specify whether player is returning to previous dialog</param>
	public void OpenSpeechDialog(Models.NPC currNpc, string strDialogueKey, NPCBehavior npc, bool returning=false) {

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
		
		/*		
		currentDialogueText = new List<string>();
		
		foreach(char c in strDialogTxt)
			currentDialogueText.Add(c.ToString());

		currentDialogueOpacity = new double[currentDialogueText.Count];
		*/

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
			        currentDialogueChoices.Add(strKeyword);	
				}
			}
		}

		List<NPCDialogButton> btnList = new List<NPCDialogButton>();

		foreach(string choice in currentDialogueChoices) {
			CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
			TextInfo textInfo = cultureInfo.TextInfo;

			string choiceName = textInfo.ToTitleCase(choice);

			NPCDialogButton btnChoice = ObjectPool.Instantiate<NPCDialogButton>();
			
			Text btnTxt = btnChoice.Button.transform.FindChild("Text").GetComponent<Text>();
			btnTxt.text = choiceName;

			btnChoice.Button.onClick.RemoveAllListeners();

			string t = choice; // I don't understand why this is necessary, but if you just pass in 'choice' below, it will break
			btnChoice.Button.onClick.AddListener (() => currentDialogueChoices.Remove(t));
			btnChoice.Button.onClick.AddListener(() => OpenSpeechDialog(currNpc, choiceName, npc, false));

			btnList.Add(btnChoice);
		}

		BackButtonDelegate del = null;

		if (strDialogueKey == "Initial") {
			del = CloseCharacterDialog;
		} else {
			del = delegate { OpenSpeechDialog(currNpc, "Initial", npc, true); };
		}

		CreateChoiceDialog(strToDisplay, btnList, del, npc);
	}

	public void CloseCharacterDialog (bool openNext=true) {
		if (dialogBox != null) {
			dialogBox.Close (openNext);
			dialogBox = null;
		}
	}
}