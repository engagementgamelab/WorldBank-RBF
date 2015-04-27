/***************************************************************************************
 * Copyright 2010 Friction Point Studios Pty Ltd                                      
 ***************************************************************************************/

using UnityEngine;
using System.Collections;

public class FastSphere : MonoBehaviour
{
    IBezierCurveManager bezierCurveManager;

    void Awake()
    {
        this.bezierCurveManager = GameObject.Find("BezierCurve").GetComponent(typeof(IBezierCurveManager)) as IBezierCurveManager;
    }

	public float TimeAhead = 0;

    public float Speed = 5;
	
    void Update()
    {
        float time = Time.time  * Speed;

        Vector3 newDir = Vector3.RotateTowards(transform.forward, this.bezierCurveManager.GetNextPositionAtTime(time + TimeAhead), Time.deltaTime * Speed, 0.0F);

        this.transform.position = this.bezierCurveManager.GetPositionAtTime(time + TimeAhead);
        this.transform.localRotation = Quaternion.LookRotation(newDir);
    }

}