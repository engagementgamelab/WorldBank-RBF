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

 		// mainCam = Camera.main;
 		npcRef = npcData;

		Texture2D npcTex = Resources.Load("Textures/NPC/" + npcData.character, typeof(Texture2D)) as Texture2D;
 		GetComponent<Renderer>().material.mainTexture = npcTex;

 	}

 	// On Touch/Click NPC
	void OnMouseDown() {
		CameraBehavior.ZoomIn(transform);
		
		startDialog = true;
		cameraStartTime = 0;
	}

	void Update()
	{

		if(!startDialog) 
			return;

		if(cameraStartTime < cameraTime)
		    cameraStartTime += Time.deltaTime;
		else {
			DialogManager.instance.OpenCharacterDialog(npcRef, "Initial");	 
			startDialog = false;
		}

	}
}