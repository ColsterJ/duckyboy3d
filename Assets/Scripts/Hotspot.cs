using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Hotspot : MonoBehaviour {
    public UnityEvent interact;
    public bool fovKick = false;
    public bool useCustomPopAmount = false;
    public bool useCustomPopDuration = false;
    public float customPopAmount = 15.0f;
    public float customPopDuration = 1.0f;
    [HideInInspector]
    public bool interacting = false;
    private FovPopper fp;
    private HotspotManager mgr;

    void Start()
    {
        if(fovKick)
            fp = GameObject.Find("Player Camera").GetComponent<FovPopper>();
        mgr = GameObject.Find("Hotspot Manager").GetComponent<HotspotManager>(); ;
    }

    public void Interact()
    {
        interacting = true;
        mgr.HideInteractPrompt();
        interact.Invoke();

        if (useCustomPopAmount)
            fp.SetFovPopAmount(customPopAmount);
        if (useCustomPopDuration)
            fp.SetFovPopDuration(customPopDuration);

        if(fp) fp.DoFovPopIn();
    }
    public void EndInteraction()
    {
        interacting = false;
        if(fp) fp.DoFovPopOut();
    }
}
