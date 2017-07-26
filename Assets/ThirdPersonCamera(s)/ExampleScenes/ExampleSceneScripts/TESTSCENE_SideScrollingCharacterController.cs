using UnityEngine;
using System.Collections;
using AdvancedUtilities.Cameras;

public class TESTSCENE_SideScrollingCharacterController : MonoBehaviour
{
    /// <summary>
    /// Going to rotate the camera when we enter here
    /// </summary>
    public GameObject TurningPoint;

    /// <summary>
    /// The camera controller for the scene
    /// </summary>
    public SideScrollingCameraController CameraController;

    public float JumpPower = 10;

    public float Speed = 5;

    private Rigidbody _body;

	// Use this for initialization
	void Start ()
	{
	    _body = GetComponent<Rigidbody>();
	}

    void OnTriggerEnter(Collider other)
    {

    }

    // Update is called once per frame
    void FixedUpdate ()
    {
        float horizontal = 0;
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            horizontal = 1;
        }
        else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            horizontal = -1;
        }
        bool jump = Input.GetButton("Jump") || Input.GetKey(KeyCode.W);

        // Jumping
        if (jump && IsGrounded())
        {
            _body.velocity = new Vector3(_body.velocity.x,
                JumpPower,
                _body.velocity.z);
        }

        _body.velocity = new Vector3(horizontal * Speed,
                _body.velocity.y,
                _body.velocity.z);
    }

    bool IsGrounded()
    {
        RaycastHit hit;
        if (Physics.Raycast(this.transform.position, Vector3.down, out hit, 0.52f))
        {
            return true;
        }

        return false;
    }
}
