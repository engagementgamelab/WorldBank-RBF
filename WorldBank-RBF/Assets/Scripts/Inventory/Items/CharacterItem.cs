using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Models;

public class CharacterItem : ModelItem {

	new public string Symbol { get; private set; }
	new public Character Model { get; private set; }

	public NPC Npc { get; private set; }
	public string DisplayName { get; private set; }
	
	Dialogue initialDialog;
	public string InitialDialog { 
		get { return initialDialog.text[Returning ? 1 : 0]; }
	}

	Dictionary<string, Dialogue> choices;
	public Dictionary<string, Dialogue> Choices {
		get {
			if (choices == null) {
				if (!phaseOneCharacter) {
					choices = new Dictionary<string, Dialogue> ();
				} else {
					choices = new Dictionary<string, Dialogue> (
						Npc.dialogue, StringComparer.OrdinalIgnoreCase);
				}
			}
			return choices;
		}
		set { choices = value; }
	}

	List<string> descriptions;
	List<string> Descriptions {
		get {
			if (descriptions == null) {
				descriptions = Model.description.ToList ();
			}
			return descriptions;
		}
	}

	public bool Returning {
		get { return Choices.Count < Npc.dialogue.Count-1; }
	}

	public bool NoChoices {
		get { return Choices.Count == 0 && Returning; }
	}

	KeyValuePair<string, Dialogue> currentDialog;
	public Dialogue CurrentDialog { // TODO: should this be a string?
		get { return currentDialog.Value; }
	}

	bool phaseOneCharacter;

	public CharacterItem () {}

	public CharacterItem (Character model) {
		this.Model = model;
		Symbol = Model.symbol;
		this.Npc = DataManager.GetNpc (Symbol);
		phaseOneCharacter = (Npc != null);
		DisplayName = Model.display_name;
		SetInitialDialog ();
	}

	void SetInitialDialog () {
		if (!phaseOneCharacter) return;
		initialDialog = Choices.FirstOrDefault (x => x.Key == "Initial").Value;
		Choices.Remove ("Initial");
	}

	public string GetDescription () {
		if (NoChoices) return Descriptions[Descriptions.Count-1];
		if (Returning) return Descriptions[1];
		return Descriptions[0];
	}

	public void SelectChoice (string choice) {
		
		currentDialog = Choices.FirstOrDefault (x => x.Key == choice);
		Dialogue dialog = currentDialog.Value;
		
		if (dialog.unlocks != null) {

			string[] unlockableSymbols = dialog.unlocks;
			foreach (string symbol in unlockableSymbols) {
				Models.Unlockable unlockableRef = DataManager.GetUnlockableBySymbol(symbol);
				PlayerData.UnlockItem(symbol);
				Debug.Log ("unlocked: " + symbol);
			}
		}

		Choices.Remove (choice);
	}
}
