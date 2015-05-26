using UnityEngine;
using System.Collections;

[JsonSerializable (typeof (Models.ParallaxNpc))]
public class ParallaxNpc : ParallaxElement, IClickable {

	public InputLayer[] IgnoreLayers { 
		get { return new InputLayer[] { InputLayer.UI }; }
	}

	[ExposeInWindow] public string symbol;
	[ExposeInWindow] public bool facingLeft = false;

	public override void Reset () {
		base.Reset ();
		symbol = "";
	}

	public void OnClick (ClickSettings clickSettings) {
		NPCFocusBehavior.Instance.OnClickNpc (this);
	}

	// lighting example
	/*void OnGUI () {
		if (Index != 0) return;
		if (GUILayout.Button ("fade out")) {
			ParallaxLayerLightController.Instance.FadeOutOtherLayers (gameObject);
		}	
		if (GUILayout.Button ("fade in")) {
			ParallaxLayerLightController.Instance.FadeInOtherLayers (gameObject);
		}
	}*/
}
