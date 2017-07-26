using AdvancedUtilities.Cameras;
using UnityEngine;

/// <summary>
/// For testing purposes to demonstrate how a basic camera and a lock camera can work together.
/// </summary>
public class TESTSCENE_MultiCameraSwitcher : MonoBehaviour
{

    public MultiCameraController Camera;

    public KeyCode Swap = KeyCode.LeftShift;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if (Camera.IsSwitching)
	    {
            return;
	    }

	    if (Input.GetKeyDown(Swap))
	    {
	        Camera.CurrentIndex++;
	        if (Camera.CurrentIndex > Camera.CameraControllers.Count - 1)
	        {
	            Camera.CurrentIndex = 0;
	        }
	    }
	}
}
