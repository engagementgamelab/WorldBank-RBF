using UnityEngine;
using System.Collections;

public class ParallaxImageDesigner : ScriptableObject {

	public EditorObjectDrawer<ParallaxImage> objectDrawer =
		new EditorObjectDrawer<ParallaxImage> ();

	public void OnGUI () {
		if (objectDrawer.Target == null) return;
		objectDrawer.DrawObjectProperties ();
		IEditorRefreshable refreshable = (IEditorRefreshable)objectDrawer.Target;
		refreshable.Refresh ();
	}
}
