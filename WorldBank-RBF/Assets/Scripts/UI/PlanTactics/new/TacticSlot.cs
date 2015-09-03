using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

// http://answers.unity3d.com/questions/865191/unity-new-ui-drag-and-drop.html

public class TacticSlot : MonoBehaviour, IDropHandler {

	 #region IDropHandler implementation
	 public void OnDrop (PointerEventData eventData) {
	 	Debug.Log ("heard");
	 }
	 #endregion
}
