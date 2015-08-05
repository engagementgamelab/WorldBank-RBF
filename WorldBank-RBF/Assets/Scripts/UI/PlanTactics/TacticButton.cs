using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TacticButton : MB {
	protected Column column;
	protected Transform contentParent;

	public virtual void Init (Column column, Transform contentParent, string content) {
		this.column = column;
		this.contentParent = contentParent;
		SetParent (contentParent);
	}

	public virtual void OnClick () {}

	protected void SetParent (Transform parent) {
		Parent = parent;
		Transform.Reset ();
	}
}
