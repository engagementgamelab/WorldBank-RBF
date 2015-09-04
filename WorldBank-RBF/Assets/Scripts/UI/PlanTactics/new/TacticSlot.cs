using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// http://answers.unity3d.com/questions/865191/unity-new-ui-drag-and-drop.html

public class TacticSlot : MonoBehaviour, IDropHandler {

	public Text text;

	#region IDropHandler implementation
	public void OnDrop (PointerEventData eventData) {
		if (Tactic.selected.Item != null)
		 	text.text = Tactic.selected.Item.Title;
	 	else
	 		text.text = "test";
	}
	#endregion
}
