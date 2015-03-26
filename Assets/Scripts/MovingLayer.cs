using System;
using UnityEngine;

public class MovingLayer : MonoBehaviour
{
    // This script is designed to be placed on the root object of a camera rig,
    // comprising 3 gameobjects, each parented to the next:

    // 	Camera Rig
    // 		Pivot
    // 			Camera

    [Range(0f, 10f)] [SerializeField] private float m_TurnSpeed = 1.5f;   // How fast the rig will rotate from user input.
    [SerializeField] private float flBackgroundSpeed = 0.1f;                // How much smoothing to apply to the turn input, to reduce mouse-turn jerkiness
    [SerializeField] private float flMiddleSpeed = 0.4f;                // How much smoothing to apply to the turn input, to reduce mouse-turn jerkiness
    [SerializeField] private float flForegroundSpeed = 0.8f;

    private float m_LookPosX;
    private float m_TiltAngle;                    // The pivot's x axis rotation.
    private const float k_LookDistance = 100f;    // How far in front of the pivot the character's look target is.
    private float flCurrentSpeed; 
	private Vector3 m_PivotEulers;
	private Quaternion m_PivotTargetRot;
	private Quaternion m_TransformTargetRot;

    private enum Layers { Background, Middle, Foreground };
    private Layers currentLayer;

    private void Awake()
    {
        m_LookPosX = transform.position.x;

        if(gameObject.tag == "Background")
            flCurrentSpeed = flBackgroundSpeed;

        else if(gameObject.tag == "Middle")
            flCurrentSpeed = flMiddleSpeed;

        else if(gameObject.tag == "Foreground")
            flCurrentSpeed  = flForegroundSpeed;
    }

    protected void Update()
    {
        HandleRotationMovement();
    }

    private void HandleRotationMovement()
    {
		if(Time.timeScale < float.Epsilon)
		return;

        // Read the user input
        var leftkey = Input.GetKey(KeyCode.LeftArrow);
        var rightkey = Input.GetKey(KeyCode.RightArrow);

        // Adjust the look angle by an amount proportional to the turn speed and horizontal input.
        if(leftkey)
            m_LookPosX -= m_TurnSpeed;
        else if(rightkey)
            m_LookPosX += m_TurnSpeed;

        var newVector = new Vector3(m_LookPosX, transform.position.y, transform.position.z);

        transform.position = Vector3.Lerp(transform.position, newVector, flCurrentSpeed * Time.deltaTime);
    }
}
