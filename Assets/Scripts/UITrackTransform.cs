using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class UITrackTransform : MonoBehaviour {
    public Transform target;
    public Vector2 offset;
    private Vector3 newPos;

    // Update is called once per frame
    void LateUpdate () {
        newPos = Camera.main.WorldToScreenPoint(target.transform.position);
        transform.position = new Vector3(newPos.x + offset.x, newPos.y + offset.y, transform.position.z);
    }
}
