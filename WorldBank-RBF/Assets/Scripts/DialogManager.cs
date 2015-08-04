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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Threading;
using System.Linq;

public class DialogManager : MonoBehaviour {

	static DialogManager _instance;

	// Singleton Manager
	public static DialogManager instance {

		get {

			if(_instance == null) {
				_instance = (DialogManager)GameObject.FindObjectOfType<DialogManager>();

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

	public delegate void BackButtonDelegate();

	TacticCardDialog tacticDialog;

	Text currentDialogLabel;
	int currentDialogIndex;
	double[] currentDialogueOpacity;
	List<string> currentDialogueText;
	
	Dictionary<string, string> currentDialogueChoices;
	Dictionary<string, Models.Dialogue> currentDialogueUnlockables;

	List<Models.NPC> talkedToNpcs = new List<Models.NPC>();
	StringBuilder builder = new StringBuilder();

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

	void CloseAll() {

		if(dialogBox != null)
			dialogBox.Close();

	}

	/// <summary>
	/// Generate a dialog with text and choice buttons
	/// </summary>
	/// <param name="strDialogTxt">Text to show in the dialogue</param>
	public void CreateChoiceDialog(string strDialogTxt, List<GenericButton> btnChoices, string strHeader="", BackButtonDelegate backEvent=null, bool worldSpace=false, bool left=false, bool back=false) {

		if (dialogBox == null) {
			// if(npc == null)
				dialogBox = CreateGenericDialog (strDialogTxt, worldSpace, left);
			// else
			// 	dialogBox = CreateNPCDialog (strDialogTxt, npc);
		} else {
			dialogBox.Content = strDialogTxt;
		}

		if (strHeader != "") dialogBox.Header = strHeader;
		dialogBox.AddButtons(btnChoices, !worldSpace);

		// Setup back button
		if (worldSpace) {
			Button backButton = dialogBox.BackButton;
			if (btnChoices.Count == 0 || back) {
				dialogBox.BackButton.gameObject.SetActive (true);
				backButton.onClick.RemoveAllListeners ();
				backButton.onClick.AddListener(() => backEvent());
			} else {
				dialogBox.BackButton.gameObject.SetActive (false);
			}
		}
	}

	/// <summary>
	/// Generate a generic dialog with text
	/// </summary>
	/// <param name="strDialogTxt">Text to show in the dialogue</param>
	public GenericDialogBox CreateGenericDialog(string strDialogTxt, bool worldSpace, bool left) {

	    dialogBox = ObjectPool.Instantiate<GenericDialogBox> ();
	    dialogBox.Open(null, worldSpace, left);
	    dialogBox.Content = strDialogTxt;

	    return dialogBox;

	}

	/// <summary>
	/// Generate a Scenario dialog for the specified scenario
	/// </summary>
	/// <param name="scenario">The instance of the scenario</param>
	/// <param name="strAdvisorSymbol">The symbol of the advisor who is talking (optional)</param>
	/// <param name="closeAll">Close all open dialogs first (optional, true by default)</param>
	public ScenarioCardDialog CreateScenarioDialog(Models.ScenarioCard scenario, string strAdvisorSymbol=null, bool closeAll=true) {

		// Close all diags
		if(closeAll)
			CloseAll();

		ScenarioCardDialog scenarioDialog = ObjectPool.Instantiate<ScenarioCardDialog>("Scenario");
	    scenarioDialog.Data = scenario;

	    scenarioDialog.transform.SetAsFirstSibling();

	    return scenarioDialog;

	}

	/// <summary>
	/// Generate a Scenario Decision dialog for the specified year end card.
	/// </summary>
	/// <param name="scenarioConfig">The instance of the current scenario's config.</param>
	public ScenarioDecisionDialog CreateScenarioDecisionDialog(Models.ScenarioConfig scenarioConfig) {

	    ScenarioDecisionDialog yearEndDialog = ObjectPool.Instantiate<ScenarioDecisionDialog>("Scenario");

	    yearEndDialog.Year = DataManager.CurrentYear;
	    yearEndDialog.Data = scenarioConfig;
	    
	    yearEndDialog.transform.SetAsFirstSibling();
	    yearEndDialog.Open();

	    return yearEndDialog;

	}

	/// <summary>
	/// Generate a Tactic card dialog for the specified tactic
	/// </summary>
	public TacticCardDialog CreateTacticDialog(Models.TacticCard tactic) {

	    // ;
	    //

	    // tacticDialog.RemoveButtons<GenericButton>(tacticDialog.HorizontalGroup);

		// tacticDialog.Init();

		List<GenericButton> btnList = new List<GenericButton>();

		TacticCardDialog tacticDialog = ObjectPool.Instantiate<TacticCardDialog>("Scenario");
		tacticDialog.Data = tactic;
		
		return tacticDialog;

	}

	/// <summary>
	/// Open intro dialog for a given character
	/// </summary>
	/// <param name="currNpc">Instance of Models.NPC for this NPC</param>
	public void OpenIntroDialog(Models.NPC currNpc, bool left) {

		int introTxtIndex = GetDialogIndex(currNpc);

		// If the "initial" text array index is over zero, or we've not talked to this NPC, show "Learn More"
		bool enableLearnMore = introTxtIndex > 0 || !talkedToNpcs.Contains (currNpc);

		List<GenericButton> btnChoices = new List<GenericButton> ();

		if (enableLearnMore && !PlayerData.InteractionGroup.Empty) {
			GenericButton btnChoice = ObjectPool.Instantiate<GenericButton> ();
			btnChoice.Text = "Learn More";
			btnChoice.Button.onClick.RemoveAllListeners ();
			btnChoice.Button.onClick.AddListener (() => OpenDialog (currNpc));
			btnChoices.Add (btnChoice);
		}

		Models.Character character = DataManager.GetDataForCharacter(currNpc.character);
		CreateChoiceDialog (
			character.description[Mathf.Clamp(introTxtIndex, 0, character.description.Length-1)],
			btnChoices,
			character.display_name,
			CloseAndUnfocus,
			true, left, true
		);

	}

	/// <summary>
	/// Open specified dialog for a given character
	/// </summary>
	/// <param name="currNpc">Instance of Models.NPC for this NPC</param>
	/// <param name="strDialogueKey">The key corresponding to the dialogue to show</param>
	/// <param name="returning">Specify whether player is returning to previous dialog</param>
	public void OpenSpeechDialog(Models.NPC currNpc, string strDialogueKey, bool returning=false, bool left=false) {

		Models.Dialogue currDialogue = currNpc.GetDialogue(strDialogueKey);
		bool initialDialogue = strDialogueKey == "Initial";
		int dialogTxtIndex = initialDialogue ? GetDialogIndex(currNpc) : 0;
		int intTxtCount = currDialogue.text.Length-1;

		CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
		TextInfo textInfo = cultureInfo.TextInfo;

		if (returning && currentDialogueChoices == null)
			throw new Exception ("You are trying to return to the previous dialog, but none exists");

		string strDialogTxt = currDialogue.text[Mathf.Clamp(dialogTxtIndex, 0, intTxtCount)];

		// Match any characters in between [[ and ]]
		string strKeywordRegex = "(\\[)(\\[)(.*?)(\\])(\\])";

		// Does this dialogue unlock something?
		if(currDialogue.unlocks != null)
		{
			string[] unlockableSymbols = currDialogue.unlocks;

			foreach(string symbol in unlockableSymbols)
			{

				Models.Unlockable unlockableRef = DataManager.GetUnlockableBySymbol(symbol);
				strDialogTxt += "\n\n<color=yellow>Unlocked</color> " + unlockableRef.title + " ";

				// Unlock this implementation option for player
				PlayerData.UnlockItem(symbol);

			}

		}
 		
 		string strToDisplay = strDialogTxt;

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
			currentDialogueChoices = new Dictionary<string, string>();

			currentDialogueUnlockables = currNpc.dialogue
												.Where(kvp => kvp.Key.StartsWith("unlockable_dialogue_"))
												.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

			foreach(Match m in keyMatches) {
			    if (m.Success)
				{
			        string strKeyword = m.Groups[3].ToString();
			        string strKeywordTitle = textInfo.ToTitleCase(m.Groups[3].ToString());

			        // This is kind of a mess but it's currently how I am deciding which dialogue option to highlight, based on if something is unlocked (or not unlockable) 
			       	IEnumerable<KeyValuePair<string, Models.Dialogue>> unlockLookup = currentDialogueUnlockables
																				      .Where(diag => diag.Value.display_name == strKeywordTitle);
			       	string unlockableKey = null;

			       	// This key is unlockable dialogue?
			       	if(unlockLookup.Any())
			       		unlockableKey = unlockLookup.Select(diag => diag.Key).First(); 

			       	// Highlight options that are unlocked or not an unlockable
			       	if((unlockableKey != null && PlayerData.DialogueGroup.IsUnlocked(unlockableKey)) || unlockableKey == null) {
						
						strToDisplay = strToDisplay.Replace("[[" + strKeyword + "]]", "<color=orange>" + strKeyword + "</color>");
	
						if(unlockableKey == null)
							currentDialogueChoices.Add(strKeywordTitle, strKeywordTitle);
						else
							currentDialogueChoices.Add(unlockableKey, strKeywordTitle);

				    }
				    // Remove brackets but do not highlight keyword
				    else
					    strToDisplay = strToDisplay.Replace("[[" + strKeyword + "]]", strKeyword);
				}
			}

			// Add dialogue options not in text
			foreach(KeyValuePair<string, Models.Dialogue> dialogue in currentDialogueUnlockables)
			{
				if(PlayerData.DialogueGroup.IsUnlocked(dialogue.Key))
			        currentDialogueChoices.Add(dialogue.Key, dialogue.Value.display_name);
			}
		}

		List<GenericButton> btnList = new List<GenericButton>();

		foreach(KeyValuePair<string, string> choice in currentDialogueChoices) {

			string choiceKey = choice.Key;
			string choiceName = choice.Value;

			GenericButton btnChoice = ObjectPool.Instantiate<GenericButton>();
			btnChoice.Text = choiceName;

			btnChoice.Button.onClick.RemoveAllListeners();

			KeyValuePair<string, string> choiceRef = choice; // I don't understand why this is necessary, but if you just pass in 'choice' below, it will break
			btnChoice.Button.onClick.AddListener (() => currentDialogueChoices.Remove(choiceRef.Key));
			btnChoice.Button.onClick.AddListener(() => OpenSpeechDialog(currNpc, choiceKey, false, left));

			btnList.Add(btnChoice);
		}

		BackButtonDelegate del = CloseAndUnfocus;

		CreateChoiceDialog(strToDisplay, btnList, "", del, true, left);
	}

	public void OpenSpeechDialog(string symbol, string strDialogueKey, bool returning=false, bool left=false) {
		OpenSpeechDialog (NpcManager.GetNpc (symbol), strDialogueKey, returning, left);
	}

	void OpenDialog (Models.NPC currNpc) {
		NPCFocusBehavior.Instance.DialogFocus ();
		talkedToNpcs.Add (currNpc);
	}

	void CloseAndUnfocus () {
		NPCFocusBehavior.Instance.DefaultFocus ();
		CloseCharacterDialog ();
	}

    /// <summary>
    /// Determine which text index (for description and initial text) to show by looking at if player has previously unlocked 1 or more of the referenced NPC's unlockables
    /// </summary>
    /// <param name="npcRef">The NPC reference</param>
    /// <returns>The text index</returns>
	int GetDialogIndex(Models.NPC npcRef) {

		int intDialogIndex = 0;

		foreach(string[] arrUnlocks in DataManager.GetUnlocksForCharacter(npcRef))
		{
			bool unlockItemUnlocked = false;

			if(arrUnlocks[0].StartsWith("unlockable_dialogue_")) 
				unlockItemUnlocked = PlayerData.DialogueGroup.IsUnlocked(arrUnlocks[0]);
			else if(!arrUnlocks[0].StartsWith("unlockable_route_"))
				unlockItemUnlocked = PlayerData.TacticGroup.IsUnlocked(arrUnlocks[0]);

			if(unlockItemUnlocked)
				intDialogIndex++;
		}

		return intDialogIndex;

	}

	public void CloseCharacterDialog (bool openNext=true) {
		if (dialogBox != null) {
			dialogBox.Close ();
			dialogBox = null;
		}
	}
}
