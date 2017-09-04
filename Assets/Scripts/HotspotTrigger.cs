using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotspotTrigger : MonoBehaviour {
    public Hotspot hotspot;
    private Hotspot last = null;
    private HotspotManager mgr;

    void Start()
    {
        GetComponent<MeshRenderer>().enabled = false;
        mgr = GameObject.Find("Hotspot Manager").GetComponent<HotspotManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if (mgr.currentHotspot != null)
                last = mgr.currentHotspot;
            mgr.currentHotspot = hotspot;
            mgr.ShowInteractPrompt();
        }
    }
    void OnTriggerStay(Collider other)
    {
        if (!hotspot.interacting && !mgr.interactPrompt.enabled)
            mgr.ShowInteractPrompt();
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            if (last != null)
                mgr.currentHotspot = last;
            else
                mgr.currentHotspot = null;
            mgr.HideInteractPrompt();
        }
    }
    public void StopUsingHotspot()
    {
        if (last != null)
            mgr.currentHotspot = last;
        else
            mgr.currentHotspot = null;
        mgr.HideInteractPrompt();
        GetComponent<BoxCollider>().enabled = false;
    }
}
