using System;
using AdvancedUtilities.Cameras.Components;
using UnityEngine;

namespace AdvancedUtilities.Cameras
{
    /// <summary>
    /// A camera capable of locking onto a target and looking at through another target making it look like the camera is locked onto 
    /// the target from your character.
    /// </summary>
    [Serializable]
    public class LockTargetCameraController : CameraController
    {
        #region Public Fields

        /// <summary>
        /// The desired distance from the Target point by the Camera.
        /// </summary>
        [Tooltip("The desired distance from the Target point by the Camera.")]
        public float DesiredDistance = 10f;


        #region Components

        /// <summary>
        /// SwitchTargetComponent
        /// </summary>
        [Header("Components")]
        public SwitchTargetComponent LockTarget;
        /// <summary>
        /// MultipleRelativeTargetComponent
        /// </summary>
        public MultipleRelativeTargetComponent Target;
        /// <summary>
        /// ZoomComponent
        /// </summary>
        public ZoomComponent Zoom;
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
        [Tooltip("Distance will be based on the last update rather than the current update due to semantics.")]
        public HeadbobComponent Headbob;
        /// <summary>
        /// ScreenShakeComponent
        /// </summary>
        public ScreenShakeComponent ScreenShake;

        #endregion

        #endregion

        #region Public Properties and Private Fields

        /// <summary>
        /// The previous distance the camera was at during the last update.
        /// </summary>
        private float _previousDistance;

        #endregion

        protected override void AddCameraComponents()
        {
            AddCameraComponent(Zoom);
            AddCameraComponent(LockTarget);
            AddCameraComponent(Target);
            AddCameraComponent(ViewCollision);
            AddCameraComponent(Cursor);
            AddCameraComponent(Headbob);
            AddCameraComponent(ScreenShake);
        }

        void Start()
        {
            _previousDistance = float.MaxValue;
            UpdateCamera();
        }

        void Update()
        {
            UpdateCamera();

            CameraTransform.ApplyTo(Camera);
        }

        public override void UpdateCamera()
        {
            Vector3 lockTarget = LockTarget.GetTarget();
            
            Vector3 headbobOffset = Headbob.GetHeadbobModifier(_previousDistance);
            Target.AddWorldSpaceOffset(headbobOffset);
            Vector3 screenShakeOffset = ScreenShake.GetShaking();
            Target.AddWorldSpaceOffset(screenShakeOffset);

            Vector3 target = Target.GetTarget(lockTarget);

            Vector3 direction = lockTarget - target;
            
            float desired = DesiredDistance; // Where we want the camera to be
            float actual = Vector3.Distance(CameraTransform.Position, target); // Where the camera physically is right now
            float calculated = ViewCollision.CalculateMaximumDistanceFromTarget(target, Mathf.Max(desired, actual)); // The maximum distance we calculated we can be based off collision and preference
            float zoom = Zoom.CalculateDistanceFromTarget(actual, calculated, desired); // Where we want to be for the sake of zooming

            CameraTransform.Position = target - direction.normalized * zoom; // Set the position of the transform
            CameraTransform.LookAt(target);

            _previousDistance = Vector3.Distance(CameraTransform.Position, target);
            Target.ClearAdditionalOffsets();
            LockTarget.ClearAdditionalOffsets();
        }
    }
}
