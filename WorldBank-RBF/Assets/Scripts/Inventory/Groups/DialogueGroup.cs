using UnityEngine;
using System.Collections;

/// <summary>
/// Contains unlockable dialogue (*TODO: THIS IS INCOMPLETE*)
/// </summary>
public class DialogueGroup : ModelGroup<DialogueItem> {

	public override string ID { get { return "dialogues"; } }
	
	public DialogueGroup () : base ("dialogue") {}
}
