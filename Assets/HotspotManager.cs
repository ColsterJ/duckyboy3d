using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HotspotManager : MonoBehaviour {
    public Image interactPrompt;
    public Hotspot currentHotspot;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHotspot != null)
        {
            if (!currentHotspot.interacting)
            {
                if (Input.GetButtonDown("ActionA"))
                {
                    currentHotspot.Interact();
                }
            }
        }
	}
    public void ShowInteractPrompt()
    {
        interactPrompt.enabled = true;
    }
    public void HideInteractPrompt()
    {
        interactPrompt.enabled = false;
    }
}
