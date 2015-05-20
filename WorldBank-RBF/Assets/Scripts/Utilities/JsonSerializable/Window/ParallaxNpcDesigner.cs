using UnityEngine;
using System.Collections;

public class ParallaxNpcDesigner : ScriptableObject {

	public EditorObjectDrawer<ParallaxNpc> objectDrawer = 
        new EditorObjectDrawer<ParallaxNpc> ();

    public void OnGUI () {
    	if (objectDrawer.Target == null) return;
    	objectDrawer.DrawObjectProperties (new GUILayoutOption[0]);
    	if (GUILayout.Button ("Refresh")) {
    		objectDrawer.Target.Refresh ();
    	}
    }
}
