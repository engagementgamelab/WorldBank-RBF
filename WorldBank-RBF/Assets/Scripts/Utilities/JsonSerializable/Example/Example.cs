using UnityEngine;
using System.Collections;

public class Example : MonoBehaviour {
    
    // [Range (0, 100), WindowField]
    // public float test = 50;

    [WindowField]
    public string hello = "world";

    [Regex (@"^(?:\d{1,3}\.){3}\d{1,3}$", "Invalid IP address!\nExample: '127.0.0.1'"), WindowField]
    public string serverAddress = "192.168.0.1";

    /*[Range (0, 100)]
    public float health = 100;


    public ScaledCurve range;
    public ScaledCurve falloff;

    [ContextMenu ("Do Something")]
    void DoSomething () {
        Debug.Log ("do something");
    }*/
}