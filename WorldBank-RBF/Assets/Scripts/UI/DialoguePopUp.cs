using UnityEngine;
using System.Collections;

public class DialoguePopUp : MonoBehaviour {

	public Transform boxPosition;
	public GameObject dialogueBox;
	//public GameObject npc;
	//Vector3 npcPosition;
	Vector3 rightPosition;
	//Vector3 leftPosition;

	//is the npc on the left?
	public bool setLeft;


	public void Open (bool left) {

		Camera camera = Camera.main;
		//position the box to the left
		Vector3 leftPosition = camera.ViewportToWorldPoint(new Vector3(.1f, .5f, 0f));
		//position the box to the right
		Vector3 rightPosition = camera.ViewportToWorldPoint(new Vector3(.9f, .5f, 0f));


		//is the npc on the left?
		if (left == true) {
			//position box on the right
			dialogueBox.transform.localPosition = rightPosition;
		} else {
			//position box on the left
			dialogueBox.transform.localPosition = leftPosition;
		}
	}


	// Use this for initialization
	void Start () {
		//npcPosition = npc.transform.position.x;

	}
	
	// Update is called once per frame
	void Update () {
		Open (setLeft);
		
	}
}
