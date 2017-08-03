using System;
using System.Collections;
using System.Collections.Generic;
using AdvancedUtilities;
using UnityEngine;
using AdvancedUtilities.Cameras;

public class TESTSCENE_CharController : MonoBehaviour
{
    public LayerMask Ground = -1;
    public BasicCameraController CameraController;
    public Rigidbody Rigidbody;
    public Collider Collider;
    public Transform Transform;
    public float DistanceToTheGround;

    [Serializable]
    public class CharacterControllerSettings
    {
        public float Slope = 50;

        public float Speed = 5;

        public float JumpPower = 5;

        public float GroundCheckDistance = 0.1f;
    }
    public CharacterControllerSettings Settings = new CharacterControllerSettings();

    void Start ()
    {
        DistanceToTheGround = Collider.bounds.extents.y;
    }
    
    void UpdateLookingDirection()
    {
        // Notice that I have to unattach the camera
        // That's because the camera is already rotated.
        // If I don't unattach it, the entire camera will rotate with the object as it rotates.
        Transform camParent = CameraController.Camera.transform.parent;
        CameraController.Camera.transform.parent = null;
        Vector3 forward = Flatten(CameraController.CameraTransform.Forward);
        VirtualTransform vt = new VirtualTransform();
        vt.LookAt(forward);
        Transform.rotation = vt.Rotation;
        CameraController.Camera.transform.parent = camParent;
    }

	void FixedUpdate ()
	{
        Vector3 movement = GetMovementVector();
        bool grounded = IsGrounded();

        UpdateLookingDirection();

        if (grounded)
        {
            Rigidbody.MovePosition(Transform.position + movement);

            if (Input.GetButton("Jump"))
            {
                Rigidbody.velocity = new Vector3(Rigidbody.velocity.x, Rigidbody.velocity.y - Settings.JumpPower, Rigidbody.velocity.z);
            }
        }
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(Transform.position - Vector3.up * 0.05f, -Vector3.up, DistanceToTheGround + Settings.GroundCheckDistance, Ground.value);
    }

    private Vector3 GetMovementVector()
    {
        float horizontal = Input.GetKey(KeyCode.A) ? -1 : Input.GetKey(KeyCode.D) ? 1 : 0;//.GetAxis("Horizontal");
        float vertical = Input.GetKey(KeyCode.S) ? -1 : Input.GetKey(KeyCode.W) ? 1 : 0;//.GetAxis("Vertical");
		
        Vector3 forward = Flatten(CameraController.CameraTransform.Forward);
        Vector3 right = Flatten(CameraController.CameraTransform.Right);

        forward *= vertical;
        right *= horizontal;

        Vector3 combined = forward + right;

        if (combined.magnitude > 1)
        {
            combined = combined.normalized;
        }

        combined *= Settings.Speed;
        combined *= Time.deltaTime;

        return combined;
    }

    private Vector3 Flatten(Vector3 vector)
    {
        return new Vector3(vector.x, 0, vector.z).normalized;
    }
}
