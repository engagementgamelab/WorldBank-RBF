using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor (typeof (AmbienceZone))]
public class AmbienceZoneEditor : MyCustomEditor<AmbienceZone> {

	protected override void Draw () {
		DrawStringProperty ("context");
		DrawFloatProperty ("width");
		DrawFloatProperty ("offset");
		DrawFloatProperty ("fadeLength");
		if (GUILayout.Button ("Destroy this zone")) {
			ObjectPool.Destroy<AmbienceZone> (Target.Transform);
		}
	}
}
