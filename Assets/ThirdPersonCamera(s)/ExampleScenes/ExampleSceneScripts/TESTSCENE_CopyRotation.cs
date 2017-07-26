using UnityEngine;
using System.Collections;

public class TESTSCENE_CopyRotation : MonoBehaviour
{

    public Transform CopyFrom;
    
	// Update is called once per frame
	void Update ()
	{
	    this.transform.rotation = CopyFrom.rotation;
	}
}
