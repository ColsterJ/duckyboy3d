using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AdvancedUtilities.Cameras;

/// <summary>
/// Purely for demonstration purposes of switching the camera's lock target.
/// </summary>
public class TESTSCENE_TargetSwitcher : MonoBehaviour
{
    public LockTargetCameraController Camera;

    public List<Transform> LockTargets;

    public KeyCode SwitchNext = KeyCode.RightArrow;
    public KeyCode SwitchPrevious = KeyCode.LeftArrow;

    private int _currentIndex;

    // Use this for initialization
    void Start ()
    {
        _currentIndex = 0;
        Camera.LockTarget.Target = LockTargets[_currentIndex];
    }
	
	// Update is called once per frame
	void Update ()
    {
	    if (Camera.LockTarget.IsSwitching)
	    {
            return;
	    }

	    if (Input.GetKey(SwitchNext))
	    {
	        _currentIndex++;
	        if (_currentIndex > LockTargets.Count - 1)
	        {
	            _currentIndex = 0;
	        }
	    }
        else if (Input.GetKey(SwitchPrevious))
        {
            _currentIndex--;
            if (_currentIndex < 0)
            {
                _currentIndex = LockTargets.Count - 1;
            }
        }

        Camera.LockTarget.Target = LockTargets[_currentIndex];
    }

}
