using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Models;

public class CharacterItem : ModelItem {

	new public string Symbol { get; private set; }
	new public Character Model { get; private set; }

	/// <summary>
	/// The NPC model associated with this character.
	/// </summary>
	public NPC Npc { get; private set; }

	/// <summary>
	/// The name that gets shown in the game.
	/// </summary>
	public string DisplayName { get; private set; }
	
	Dialogue initialDialog;

	/// <summary>
	/// The first dialogue that gets shown after the player decides to "learn more" after the description.
	/// </summary>
	public string InitialDialog { 
		get { 
			try {
				return initialDialog.text[Returning ? 1 : 0]; 
			} catch {
				throw new System.Exception (Symbol + " is missing its second initial text");
			}
		}
	}

	Dictionary<string, Dialogue> choices;

	/// <summary>
	/// All of the possible choices that can be shown in the dialogue.
	/// These get removed as a player selects a choice.
	/// </summary>
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

	/// <summary>
	/// The descriptions of the character.
	/// </summary>
	List<string> Descriptions {
		get {
			if (descriptions == null) {
				descriptions = Model.description.ToList ();
			}
			return descriptions;
		}
	}

	/// <summary>
	/// Returns true if the player has talked to this character before.
	/// </summary>
	public bool Returning {
		get { return Choices.Count < Npc.dialogue.Count-1; }
	}

	/// <summary>
	/// Returns true if the player has selected all the choices.
	/// </summary>
	public bool NoChoices {
		get { return Choices.Count == 0 && Returning; }
	}

	KeyValuePair<string, Dialogue> currentDialog;

	/// <summary>
	/// The Dialogue model that the player is currently seeing.
	/// </summary>
	public Dialogue CurrentDialog {
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

	/// <summary>
	/// Gets the current description of the character (descriptions update based on how many times the player has approached the character)
	/// </summary>
	/// <returns>A description of the character.</returns>
	public string GetDescription () {
		if (NoChoices) return Descriptions[Descriptions.Count-1];
		if (Returning) return Descriptions[1];
		return Descriptions[0];
	}

	/// <summary>
	/// Selects a choice and unlocks an item if the choice triggers an unlockable.
	/// </summary>
	/// <param name="choice">The symbol of the choice.</param>
	/// <param name="context">The context of the unlockable (optional).</param>
	/// <param name="npc">The NPC that unlocked the item (optional).</param>
	public void SelectChoice (string choice, string context="", string npc="") {
		
		currentDialog = Choices.FirstOrDefault (x => x.Key == choice);
		Dialogue dialog = currentDialog.Value;
		
		if (dialog.unlocks != null) {

			string[] unlockableSymbols = dialog.unlocks;
			foreach (string symbol in unlockableSymbols) {
				Models.Unlockable unlockableRef = DataManager.GetUnlockableBySymbol(symbol);
				PlayerData.UnlockItem (symbol, context, npc);
			}
		}

		Choices.Remove (choice);
	}
}
