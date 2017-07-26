using System;
using AdvancedUtilities.Cameras.Components;
using UnityEngine;

namespace AdvancedUtilities.Cameras
{
    /// <summary>
    /// A camera controller that can hold a position and look at a target.
    /// </summary>
    [Serializable]
    public class HoldPositionCameraControllerOldV1 : CameraController
    {
        /// <summary>
        /// This is the position that the Holding Camera will be located when looking at the target.
        /// </summary>
        [Header("Settings")]
        [Tooltip("This is the position that the Holding Camera will be located when looking at the target.")]
        public Transform HoldingPosition;

        /// <summary>
        /// TargetComponent
        /// </summary>
        [Header("Components")]
        public TargetComponent Target;
        /// <summary>
        /// ZoomComponentOldV1
        /// </summary>
        public ZoomComponentOldV1 Zoom;
        /// <summary>
        /// ViewCollisionComponent
        /// </summary>
        public ViewCollisionComponent ViewCollision;
        /// <summary>
        /// CursorComponent
        /// </summary>
        public CursorComponent Cursor;
        /// <summary>
        /// HeadbobComponent
        /// </summary>
        public HeadbobComponent Headbob;
        /// <summary>
        /// ScreenShakeComponent
        /// </summary>
        public ScreenShakeComponent ScreenShake;

        /// <summary>
        /// The distance from the target in the last update.
        /// </summary>
        private float _previousDistance;

        protected override void AddCameraComponents()
        {
            AddCameraComponent(Zoom);
            AddCameraComponent(Target);
            AddCameraComponent(ViewCollision);
            AddCameraComponent(Cursor);
            AddCameraComponent(Headbob);
            AddCameraComponent(ScreenShake);
        }

        void Start()
        {
            CameraTransform.Position = HoldingPosition.position;
            CameraTransform.LookAt(Target.GetTarget());
            _previousDistance = Target.GetDistanceFromTarget();
        }

        void Update()
        {
            UpdateCamera();

            CameraTransform.ApplyTo(Camera); // Apply the virtual transform to the actual transform
        }

        public override void UpdateCamera()
        {
            // Apply target offset modifications
            Vector3 headbobOffset = Headbob.GetHeadbobModifier(_previousDistance);
            Target.AddWorldSpaceOffset(headbobOffset);
            Vector3 screenShakeOffset = ScreenShake.GetShaking();
            Target.AddWorldSpaceOffset(screenShakeOffset);

            // Handle Cursor
            Cursor.SetCursorLock();

            // Get target
            Vector3 target = Target.GetTarget();

            float actualDistance = Target.GetDistanceFromTarget();

            // -- Set camera position and rotation
            CameraTransform.Position = HoldingPosition.position; // First move it to the holding position
            CameraTransform.LookAt(target); // Look at the target from the perspective of the holding position. Now we have the proper rotation in the virtual transform.
            
            // Set Camera Position
            float desiredDistance = Vector3.Distance(target, HoldingPosition.position); // The distance between the hold position and the target
            float calculatedDistance = ViewCollision.CalculateMaximumDistanceFromTarget(target, Mathf.Max(desiredDistance, actualDistance)); // The maximum distance we calculated we can be based off collision and preference
            float zoom = Zoom.CalculateDistanceFromTarget(actualDistance, calculatedDistance, desiredDistance); // Where we want to be for the sake of zooming

            CameraTransform.Position = target - CameraTransform.Forward * zoom; // Set the position of the transform

            _previousDistance = Target.GetDistanceFromTarget();
            Target.ClearAdditionalOffsets();
        }
    }
}
