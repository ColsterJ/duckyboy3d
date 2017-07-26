using UnityEngine;
using System.Collections;

/// <summary>
/// Purely for demonstration purposes only to have objects moving around.
/// </summary>
public class TESTSCENE_RotatingObjects : MonoBehaviour
{

    public Transform RotatingPoint;
    public float Speed;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        float amount = Time.deltaTime * Speed;
	    this.transform.RotateAround(RotatingPoint.position, Vector3.up, amount);
	}
}
