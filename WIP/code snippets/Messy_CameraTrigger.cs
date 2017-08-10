using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrigger : MonoBehaviour {
    public GameObject cameraToSwitchTo;
    public GameObject oldCamera;
    [Tooltip("Prevent player from getting 'camera change whiplash' by limiting changes to 1 per specified number of seconds")]
    public float triggerDelaySecs = 1.0f;
    private float delayTimer = 0.0f;
    private bool delayTimerTrigger = false, playerHere = false, playerWasHere = false;
    private GameObject playerObject;
    private Camera myCamera;

    void Start()
    {
        GetComponent<MeshRenderer>().enabled = false;
        cameraToSwitchTo.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            playerObject = other.gameObject;
            playerHere = true;
            if (delayTimer <= 0.0f)
            {
                SwitchToNewCamera();
                delayTimerTrigger = true;
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerObject = other.gameObject;
            playerHere = false;
            if (delayTimer <= 0.0f)
            {
                SwitchToOldCamera();
                delayTimerTrigger = true;
            }
        }
    }

    void SwitchToNewCamera()
    {
        oldCamera.SetActive(false);
        cameraToSwitchTo.SetActive(true);
        playerObject.GetComponent<PlayerMovement>().pivotTransform = cameraToSwitchTo.transform;
    }
    void SwitchToOldCamera()
    {
        oldCamera.SetActive(true);
        cameraToSwitchTo.SetActive(false);
        playerObject.GetComponent<PlayerMovement>().pivotTransform = oldCamera.transform;
    }

    void Update()
    {
        if (delayTimer > 0.0f)
            delayTimer -= Time.deltaTime;
        if (delayTimerTrigger)
        {
            delayTimerTrigger = false;
            delayTimer = triggerDelaySecs;
        }

        // If time passes and the camera wasn't changed (due to delayTimer), go ahead and change it
        if (delayTimer <= 0.0f)
        {
            if (playerHere)
            {
                if (!cameraToSwitchTo.activeSelf) SwitchToNewCamera();
            }
            else if (playerWasHere)
            {
                if (!oldCamera.activeSelf)      // TODO could introduce bugs if more than 1 possible camera swap happens in close proximity (i.e. 3 cameras competing)
                    SwitchToOldCamera();
            }
        }
    }
}
