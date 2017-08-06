using UnityEngine;
using System.Collections;

//public class that properly lerps between two positions
//used as delay for the camera follow target
public class Breathe : MonoBehaviour {
	Vector3 endPos, newPos;
	
	public float amplitude;
	public float period;
	public float smooth;

	private float distance;
	private float theta;

	public Vector3 moveDir;
	public Vector3 offset;
	public Transform target;

	void Awake() {
	}
	
	void FixedUpdate() {
		endPos = target.position + offset;
		theta = Time.deltaTime / period;
		distance = amplitude * Mathf.Sin(theta);
		moveDir = endPos - transform.position;

        transform.position += moveDir * distance;
	}
}