using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FovPopTrigger : MonoBehaviour {
    public bool useCustomPopAmount = false;
    public bool useCustomPopDuration = false;
    public float customPopAmount = 15.0f;
    public float customPopDuration = 1.0f;
    private GameObject playerCamObj;
    private FovPopper fp;
    private float orig_cam_fov;

    void Start()
    {
        GetComponent<MeshRenderer>().enabled = false;

        playerCamObj = GameObject.Find("Player Camera");
        fp = playerCamObj.GetComponent<FovPopper>();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if (useCustomPopAmount)
                fp.SetFovPopAmount(customPopAmount);
            if (useCustomPopDuration)
                fp.SetFovPopDuration(customPopDuration);
            fp.DoFovPopIn();
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            fp.DoFovPopOut();
        }
    }
}
