using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrigger : MonoBehaviour {
    public GameObject cameraToSwitchTo;
    private GameObject originalCameraObject;
    private Camera myCamera;

    void Start()
    {
        GetComponent<MeshRenderer>().enabled = false;
        originalCameraObject = Camera.main.gameObject;
        cameraToSwitchTo.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            originalCameraObject.SetActive(false);
            cameraToSwitchTo.SetActive(true);
            other.gameObject.GetComponent<PlayerMovement>().pivotTransform = cameraToSwitchTo.transform;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            originalCameraObject.SetActive(true);
            cameraToSwitchTo.SetActive(false);
            other.gameObject.GetComponent<PlayerMovement>().pivotTransform = originalCameraObject.transform;
        }
    }
}
