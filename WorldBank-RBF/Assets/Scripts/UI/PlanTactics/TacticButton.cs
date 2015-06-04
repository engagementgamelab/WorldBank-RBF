using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TacticButton : MB {

	Text text;
	protected Text Text {
		get {
			if (text == null) {
				text = Transform.GetChild (0).GetComponent<Text> ();
			}
			return text;
		}
	}

	protected Column column;
	protected Transform contentParent;

	public virtual void Init (Column column, Transform contentParent, string content) {
		this.column = column;
		this.contentParent = contentParent;
		SetParent (contentParent);
		Text.text = content;
	}

	public virtual void OnClick () {}

	protected void SetParent (Transform parent) {
		Parent = parent;
		Transform.Reset ();
	}
}
