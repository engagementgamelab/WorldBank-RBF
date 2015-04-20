using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Timers;

public class NPCBehavior : MB {

	public DataManager.NPC npcRef;
	public GameObject diagManager;

	private Transform targetNPC;
 	private Timer aTimer;

 	private bool startDialog = false;
 	private float cameraStartTime = 0.7f;
 	private float cameraTime = 0.7f;

 	public void Initialize(NPC npcData, GameObject manager) {



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
			ExecuteEvents.Execute<INPC>(diagManager, null, (x,y)=>x.OnNPCSelected(npcRef));	 
			startDialog = false;
		}
	}
}
