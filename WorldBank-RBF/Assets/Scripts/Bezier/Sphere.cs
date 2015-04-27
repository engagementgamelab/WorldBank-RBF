/***************************************************************************************
 * Copyright 2010 Friction Point Studios Pty Ltd                                      
 ***************************************************************************************/

using UnityEngine;
using System.Collections;

public class Sphere : MonoBehaviour
{
    IBezierCurveManager bezierCurveManager;

    void Awake()
    {
        this.bezierCurveManager = GameObject.Find("BezierCurve").GetComponent(typeof(IBezierCurveManager)) as IBezierCurveManager;
    }

	public float TimeAhead = 0;
	
    void Update()
    {
        float time = Time.time;

        this.transform.position = this.bezierCurveManager.GetPositionAtTime(time + TimeAhead);
    }
}

