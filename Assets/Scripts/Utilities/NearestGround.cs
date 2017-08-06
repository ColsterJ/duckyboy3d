using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NearestGround {
    public static bool GetNearestGround(Vector3 from, out Vector3 nearestGround)
    {
        Ray ray;
        RaycastHit hit;

        ray = new Ray(from, Vector3.down);

        if (Physics.Raycast(ray, out hit))
        {
            nearestGround = hit.point;
            return true;
        }
        else
        {
            nearestGround = Vector3.zero;
            return false;
        }
    }
}
