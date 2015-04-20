using UnityEngine;
using System.Collections;

public class CameraBehavior : MonoBehaviour {

	private static bool zoomIn;
	private static bool zoomOut;

	private Vector3 startingPosition;

	private static Transform zoomTarget;

	// Use this for initialization
	void Start () {

		startingPosition = transform.position;
	
	}

	void FixedUpdate() {

        if(zoomOut)
		{
			
			if(Mathf.Round(transform.position.z) == Mathf.Round(startingPosition.z))
				zoomOut = false;

    		transform.position = Vector3.Lerp(transform.position, startingPosition, 0.7f * Time.deltaTime);

		}
		else if(zoomIn)
		{
			Vector3 dest = zoomTarget.position;
			dest.z -= .7f;

    		// mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position, dest, step);
    		transform.position = Vector3.Lerp(transform.position, dest, 0.7f * Time.deltaTime);

    		if(transform.position.z >= 2.2f)    		
    			zoomIn = false;
		}

	}

	public static void ZoomIn(Transform target) {
		zoomTarget = target;
		zoomIn = true;
	}

	public static void ZoomOut() {
		zoomOut = true;
		zoomIn = false;
	}
}
