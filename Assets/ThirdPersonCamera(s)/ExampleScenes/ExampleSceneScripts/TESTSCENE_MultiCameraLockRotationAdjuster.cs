using UnityEngine;
using AdvancedUtilities.Cameras;

/// <summary>
/// Helping demostrate how you can switch from lock settings easier.
/// </summary>
public class TESTSCENE_MultiCameraLockRotationAdjuster : MonoBehaviour
{
    public TESTSCENE_MultiCameraSwitcher Switcher;

    public LockTargetCameraController LockTarget;

    public BasicCameraController Basic;

    private CameraController _previous;

	// Use this for initialization
	void Start ()
	{
	    _previous = Switcher.Camera.CurrentCameraController;
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if (_previous == LockTarget && _previous != Switcher.Camera.CurrentCameraController && Input.GetKeyDown(Switcher.Swap))
	    {
	        Basic.Rotation.SetRotation(LockTarget.CameraTransform.Rotation);
	    }

        _previous = Switcher.Camera.CurrentCameraController;
    }
}
