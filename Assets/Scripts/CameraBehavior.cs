using UnityEngine;
using System.Collections;

public class CameraBehavior : MonoBehaviour {

    [SerializeField] private LayerMask raycastMask;

    private float m_LookPosZ;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 20);
		Debug.DrawRay(transform.position, (Quaternion.AngleAxis(-15, new Vector3(0, 1, 0)) * transform.forward) * 20);

		bool hitObject = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), 20, raycastMask) 
					     ||
					     Physics.Raycast(transform.position, Quaternion.AngleAxis(30, transform.up) * transform.forward, 20, raycastMask) 
					     ||
					     Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), 20, raycastMask);

        if(hitObject)
            m_LookPosZ = -12.977f;
        else
            m_LookPosZ = -11.877f;

        var newVector = new Vector3(transform.position.x, transform.position.y, m_LookPosZ);

        transform.position = Vector3.Lerp(transform.position, newVector, 2 * Time.deltaTime);

	}
}
