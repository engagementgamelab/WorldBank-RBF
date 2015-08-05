using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterGroup : ModelGroup<CharacterItem> {

	public override string ID { get { return "characters"; } }

	public CharacterItem this[string symbol] {
		get { 
			symbol = symbol.Replace ("dialogue_", "");
			try {
				return Characters.Find (x => x.Symbol == symbol); 
			} catch {
				throw new System.Exception ("No character with the symbol '" + symbol + "' exists.");
			}
		}
	}

	List<CharacterItem> characters;
	List<CharacterItem> Characters {
		get { 
			if (characters == null) {
				characters = Items.ConvertAll (x => (CharacterItem)x);
			}
			return characters;
		}
	}

	public CharacterGroup () {
		Models.Character[] characterModels = DataManager.GetAllCharacters ();
		foreach (Models.Character character in characterModels) {
			Add (new CharacterItem (character));
		}
	}
}
