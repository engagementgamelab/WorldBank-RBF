/* 
World Bank RBF
Created by Engagement Lab, 2015
==============
 NPCBehavior.cs
 Unity NPC behavior handler.

 Created by Johnny Richardson on 4/20/15.
==============
*/

using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Timers;

public class NPCBehavior : MB {

	private Models.NPC npcRef;

	private Transform targetNPC;
 	private Timer aTimer;

 	private bool startDialog = false;
 	private float cameraStartTime = 0.7f;
 	private float cameraTime = 0.7f;

 	private Camera mainCam;

 	public void Initialize(Models.NPC npcData) {

 		mainCam = Camera.main;
 		npcRef = npcData;
	  
	    transform.localScale = Vector3.one;

	    // Temporary: set NPC position automatically
	    // .1f + (index/2)
	    transform.position = new Vector3(1, 0, mainCam.transform.position.z + 2);

		Texture2D npcTex = Resources.Load("Textures/NPC/" + npcData.character, typeof(Texture2D)) as Texture2D;
 		GetComponent<Renderer>().material.mainTexture = npcTex;

 	}

 	// On Touch/Click NPC
	void OnMouseDown() {
		/*CameraBehavior.ZoomIn(transform);
		mainCam.Camera().Move(10f);
		
		startDialog = true;
		cameraStartTime = 0;*/

		DialogManager.instance.OpenCharacterDialog(npcRef, "Initial");
	}

	/*void Update()
	{

		if(!startDialog) 
			return;

		if(cameraStartTime < cameraTime)
		    cameraStartTime += Time.deltaTime;
		else {
			DialogManager.instance.OpenCharacterDialog(npcRef, "Initial");	 
			startDialog = false;
		}

	}*/
}