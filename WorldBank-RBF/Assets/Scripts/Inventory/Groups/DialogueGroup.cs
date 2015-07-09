using UnityEngine;
using System.Collections;

public class DialogueGroup : ModelGroup<DialogueItem> {

	public override string ID { get { return "dialogues"; } }
	
	public DialogueGroup () : base ("dialogue") {}
}
