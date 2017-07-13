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
using UnityEngine.Events;
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
	public bool version2 = false;
	public ScenarioChatScreen scenarioChat;
	public SupervisorChatScreen supervisorChat;
	public NpcDialogBox npcDialogBox;

	public GenericDialogBox dialogBox;

	public delegate void BackButtonDelegate();

	TacticCardDialog tacticDialog;
	double[] currentDialogueOpacity;

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

	void CloseAll () {
		if (dialogBox != null) {
			dialogBox.Close();
			dialogBox = null;
		}
		// if (npcDialogBox != null)
		// 	npcDialogBox.Close ();
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

		if (dialogBox == null) {
		    dialogBox = ObjectPool.Instantiate<GenericDialogBox> ();
		}

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

		if (!version2) {

			ScenarioCardDialog scenarioDialog = ObjectPool.Instantiate<ScenarioCardDialog>("Scenario");
		    scenarioDialog.Data = scenario;

		    scenarioDialog.transform.SetAsFirstSibling();

		    return scenarioDialog;
		}
		return null;
	}

	public void SetCard (Models.ScenarioCard scenario) {
		CloseAll();
		scenarioChat.Data = scenario;
	}

	public void SetAvailableTactics (List<string> availableTactics) {
		
		supervisorChat.Available = DataManager.GetTacticCardsForSymbols(availableTactics.ToArray());
		
	}

	/// <summary>
	/// Generate a Tactic card dialog for the specified tactic
	/// </summary>
	public TacticCardDialog CreateTacticDialog(Models.TacticCard tactic) {

		// List<GenericButton> btnList = new List<GenericButton>();

		TacticCardDialog tacticDialog = ObjectPool.Instantiate<TacticCardDialog>("Scenario");
		tacticDialog.Data = tactic;
		
		return tacticDialog;

	}

	/// <summary>
	/// Opens a new dialogue box with a description of the selected NPC and the option to learn more or exit
	/// </summary>
	/// <param name="currNpc">The NPC to get the description from</param>
	/// <param name="left">If true, the dialog box will appear on the left side of the screen</param>
	public void OpenNpcDescription (Models.NPC currNpc, bool left) {
		
		CharacterItem character = PlayerData.CharacterGroup[currNpc.symbol];
		string description = character.GetDescription ();

		Dictionary<string, UnityAction> btnChoices = new Dictionary<string, UnityAction> ();
		if (!character.NoChoices && !PlayerData.InteractionGroup.Empty) {
			btnChoices.Add ("Learn More", () => {
				// CloseAll ();
				npcDialogBox.Clear ();
				NPCFocusBehavior.Instance.DialogFocus ();
			});
		}
		btnChoices.Add ("Back", () => {CloseAndUnfocus(); });

		npcDialogBox.Open (character.DisplayName, description, btnChoices, left);
	}

	//// <summary>
	/// Opens a new dialogue box with dialogue from the given NPC.
	/// </summary>
	/// <param name="currNpc">The NPC to get dialogue from</param>
	/// <param name="left">If true, the dialogue box will appear on the left side of the screen</param>
	/// <param name="initial">If true, will check for choices the player can select to further the dialogue</param>
	public void OpenNpcDialog (Models.NPC currNpc, string voice, bool left, bool initial=true) {

		CharacterItem character = PlayerData.CharacterGroup[currNpc.symbol];
		string dialog = (initial) 
			? character.InitialDialog
			: character.CurrentDialog.text[0];

		// Find choices in the dialogue
		Dictionary<string, bool> choices = GetChoices (character, dialog);

		// Highlight the choices
		dialog = HighlightChoices (character, dialog, choices);

		// Convert all choices to lowercase for cross referencing with the character's choices
		choices = choices.ToDictionary (x => x.Key.ToLower (), x => x.Value);

		Dictionary<string, UnityAction> btnChoices = new Dictionary<string, UnityAction> ();

		if (!character.NoChoices && !PlayerData.InteractionGroup.Empty) {
			foreach (var choice in character.Choices) {
			
				Models.Dialogue model = choice.Value;
				
				string ck = choice.Key;
				string displayName = ck;

				if (!DialogueUnlocked (model, ck, ref displayName))
					continue;
				
				if (!(character.Returning && initial) && !choices.ContainsKey (ck.ToLower ()))
					continue;

				// Hack that gives the dialogue option a different color if it is an unlockable
				if (ck.StartsWith ("unlockable_dialogue_"))
					displayName += "~";

				btnChoices.Add (displayName, () => {

					AudioManager.Sfx.Play (voice + "response", "NPCs");
					
					PlayerData.InteractionGroup.Remove ();
					character.SelectChoice (
						ck, 
						(model.unlocks_context != null) ? model.unlocks_context[0] : "",
						character.Symbol
					);
					OpenNpcDialog (currNpc, voice, left, false);

				});
			}
		}

		btnChoices.Add ("Back",() => { CloseAndUnfocus(character.Choices.Count); });
		npcDialogBox.Open (character.DisplayName, dialog, btnChoices, left);
	}

	/// <summary>
	/// Generate a tooltip screen
	/// </summary>
	public void CreateTutorialScreen(string strTooltipKey, string strNextKey=null, UnityAction confirmAction=null) {

		if(!DataManager.tutorialEnabled)
			return;		

		RemoveTutorialScreen();

		// Do not show if already seen
		if(DataManager.usedTooltips.Contains(strTooltipKey))
			return;

		TutorialScreen tutScreen = ObjectPool.Instantiate<TutorialScreen>();
		tutScreen.Load(strTooltipKey, strNextKey, confirmAction);

		tutScreen.transform.SetParent(GameObject.Find("Overlay").transform);

		// Add to used tooltips
		DataManager.usedTooltips.Add(strTooltipKey);

	}

	/// <summary>
	/// Remove all tooltip screens
	/// </summary>
	public void RemoveTutorialScreen() {
		// ObjectPool.DestroyAll<TutorialScreen>();
		Events.instance.Raise (new CloseTutorialEvent ());
	}

	Dictionary<string, bool> GetChoices (CharacterItem character, string dialog) {

		// Match any characters in between [[ and ]]
		/*string strKeywordRegex = "(\\[)(\\[)(.*?)(\\])(\\])";
		Regex regexKeywords = new Regex (strKeywordRegex, RegexOptions.IgnoreCase);
		MatchCollection keyMatches = regexKeywords.Matches (dialog);
		TextInfo textInfo = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo;*/
		Dictionary<string, bool> choices = new Dictionary<string, bool> ();

		/*foreach (Match m in keyMatches) {
		    if (m.Success) {
		    	string strKeyword = m.Groups[3].ToString();
		    	string choiceKey = textInfo.ToTitleCase (strKeyword.ToLower ());
		    	if (character.Choices.ContainsKey (choiceKey)) {
		    		choices.Add (strKeyword, true);
		    	} else {
		    		choices.Add (strKeyword, false);
		    	}
		    }
		}*/

		foreach (var c in character.Choices) {
			choices.Add (c.Key, true);
		}

		return choices;
	}

	string HighlightChoices (CharacterItem character, string dialog, Dictionary<string, bool> choices) {
		
		/*foreach (var choice in choices) {
			string strKeyword = choice.Key;
			bool unlocked = choice.Value;
			dialog = dialog.Replace ("[[" + strKeyword + "]]", strKeyword);
		}*/

		dialog = dialog.Replace ("[[", "");
		dialog = dialog.Replace ("]]", "");

		return dialog;
	}

	bool DialogueUnlocked (Models.Dialogue model, string choiceKey, ref string displayName) {
		if (choiceKey.StartsWith ("unlockable_dialogue_")) {
			displayName = model.display_name;
			if (!PlayerData.DialogueGroup.Unlockables.FirstOrDefault (x => x.Symbol == choiceKey).Unlocked)
				return false;
		}
		return true;
	}

	void CloseAndUnfocus (int choicesCount=0) {
		NPCFocusBehavior.Instance.DefaultFocus (choicesCount);
		// CloseAll ();
		npcDialogBox.Close ();
	}
	
	// TODO: This needs some work - text doesn't fade correctly, and also it needs to handle
	// text that already has color tags
	IEnumerator CoFadeText () {
	
		List<string> fadeText = new List<string> ();
		List<double> textOpacity = new List<double> ();
		foreach (char c in dialogBox.Content) {
			fadeText.Add (c.ToString ());
			textOpacity.Add (0);
		}
		
		int textLength = fadeText.Count;
		int index = 0;
		string[] arrText = new string[textLength];

		while (index < textLength) {
			for (int i = 0; i < index; i ++) {

				textOpacity[i] ++;

				if (fadeText[i].IndexOf ("<color") == -1 && textOpacity[i] < 100)
					arrText[i] = "<color=#ffffff" + textOpacity[i] + ">" + fadeText[i] + "</color>";			
				else
					arrText[i] = fadeText[i];
			}

			dialogBox.Content = string.Join ("", arrText);
			index ++;

			yield return null;
		}
	}
}
