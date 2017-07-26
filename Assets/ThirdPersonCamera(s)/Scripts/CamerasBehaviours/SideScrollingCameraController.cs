using System;
using AdvancedUtilities.Cameras.Components;
using UnityEngine;

namespace AdvancedUtilities.Cameras
{
    /// <summary>
    /// A camera controller for side scrolling.
    /// </summary>
    [Serializable]
    public class SideScrollingCameraController : CameraController
    {
        /// <summary>
        /// The distance that the camera wants to position itself at from the target.
        /// </summary>
        [Header("Settings")]
        [Tooltip("The distance that the camera wants to position itself at from the target.")]
        public float DesiredDistance = 20f;

        /// <summary>
        /// When the CameraController starts, horizontal rotation will be set to this value.
        /// </summary>
        [Tooltip("When the CameraController starts, horizontal rotation will be set to this value.")]
        public float InitialHorizontalRotation = 0f;

        /// <summary>
        /// When the CameraController starts, vertical rotation will be set to this value.
        /// </summary>
        [Tooltip("When the CameraController starts, vertical rotation will be set to this value.")]
        public float InitialVerticalRotation = 35f;

        #region Components

        /// <summary>
        /// TargetComponent
        /// </summary>
        [Header("Components")]
        public TargetComponent Target;
        /// <summary>
        /// RotationComponent
        /// </summary>
        public RotationComponent Rotation;
        /// <summary>
        /// ZoomComponent
        /// </summary>
        public ZoomComponent Zoom;
        /// <summary>
        /// ViewCollisionComponent
        /// </summary>
        public ViewCollisionComponent ViewCollision;
        /// <summary>
        /// HeadbobComponent
        /// </summary>
        public HeadbobComponent Headbob;
        /// <summary>
        /// ScreenShakeComponent
        /// </summary>
        public ScreenShakeComponent ScreenShake;

        #endregion

        /// <summary>
        /// The previous distance the camera was at during the last update.
        /// </summary>
        private float _previousDistance;

        protected override void AddCameraComponents()
        {
            AddCameraComponent(Rotation);
            AddCameraComponent(Zoom);
            AddCameraComponent(Target);
            AddCameraComponent(ViewCollision);
            AddCameraComponent(Headbob);
            AddCameraComponent(ScreenShake);
        }

        // Use this for initialization
        void Start()
        {
            // Set initial rotation and distance
            Rotation.Rotate(InitialHorizontalRotation, InitialVerticalRotation);
            _previousDistance = DesiredDistance;
        }

        // Update is called once per frame
        void Update()
        {
            UpdateCamera();

            CameraTransform.ApplyTo(Camera); // Apply the virtual transform to the actual transform
        }

        public override void UpdateCamera()
        {
            Rotation.UpdateAutoRotate();
            Rotation.CheckRotationDegreesEvents();

            // Apply target offset modifications
            Vector3 headbobOffset = Headbob.GetHeadbobModifier(_previousDistance);
            Target.AddWorldSpaceOffset(headbobOffset);
            Vector3 screenShakeOffset = ScreenShake.GetShaking();
            Target.AddWorldSpaceOffset(screenShakeOffset);
            
            // Get target
            Vector3 target = Target.GetTarget();
            float actual = _previousDistance;

            // Set Camera Position
            float desired = DesiredDistance; // Where we want the camera to be
            float calculated = ViewCollision.CalculateMaximumDistanceFromTarget(target, Mathf.Max(desired, actual)); // The maximum distance we calculated we can be based off collision and preference
            float zoom = Zoom.CalculateDistanceFromTarget(actual, calculated, desired); // Where we want to be for the sake of zooming

            CameraTransform.Position = target - CameraTransform.Forward * zoom; // Set the position of the transform

            _previousDistance = Target.GetDistanceFromTarget();
            Target.ClearAdditionalOffsets();
        }
    }
}
