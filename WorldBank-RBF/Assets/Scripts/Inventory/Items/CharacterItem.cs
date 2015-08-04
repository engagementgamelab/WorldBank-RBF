using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Models;

public class CharacterItem : ModelItem {

	public string Symbol { get { 
		if (Model == null) {
			Debug.Log (Model + " does not have a symbol");
			return "";
		}
		return Model.symbol; } }
	public Character Model { get; private set; }
	public NPC Npc { get; private set; }
		
	Dictionary<string, Dialogue> choices;
	Dictionary<string, Dialogue> Choices {
		get {
			if (choices == null) {
				choices = new Dictionary<string, Dialogue> (
					Npc.dialogue, StringComparer.OrdinalIgnoreCase);
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

	bool Returning {
		get { return Choices.Count < Npc.dialogue.Count; }
	}

	bool NoChoices {
		get { return Choices.Count == 0; }
	}

	public CharacterItem () {}

	public CharacterItem (Character model) {
		this.Model = model;
		Debug.Log (Model.symbol);
		this.Npc = DataManager.GetNpc (Symbol);
	}

	public string GetDescription () {
		if (NoChoices) return Descriptions[Descriptions.Count-1];
		if (Returning) return Descriptions[1];
		return Descriptions[0];
	}

	/*public Dialogue GetInitialDialog () {
		return Choices["Initial"];
	}*/

	/*public string GetDialog (string dialogKey) {
		return Npc.GetDialogue (dialogKey);
	}*/
}
