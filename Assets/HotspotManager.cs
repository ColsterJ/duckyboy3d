using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HotspotManager : MonoBehaviour {
    public int gamepad_check_interval = 2;
    public Image interactPrompt_Gamepad, interactPrompt_Keyboard;
    [HideInInspector]
    public Image interactPrompt;
    public Hotspot currentHotspot;

    void Start()
    {
        interactPrompt = interactPrompt_Keyboard;
        StartCoroutine(CheckGamepadStatus_SlowLoop());  // If gamepad is connected, the loop will quickly determine that
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
    void FixedUpdate()
    {
    }
    public void ShowInteractPrompt()
    {
        interactPrompt.enabled = true;
    }
    public void HideInteractPrompt()
    {
        interactPrompt.enabled = false;
    }

    IEnumerator CheckGamepadStatus_SlowLoop()
    {
        while (enabled) // Only run this while the MonoBehavior is enabled; see http://answers.unity3d.com/questions/876960/how-to-make-waitforseconds-work-as-a-repeatingloop.html
        {
            string[] joyNames = Input.GetJoystickNames();
            if ((interactPrompt == interactPrompt_Keyboard) && (joyNames.Length > 0 && !string.IsNullOrEmpty(joyNames[0])))      // If user connects a gamepad, switch to gamepad prompts
            {
                interactPrompt = interactPrompt_Gamepad;
                interactPrompt.enabled = interactPrompt_Keyboard.enabled;
                interactPrompt_Keyboard.enabled = false;
            }
            else if ((interactPrompt == interactPrompt_Gamepad) && (joyNames.Length == 0 || string.IsNullOrEmpty(joyNames[0])))  // If user disconnects gamepad, switch back to keyboard prompts
            { // Checks not only joyNames.Length but also value of joyNames[0], because Unity doesn't get rid of joyNames[0] upon disconnect, it merely makes it an empty string
              // See http://answers.unity3d.com/questions/1100642/joystick-runtime-plugunplug-detection.html
                interactPrompt = interactPrompt_Keyboard;
                interactPrompt.enabled = interactPrompt_Gamepad.enabled;
                interactPrompt_Gamepad.enabled = false;
            }

            yield return new WaitForSeconds(gamepad_check_interval);
        }
    }
}
