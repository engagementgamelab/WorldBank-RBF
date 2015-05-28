using UnityEngine;
using System.Collections;

[JsonSerializable (typeof (Models.ParallaxNpc))]
public class ParallaxNpc : ParallaxElement, 	IClickable {

	public InputLayer[] IgnoreLayers { 
		get { return new InputLayer[] { InputLayer.UI }; }
	}

	[ExposeInWindow] public string symbol;
	[ExposeInWindow] public bool facingLeft = false;
	public bool FacingLeft {
		get { return facingLeft; }
	}

	public override void Reset () {
		base.Reset ();
		symbol = "";
	}

	public void OnClick (ClickSettings clickSettings) {
		//TODO: pause a couple frames & check for drag
		NPCFocusBehavior.Instance.OnClickNpc (this);
	}
}
