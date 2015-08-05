using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Contains unlockable dialogue
/// </summary>
public class DialogueGroup : ModelGroup<DialogueItem> {

	public override string ID { get { return "dialogues"; } }
	
	List<DialogueItem> unlockables;
	public List<DialogueItem> Unlockables {
		get { 
			if (unlockables == null) {
				unlockables = Items.ConvertAll (x => (DialogueItem)x);
			}
			return unlockables;
		}
	}

	public DialogueGroup () : base ("dialogue") {}
}
