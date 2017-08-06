using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class NearestGroundDemo : MonoBehaviour {
    //NearestGround.cs is now located in Assets/Scripts/Utilities since the test was successful

    public Vector3 nearestGround;
    private bool foundGround = true;

    void Update()
    {
        foundGround = NearestGround.GetNearestGround(transform.position, out nearestGround);
    }

    void OnDrawGizmos()
    {
        if (foundGround)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, nearestGround);
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(nearestGround, 0.125f);
        } else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.down * 3);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position + Vector3.down * 3, 0.125f);
        }
    }
}
