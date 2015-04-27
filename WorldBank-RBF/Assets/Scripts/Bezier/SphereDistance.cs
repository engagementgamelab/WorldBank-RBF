/***************************************************************************************
 * Copyright 2010 Friction Point Studios Pty Ltd                                      
 ***************************************************************************************/

using UnityEngine;
using System.Collections;

public class SphereDistance : MonoBehaviour
{

    IBezierCurveManager bezierCurveManager;

    void Awake()
    {
        this.bezierCurveManager = GameObject.Find("BezierCurve").GetComponent(typeof(IBezierCurveManager)) as IBezierCurveManager;
    }

	public float DistanceAhead = 0;

    public float TimeAhead = 0;
	
    void Update()
    {
        float time = Time.time;

        this.transform.position = Vector3.Lerp(this.transform.position, this.bezierCurveManager.GetPositionAtDistance(DistanceAhead, time + TimeAhead), 0.2f);

    }
}

