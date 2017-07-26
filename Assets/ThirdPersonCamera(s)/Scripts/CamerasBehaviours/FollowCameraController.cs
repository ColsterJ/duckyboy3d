using System;
using AdvancedUtilities.Cameras.Components;
using UnityEngine;

namespace AdvancedUtilities.Cameras
{
    [Serializable]
    public class FollowCameraController : CameraController
    {
        /// <summary>
        /// The distance that the camera wants to position itself at from the target.
        /// </summary>
        [Header("Settings")]
        [Tooltip("The distance that the camera wants to position itself at from the target.")]
        public float DesiredDistance;

        /// <summary>
        /// The minimum that zooming will let you zoom in to.
        /// </summary>
        [Tooltip("The minimum that zooming will let you zoom in to.")]
        public float MinZoomDistance = 0f;

        /// <summary>
        /// The maximum that zooming will let you zoom out to.
        /// </summary>
        [Tooltip("The maximum that zooming will let you zoom out to.")]
        public float MaxZoomDistance = 50f;

        #region Components
        /// <summary>
        /// TargetComponent
        /// </summary>
        [Header("Components")]
        public TargetComponent Target;
        /// <summary>
        /// FollowRotationComponent
        /// </summary>
        public FollowRotationComponent FollowRotation;
        /// <summary>
        /// ZoomComponent
        /// </summary>
        public ZoomComponent Zoom;
        /// <summary>
        /// ViewCollisionComponent
        /// </summary>
        public ViewCollisionComponent ViewCollision;
        /// <summary>
        /// InputComponent
        /// </summary>
        public InputComponent Input;
        /// <summary>
        /// EasyUnityInputComponent
        /// </summary>
        public EasyUnityInputComponent EasyUnityInput;
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
            AddCameraComponent(Zoom);
            AddCameraComponent(Target);
            AddCameraComponent(FollowRotation);
            AddCameraComponent(ViewCollision);
            AddCameraComponent(Headbob);
            AddCameraComponent(ScreenShake);
            AddCameraComponent(Input);
            AddCameraComponent(EasyUnityInput);
        }

        void Start()
        {
            _previousDistance = DesiredDistance;
            
            UpdateCamera();
        }

        void Update()
        {
            UpdateCamera();

            CameraTransform.ApplyTo(Camera); // Apply the virtual transform to the actual transform
        }

        public override void UpdateCamera()
        {
            // Get Input
            EasyUnityInput.AppendInput();
            InputValues input = Input.ProcessedInput;
            Input.ClearInput();

            // Handle the rotating to follow the target from behind.
            FollowRotation.UpdateRotation();
            
            // Apply target offset modifications
            Vector3 headbobOffset = Headbob.GetHeadbobModifier(_previousDistance);
            Target.AddWorldSpaceOffset(headbobOffset);
            Vector3 screenShakeOffset = ScreenShake.GetShaking();
            Target.AddWorldSpaceOffset(screenShakeOffset);

            Vector3 target = Target.GetTarget();
            
            // Hanlde Zooming
            if (input.ZoomIn.HasValue)
            {
                DesiredDistance = Mathf.Max(DesiredDistance + input.ZoomIn.Value, 0);
                DesiredDistance = Mathf.Max(DesiredDistance, MinZoomDistance);
            }
            if (input.ZoomOut.HasValue)
            {
                DesiredDistance = Mathf.Min(DesiredDistance + input.ZoomOut.Value, MaxZoomDistance);
            }

            // Set Camera Position
            float desired = DesiredDistance; // Where we want the camera to be
            float calculated = ViewCollision.CalculateMaximumDistanceFromTarget(target, Mathf.Max(desired, _previousDistance)); // The maximum distance we calculated we can be based off collision and preference
            float zoom = Zoom.CalculateDistanceFromTarget(_previousDistance, calculated, desired); // Where we want to be for the sake of zooming

            Vector3 zoomDistance = CameraTransform.Forward * zoom;

            CameraTransform.Position = target - zoomDistance; // No buffer if the buffer would zoom us in past 0.

            float actual = Vector3.Distance(CameraTransform.Position, target);

            _previousDistance = actual;
            Target.ClearAdditionalOffsets();
        }
    }
}
