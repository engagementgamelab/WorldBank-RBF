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
	public Transform uiCanvasRoot;
	public Button btnPrefab;
	public GenericDialogBox dialogBox;
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

	private void CloseAll() {

		if(dialogBox != null)	
			dialogBox.Close();

		if(scenarioDialog != null)	
			scenarioDialog.Close();

	}

	/// <summary>
	/// Generate a dialog with text and choice buttons
	/// </summary>
	/// <param name="strDialogTxt">Text to show in the dialogue</param>
	public void CreateChoiceDialog(string strDialogTxt, List<GenericButton> btnChoices, BackButtonDelegate backEvent=null, NPCBehavior npc=null) {

		if (dialogBox == null) {
			// if(npc == null)
				dialogBox = CreateGenericDialog (strDialogTxt);
			// else
			// 	dialogBox = CreateNPCDialog (strDialogTxt, npc);
		} else {
			dialogBox.Content = strDialogTxt;
		}

		dialogBox.AddButtons(btnChoices, true);
		
		// Setup back button
	/*	Button backButton = dialogBox.backButton;

		backButton.onClick.RemoveAllListeners ();
		backButton.onClick.AddListener(() => backEvent());

		if(choiceGroup.childCount > 0)
			choiceGroup.gameObject.SetActive (true);*/
	}

	/// <summary>
	/// Generate a generic dialog with text
	/// </summary>
	/// <param name="strDialogTxt">Text to show in the dialogue</param>
	public GenericDialogBox CreateGenericDialog(string strDialogTxt) {

	    dialogBox = ObjectPool.Instantiate<GenericDialogBox> ();
	    dialogBox.Open();
	    dialogBox.Content = strDialogTxt;
	    return dialogBox;
	}


	/// <summary>
	/// Generate a Scenario dialog for the specified scenario
	/// </summary>
	/// <param name="scenario">The instance of the scenario</param>
	/// <param name="strAdvisorSymbol">The symbol of the advisor who is talking (optional)</param>
	public ScenarioDialog CreateScenarioDialog(Models.Scenario scenario, string strAdvisorSymbol=null) {

		// Close all diags
		CloseAll();

	    scenarioDialog = ObjectPool.Instantiate<ScenarioDialog>();

	    // Get initial dialogue or an advisor's?
	    if(strAdvisorSymbol == null)
		    scenarioDialog.Content = scenario.initiating_dialogue;
		else
		{
			Models.Advisor advisor = scenario.characters[strAdvisorSymbol];
			scenarioDialog.Content = advisor.dialogue;

			if(advisor.unlocks != null)
			{
				foreach(string option in advisor.unlocks)
					ScenarioManager.currentCardOptions.Add(option);
			}
		}

		List<GenericButton> btnListAdvisors = new List<GenericButton>();
		List<GenericButton> btnListOptions = new List<GenericButton>();

		List<string> choiceOptionsText = ScenarioManager.currentCardOptions;

		// TODO: Button creation itself ought to be a discrete method. This is all needlessly complex.

		// Create buttons for all advisors
		foreach(string characterSymbol in new List<string>(scenario.characters.Keys)) {

			// Show an advisor only if they have dialogue
			if(!scenario.characters[characterSymbol].hasDialogue)
				continue;

			GenericButton btnChoice = ObjectPool.Instantiate<GenericButton>();
			
			btnChoice.Text = DataManager.GetDataForCharacter(characterSymbol).display_name;

			btnChoice.Button.onClick.RemoveAllListeners();
			btnChoice.Button.onClick.AddListener (() => CreateScenarioDialog(scenario, characterSymbol));

			btnListAdvisors.Add(btnChoice);
		}

		// Create buttons for all options if not speaking to advisor
		foreach(string option in choiceOptionsText) {

			GenericButton btnChoice = ObjectPool.Instantiate<GenericButton>();
			
			btnChoice.Text = option;

			btnChoice.Button.onClick.RemoveAllListeners();
			
			if(option == "Back")
				btnChoice.Button.onClick.AddListener (() => CreateScenarioDialog(scenario));

			btnListOptions.Add(btnChoice);
		}

		scenarioDialog.AddButtons(btnListAdvisors);
		scenarioDialog.AddButtons(btnListOptions, true);
	    
	    return scenarioDialog;

	}

	/// <summary>
	/// Open intro dialog for a given character
	/// </summary>
	/// <param name="currNpc">Instance of Models.NPC for this NPC</param>
	/// <param name="npcInstance">Instance of NPCBehavior for this NPC</param>
	public void OpenIntroDialog(Models.NPC currNpc, NPCBehavior npcInstance) {

		GenericButton btnChoice = ObjectPool.Instantiate<GenericButton> ();
		
		btnChoice.Text = "Learn More";

		btnChoice.Button.onClick.RemoveAllListeners ();
		btnChoice.Button.onClick.AddListener(() => npcInstance.DialogFocus());

		CreateChoiceDialog(

			DataManager.GetDataForCharacter(currNpc.character).description, 
			new List<GenericButton>(){ btnChoice },
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

		List<GenericButton> btnList = new List<GenericButton>();

		foreach(string choice in currentDialogueChoices) {
			CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
			TextInfo textInfo = cultureInfo.TextInfo;

			string choiceName = textInfo.ToTitleCase(choice);

			GenericButton btnChoice = ObjectPool.Instantiate<GenericButton>();
			btnChoice.Text = choiceName;

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
			dialogBox.Close ();
			dialogBox = null;
		}
	}
}