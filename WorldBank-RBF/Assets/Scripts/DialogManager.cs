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

	private static DialogManager _instance;

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

	private ScenarioCardDialog scenarioDialog;
	private TacticCardDialog tacticDialog;

	private StringBuilder builder = new StringBuilder();

	private Text currentDialogLabel;
	private int currentDialogIndex;
	private double[] currentDialogueOpacity;
	private List<string> currentDialogueText;
	private List<string> currentDialogueChoices;
	private List<Models.NPC> talkedToNpcs = new List<Models.NPC> ();

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
	public void CreateChoiceDialog(string strDialogTxt, List<GenericButton> btnChoices, BackButtonDelegate backEvent=null, bool worldSpace=false, bool left=false, bool back=false) {

		if (dialogBox == null) {
			// if(npc == null)
				dialogBox = CreateGenericDialog (strDialogTxt, worldSpace, left);
			// else
			// 	dialogBox = CreateNPCDialog (strDialogTxt, npc);
		} else {
			dialogBox.Content = strDialogTxt;
		}

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

	    scenarioDialog = ObjectPool.Instantiate<ScenarioCardDialog>();
	    scenarioDialog.data = scenario;

	    // scenarioDialog.transform.SetParent(uiCanvasRoot);
	    scenarioDialog.transform.SetAsFirstSibling();

	    scenarioDialog.Header = scenario.name;

	    // Get initial dialogue or an advisor's?
	    if(strAdvisorSymbol == null)
		    scenarioDialog.Content = scenario.initiating_dialogue;

		// Create buttons for all advisors
		scenarioDialog.AddAdvisors(ScenarioManager.currentAdvisorOptions);

		// Create buttons for all options if not speaking to advisor
		scenarioDialog.AddOptions(ScenarioManager.currentCardOptions);

	    scenarioDialog.Open();

	    return scenarioDialog;

	}

	/// <summary>
	/// Generate a Tactic card dialog for the specified tactic
	/// </summary>
	public TacticCardDialog CreateTacticDialog(Models.TacticCard tactic) {

	    tacticDialog = ObjectPool.Instantiate<TacticCardDialog>();
	    tacticDialog.data = tactic;

	    tacticDialog.Content = tactic.initiating_dialogue;

	    tacticDialog.RemoveButtons<GenericButton>(tacticDialog.HorizontalGroup);

		tacticDialog.Init();

	    return tacticDialog;

	}

	/// <summary>
	/// Open intro dialog for a given character
	/// </summary>
	/// <param name="currNpc">Instance of Models.NPC for this NPC</param>
	public void OpenIntroDialog(Models.NPC currNpc, bool left) {

		List<GenericButton> btnChoices = new List<GenericButton> ();
		
		if (!talkedToNpcs.Contains (currNpc)) {
			GenericButton btnChoice = ObjectPool.Instantiate<GenericButton> ();
			btnChoice.Text = "Learn More";
			btnChoice.Button.onClick.RemoveAllListeners ();
			btnChoice.Button.onClick.AddListener (() => OpenDialog (currNpc));
			btnChoices.Add (btnChoice);
		}

		CreateChoiceDialog (
			DataManager.GetDataForCharacter(currNpc.character).description, 
			btnChoices,
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
	public void OpenSpeechDialog(Models.NPC currNpc, string strDialogueKey, bool returning=false) {

		if (returning && currentDialogueChoices == null)
			throw new Exception ("You are trying to return to the previous dialog, but none exists");

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
			btnChoice.Button.onClick.AddListener(() => OpenSpeechDialog(currNpc, choiceName, false));

			btnList.Add(btnChoice);
		}

		// BackButtonDelegate del = null;
		BackButtonDelegate del = CloseAndUnfocus;

		/*if (strDialogueKey == "Initial") {
			del = CloseAndUnfocus;
		} else {
			del = delegate { OpenSpeechDialog(currNpc, "Initial", true); };
		}*/

		CreateChoiceDialog(strToDisplay, btnList, del, true);
	}

	public void OpenSpeechDialog(string symbol, string strDialogueKey, bool returning=false) {
		OpenSpeechDialog (NpcManager.GetNpc (symbol), strDialogueKey, returning);
	}

	void OpenDialog (Models.NPC currNpc) {
		NPCFocusBehavior.Instance.DialogFocus ();
		talkedToNpcs.Add (currNpc);
	}

	void CloseAndUnfocus () {
		NPCFocusBehavior.Instance.DefaultFocus ();
		CloseCharacterDialog ();
	}

	public void CloseCharacterDialog (bool openNext=true) {
		if (dialogBox != null) {
			dialogBox.Close ();
			dialogBox = null;
		}
	}
}