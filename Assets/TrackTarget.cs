using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackTarget : MonoBehaviour {
    public Transform target;
    private Vector3 lastPos;

	// Use this for initialization
	void Start () {
        lastPos = target.position;
	}
	
	void LateUpdate () {
        transform.position += (target.position - lastPos);
        lastPos = target.position;
	}
}
