using UnityEngine;
using System.Collections;

public class ParallaxZoomTriggerDesigner : ScriptableObject {

	public EditorObjectDrawer<ParallaxZoomTrigger> objectDrawer = 
        new EditorObjectDrawer<ParallaxZoomTrigger> ();

    public void OnGUI () {
    	if (objectDrawer.Target == null) return;
    	objectDrawer.DrawObjectProperties (new GUILayoutOption[0]);
    	if (GUILayout.Button ("Refresh")) {
    		objectDrawer.Target.Refresh ();
    	}
    }
}
