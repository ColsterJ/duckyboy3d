using System;
using UnityEngine;
using System.Collections.Generic;
using AdvancedUtilities.Cameras.Components.Events;
using AdvancedUtilities.LerpTransformers;

namespace AdvancedUtilities.Cameras.Components
{
    /// <summary>
    /// A component that can rotate the view of a camera.
    /// </summary>
    [Serializable]
    public class RotationComponent : CameraComponent
    {
        #region Public Fields

        /// <summary>
        /// Whether or not the RotationComponent will do anything when called.
        /// </summary>
        [Tooltip("Whether or not the RotationComponent will do anything when called.")]
        public bool Enabled = true;

        /// <summary>
        /// Encapsulates the rotation limits for the RotationComponent.
        /// </summary>
        [Serializable]
        public class RotationLimits
        {
            /// <summary>
            /// Limits for vertical rotation will be enforced.
            /// </summary>
            [Tooltip("Limits for vertical rotation will be enforced.")]
            public bool EnableVerticalLimits = true;

            /// <summary>
            /// The limit that the Camera can rotate vertically above the target. Generally this should be a positive value.
            /// </summary>
            [Tooltip("The limit that the Camera can rotate vertically above the target. Generally this should be a positive value.")]
            public float VerticalUp = 90f;

            /// <summary>
            /// The limit that the Camera can rotate vertically below the target. Generally this should be a negative value.
            /// </summary>
            [Tooltip("The limit that the Camera can rotate vertically below the target. Generally this should be a negative value.")]
            public float VerticalDown = -90f;

            /// <summary>
            /// Limits for horizontal rotation will be enforced.
            /// </summary>
            [Tooltip("Limits for horizontal rotation will be enforced.")]
            public bool EnableHorizontalLimits = false;

            /// <summary>
            /// The limit that the Camera can rotate horizontally to the left of the target. Generally this should be a positive value.
            /// </summary>
            [Tooltip("The limit that the Camera can rotate horizontally to the left of the target. Generally this should be a positive value.")]
            public float HorizontalLeft = 90f;

            /// <summary>
            /// The limit that the Camera can rotate horizontally to the right of the target. Generally this should be a negative value.
            /// </summary>
            [Tooltip("The limit that the Camera can rotate horizontally to the right of the target. Generally this should be a negative value.")]
            public float HorizontalRight = -90f;
        }
        /// <summary>
        /// The settings for rotation limits for this RotationComponent.
        /// Limits the amount you can rotate in any direction based off the orientation.
        /// </summary>
        [Tooltip("Limits the amount you can rotate in any direction based off the orientation.")]
        public RotationLimits Limits = new RotationLimits();

        /// <summary>
        /// Encapsulates information on horizontal rotation degrees events.
        /// </summary>
        [Serializable]
        public class RotationHorizontalDegreesEvent
        {
            /// <summary>
            /// Enables the rotation component to fire events every time the horizontal has rotated a given amount of degrees in either direction.
            /// </summary>
            [Tooltip("Enables the rotation component to fire events every time the horizontal has rotated a given amount of degrees in either direction.")]
            public bool Enabled = false;

            /// <summary>
            /// Everytime the camera rotates the given amount in degrees, it will fire an event to all listeners registered to it with the amount it rotated.
            /// </summary>
            [Tooltip("Everytime the camera rotates the given amount in degrees, it will fire an event to all listeners registered to it with the amount it rotated.")]
            public float DegreesTrigger = 90f;

            /// <summary>
            /// If true, everytime the event is fired, it will reset tracking during that update to having rotated 0 degrees.
            /// If set to false, if you rotate 135 degrees and fire an event every 90 degrees, then you will fire an event and be left with 45 degrees.
            /// Technically multiple events can fire in a single update if this is set to false you rotate in a multiple of your event degrees value.
            /// Only really useful if you have a low trigger or massive rotations.
            /// </summary>
            [Tooltip("If true, everytime the event is fired, it will reset tracking during that update to having rotated 0 degrees. " +
                     "If set to false, if you rotate 135 degrees and fire an event every 90 degrees, then you will fire an event and be left with 45 degrees. " +
                     "Technically multiple events can fire in a single update if this is set to false you rotate in a multiple of your event degrees value. " +
                     "Only really useful if you have a low trigger or massive rotations.")]
            public bool ResetTotalAfterEachEvent = false;

            /// <summary>
            /// A list of all the rotation listeners currently listening for rotation events.
            /// </summary>
            public IList<HorizontalDegreesListener> RotationEventListeners;
        }
        /// <summary>
        /// The settings for horizontal rotation events for this RotationComponent.
        /// The camera controller has the ability to fire an event when you rotate it on the horizontal direction.
        /// </summary>
        [Tooltip("The camera controller has the ability to fire an event when you rotate it on the horizontal direction.")]
        public RotationHorizontalDegreesEvent HorizontalDegreesEvent = new RotationHorizontalDegreesEvent();

        /// <summary>
        /// Encapsulates information on vertical rotation degrees events.
        /// </summary>
        [Serializable]
        public class RotationVerticalDegreesEvent
        {
            /// <summary>
            /// Enables the rotation component to fire events every time the vertical has rotated a given amount of degrees in either direction.
            /// </summary>
            [Tooltip("Enables the rotation component to fire events every time the vertical has rotated a given amount of degrees in either direction.")]
            public bool Enabled = false;

            /// <summary>
            /// Everytime the camera rotates the given amount in degrees, it will fire an event to all listeners registered to it with the amount it rotated.
            /// </summary>
            [Tooltip("Everytime the camera rotates the given amount in degrees, it will fire an event to all listeners registered to it with the amount it rotated.")]
            public float DegreesTrigger = 90f;

            /// <summary>
            /// If true, everytime the event is fired, it will reset tracking during that update to having rotated 0 degrees.
            /// If set to false, if you rotate 135 degrees and fire an event every 90 degrees, then you will fire an event and be left with 45 degrees.
            /// Technically multiple events can fire in a single update if this is set to false you rotate in a multiple of your event degrees value.
            /// Only really useful if you have a low trigger or massive rotations.
            /// </summary>
            [Tooltip("If true, everytime the event is fired, it will reset tracking during that update to having rotated 0 degrees. " +
                     "If set to false, if you rotate 135 degrees and fire an event every 90 degrees, then you will fire an event and be left with 45 degrees. " +
                     "Technically multiple events can fire in a single update if this is set to false you rotate in a multiple of your event degrees value. " +
                     "Only really useful if you have a low trigger or massive rotations.")]
            public bool ResetTotalAfterEachEvent = false;

            /// <summary>
            /// A list of all the rotation listeners currently listening for rotation events.
            /// </summary>
            public IList<VerticalDegreesListener> RotationEventListeners;
        }
        /// <summary>
        /// The settings for vertical rotation events for this RotationComponent.
        /// The camera controller has the ability to fire an event when you rotate it on the vertical direction.
        /// </summary>
        [Tooltip("The camera controller has the ability to fire an event when you rotate it on the vertical direction.")]
        public RotationVerticalDegreesEvent VerticalDegreesEvent = new RotationVerticalDegreesEvent();

        /// <summary>
        /// Encapsulates information related to automatically rotating the camera over a period of time.
        /// </summary>
        [Serializable]
        public class RotationAutoRotate
        {
            /// <summary>
            /// Whether or not the auto rotation will actually happen.
            /// </summary>
            [Tooltip("Whether or not the auto rotation will actually happen.")]
            public bool Enabled = true;

            /// <summary>
            /// When the component is automatically rotating, the camera will not be able to be rotated with this component from external method calls.
            /// </summary>
            [Tooltip("When the component is automatically rotating, the camera will not be able to be rotated with this component from external method calls.")]
            public bool DisableExternalRotationWhileAutoRotating = true;

            /// <summary>
            /// If set to true, then when you call auto rotate, it will begin performing a new auto rotation instead of doing nothing and waiting for the first one to complete.
            /// </summary>
            [Tooltip("If set to true, then when you call auto rotate, it will begin performing a new auto rotation instead of doing nothing and waiting for the first one to complete.")]
            public bool AllowOverrides = false;

            /// <summary>
            /// This class contains information regarding how to perform smart follow for the Third Person Camera.
            /// </summary>
            [Serializable]
            public class RotationSmartFollow
            {
                /// <summary>
                /// Determines the orientation that smart follow will rotate to, this transform's rotation.
                /// </summary>
                [Tooltip("Determines the orientation that smart follow will rotate to, this transform's rotation.")]
                public Transform SmartFollowOrientationTransform;

                /// <summary>
                /// Whether or not the smart follow will be active. Requires AutoRotate to be enabled.
                /// </summary>
                [Tooltip("Whether or not the smart follow will be active. Requires AutoRotate to be enabled.")]
                public bool Enabled = true;

                /// <summary>
                /// If enabled, when conditions for Smart Follow are met, then the camera will begin smart following.
                /// </summary>
                [Header("Settings")]
                [Tooltip("If enabled, when conditions for Smart Follow are met, then the camera will begin smart following.")]
                public bool AutoActivate = true;

                /// <summary>
                /// If set, the smart follow will only update its rotation if the SmartFollowOrientationTarget is moving.
                /// </summary>
                [Tooltip("If set, the smart follow will only update its rotation if the SmartFollowOrientationTarget is moving.")]
                public bool OnlyUpdateWhileMoving = false;

                /// <summary>
                /// Tolerance for determining if the target is moving. They must move this much.
                /// </summary>
                [Tooltip("Tolerance for determining if the target is moving. They must move this much.")]
                public float OnlyUpdateWhileMovingTolerance = 0.01f;

                /// <summary>
                /// If the mouse is locked, then the smart follow won't update.
                /// </summary>
                [Tooltip("If the mouse is locked, then the smart follow won't update.")]
                public bool DontUpdateIfMouseLocked = true;

                /// <summary>
                /// The degrees per second the smart follow will reorient itself.
                /// </summary>
                [Tooltip("The degrees per second the smart follow will reorient itself.")]
                public float OrientateDegreesPerSecond = 80f;

                /// <summary>
                /// If the total angle is greater than this, quadratic scaling on the speed will occur making it reorient faster.
                /// </summary>
                [Tooltip("If the total angle is greater than this, quadratic scaling on the speed will occur making it reorient faster.")]
                public float OrientateDegreesSpeedUpAngle = 45;

                /// <summary>
                /// This will cause it to be faster when it's further from orientation and slower when closer.
                /// </summary>
                [Tooltip("This will cause it to be faster when it's further from orientation and slower when closer.")]
                public float OrientateDegreesSpeedUpRate = 0.5f;

                /// <summary>
                /// The distance that the SmartFollowOrientationTarget needs to move in order to activate smart follow.
                /// </summary>
                [Tooltip("The distance that the SmartFollowOrientationTarget needs to move in order to activate smart follow.")]
                public float DistanceActivation = 0.2f;
            }
            /// <summary>
            /// Defines how the camera will smart follow and automatically reorient itself.
            /// </summary>
            [Tooltip("Defines how the camera will smart follow and automatically reorient itself.")]
            public RotationSmartFollow SmartFollow = new RotationSmartFollow();
        }
        /// <summary>
        /// Allows you to automatically rotate the camera over a period of time
        /// </summary>
        [Tooltip("Allows you to automatically rotate the camera over a period of time")]
        public RotationAutoRotate AutoRotation = new RotationAutoRotate();

        #endregion

        #region Public Properties

        /// <summary>
        /// The orientation of the Camera's rotation. This orientation is used to determine what is horizontal, vertical, and limits on rotation.
        /// </summary>
        public Quaternion Orientation
        {
            get
            {
                return _orientationTransform.Rotation;
            }
        }

        /// <summary>
        /// The up vector for the current orientation of the Camera's rotation.
        /// </summary>
        public Vector3 OrientationUp
        {
            get
            {
                return _orientationTransform.Up;
            }
        }

        /// <summary>
        /// The amount of degrees the camera has rotated horizontally from the nuetral position bounded by [0, 360)
        /// </summary>
        public float HorizontalRotation
        {
            get
            {
                return (_horizontalRotationLimitsTotal % 360 + 360)%360;
            }
        }

        /// <summary>
        /// The amount of degrees the camera has rotated vertically from the nuetral position bounded by [0, 360)
        /// </summary>
        public float VerticalRotation
        {
            get
            {
                return (_verticalRotationLimitsTotal % 360 + 360) % 360;
            }
        }

        /// <summary>
        /// The VirtualTransform used to represent the orientation.
        /// Be careful directly modifying this as it may have unintended side effects.
        /// Modifying rotation of the orientation directly may have uses if you want to modify it's rotation without rotating the whole system,
        /// but manual rotation will not obey limits set up in the Rotation component.
        /// I imagine the primary use of directly accessing this would be when you've limited the horizontal rotation and you want to change the
        /// orientation so the limits are in different locations.
        /// </summary>
        public VirtualTransform OrientationTransform
        {
            get
            {
                return _orientationTransform;
            }
        }

        #endregion

        #region Private Fields & Properties

        /// <summary>
        /// The first time this rotation component is initialized, this will be set to true.
        /// </summary>
        private bool _isInitialized = false;

        /// <summary>
        /// VirtualTransform used to represent the orientation of the RotationComponent/Camera's rotation.
        /// </summary>
        private VirtualTransform _orientationTransform;

        /// <summary>
        /// Total amount of rotation on the vertical rotation for the degrees event.
        /// </summary>
        private float _verticalRotationDegreesEventTotal;

        /// <summary>
        /// Total amount of rotation on the horizontal rotation for the degrees event.
        /// </summary>
        private float _horizontalRotationDegreesEventTotal;

        /// <summary>
        /// Total rotation horizontally for enforcing limits. This number can overflow.
        /// </summary>
        private float _horizontalRotationLimitsTotal;

        /// <summary>
        /// Total rotation vertical for enforcing limits. This number can overflow.
        /// </summary>
        private float _verticalRotationLimitsTotal;

        #region Auto Rotate

        /// <summary>
        /// The remaining time left for autorotation.
        /// </summary>
        private float _autoRotateTimeRemaining;

        /// <summary>
        /// The total time for the current auto rotation.
        /// </summary>
        private float _autoRotateTimeTotal;

        /// <summary>
        /// the total amount of horizontal degrees for this auto rotation.
        /// </summary>
        private float _autoRotateTotalHorizontalDegrees;

        /// <summary>
        /// the total amount of vertical degrees for this auto rotation.
        /// </summary>
        private float _autoRotateTotalVerticalDegrees;

        /// <summary>
        /// Lerp transformation applied to the horizontal rotation.
        /// </summary>
        private ILerpTransformer _autoRotateLerpHorizontalTransformer;

        /// <summary>
        /// Lerp transformation applied ot the vertical rotation.
        /// </summary>
        private ILerpTransformer _autoRotateLerpVerticalTransformer;

        /// <summary>
        /// Whether or not the external rotation is disabled from rotating the component or not.
        /// </summary>
        private bool _isExternalDisabled
        {
            get
            {
                return AutoRotation.DisableExternalRotationWhileAutoRotating && _autoRotateTimeRemaining > 0;
            }
        }

        #endregion

        #region Smart Follow

        /// <summary>
        /// The last position the smart follow target was at.
        /// </summary>
        private Vector3 _smartFollowTargetLastPosition;

        /// <summary>
        /// If the smart follow is current running.
        /// </summary>
        private bool _smartFollowActivated;

        /// <summary>
        /// Set to true while rotating in the smart follow.
        /// </summary>
        private bool _smartFollowRotationRetain;

        #endregion

        #endregion

        public override void Initialize(CameraController cameraController)
        {
            base.Initialize(cameraController);

            HorizontalDegreesEvent.RotationEventListeners = new List<HorizontalDegreesListener>();
            VerticalDegreesEvent.RotationEventListeners = new List<VerticalDegreesListener>();

            _orientationTransform = new VirtualTransform();
            _horizontalRotationLimitsTotal = 0;
            _verticalRotationLimitsTotal = 0;
            _horizontalRotationDegreesEventTotal = 0;
            _verticalRotationDegreesEventTotal = 0;

            _autoRotateLerpHorizontalTransformer = new DoNothingLerpTransformer();
            _autoRotateLerpVerticalTransformer = _autoRotateLerpHorizontalTransformer;

            _isInitialized = true;

            if (AutoRotation.SmartFollow.SmartFollowOrientationTransform != null)
            {
                _smartFollowTargetLastPosition = AutoRotation.SmartFollow.SmartFollowOrientationTransform.position;
            }
        }

        #region Orientation

        /// <summary>
        /// Adjusts the orientation of RotationComponent. 
        /// This should be used to change the up vector of the Camera's rotation, as well as where the limits of rotation are for the RotationComponent.
        /// This will preserve the Camera's current location in relation to the target by moving the Camera.
        /// </summary>
        /// <param name="eulerAngles">New orientation in euler angles.</param>
        /// <param name="target">OffsetTarget that is currently being Rotated around.</param>
        public void SetRotationOrientation(Vector3 eulerAngles, Vector3 target)
        {
            SetRotationOrientation(Quaternion.Euler(eulerAngles), target);
        }

        /// <summary>
        /// Adjusts the orientation of RotationComponent. 
        /// This should be used to change the up vector of the Camera's rotation, as well as where the limits of rotation are for the RotationComponent.
        /// This will preserve the Camera's current location in relation to the target by moving the Camera.
        /// </summary>
        /// <param name="orientation">New orientation.</param>
        /// <param name="target">Target that is currently being Rotated around.</param>
        public void SetRotationOrientation(Quaternion orientation, Vector3 target)
        {
            if (_orientationTransform.Rotation == orientation)
            {
                return;
            }

            float distance = Vector3.Distance(target, CameraTransform.Position);

            var diff = orientation * Quaternion.Inverse(Orientation);

            CameraTransform.Rotate(diff, Space.World);
            CameraTransform.Position = target - distance * CameraTransform.Forward;

            _orientationTransform.Rotation = orientation;
        }

        #endregion

        #region SmartFollow

        /// <summary>
        /// Activates SmartFollow to start rotating the camera.
        /// This is different from enabling it.
        /// </summary>
        public void ActivateSmartFollow()
        {
            _smartFollowActivated = true;
        }

        /// <summary>
        /// Updates the smart follow.
        /// This will progress the current smart follow rotation.
        /// </summary>
        public void UpdateSmartFollow()
        {
            Transform orientation = AutoRotation.SmartFollow.SmartFollowOrientationTransform;
            if (!AutoRotation.Enabled ||
                !AutoRotation.SmartFollow.Enabled ||
                orientation == null)
            {
                if (orientation != null)
                {
                    _smartFollowTargetLastPosition = orientation.position;
                }
                return;
            }
            
            Vector3 position = orientation.position;
            if (SmartFollowDeactivationConditionsMet())
            {
                _smartFollowActivated = false;
                _smartFollowTargetLastPosition = position;
            }
            
            if (_smartFollowActivated || SmartFollowActivationConditionsMet())
            {
                _smartFollowActivated = true;

                // Calculate the total distance we need to rotate horizontally
                float camH = GetHorizontalAngle(CameraTransform.Rotation);
                float orientationH = GetHorizontalAngle(orientation.rotation);
                float diffH = GetShortestDegreesDistance(orientationH, camH);

                // Calculate the total distance we need to rotate vertically
                float camV = VerticalRotation;
                float orientationV = 360 - GetVerticalAngle(orientation.rotation);
                float diffV = -GetShortestDegreesDistance(orientationV, camV);

                float toGoH = diffH;
                float toGoV = diffV;

                if (toGoH != 0 || toGoV != 0)
                {
                    float toGoHAbs = Mathf.Abs(toGoH);
                    float toGoVAbs = Mathf.Abs(toGoV);

                    float power = AutoRotation.SmartFollow.OrientateDegreesPerSecond*Time.deltaTime;

                    float overPower = (toGoHAbs + toGoVAbs) - AutoRotation.SmartFollow.OrientateDegreesSpeedUpAngle;
                    if (overPower > 0)
                    {
                        power += overPower * Time.deltaTime * AutoRotation.SmartFollow.OrientateDegreesSpeedUpRate;
                    }

                    // power * positive or negative * percentage that belongs to that rotation axis
                    // This makes it so that it moves across both the vertical and horizontal in a way that they both 
                    // reach their destination at the same time.
                    float powerH = power * (toGoH > 0 ? 1 : -1) * (toGoHAbs / (toGoHAbs + toGoVAbs));
                    float powerV = power * (toGoV > 0 ? 1 : -1) * (toGoVAbs / (toGoHAbs + toGoVAbs));

                    float horizontal = Mathf.Abs(powerH) < toGoHAbs ? powerH : toGoH;
                    float vertical = Mathf.Abs(powerV) < toGoVAbs ? powerV : toGoV;
                    
                    _smartFollowRotationRetain = true;
                    if (horizontal != 0)
                    {
                        RotateHorizontally(horizontal);
                    }
                    if (vertical != 0)
                    {
                        RotateVertically(vertical);
                    }
                    _smartFollowRotationRetain = false;
                }
                _smartFollowTargetLastPosition = position;
            }
        }

        /// <summary>
        /// Returns a flattened vector on the XZ plane.
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        private Vector3 Flatten(Vector3 vec)
        {
            return new Vector3(vec.x, 0, vec.z).normalized;
        }

        /// <summary>
        /// Returns the horizontal angle from the forward.
        /// </summary>
        private float GetHorizontalAngle(Quaternion rot)
        {
            VirtualTransform vt = new VirtualTransform(rot);
            Vector3 flat = Flatten(vt.Forward);
            float f1 = Vector3.Angle(flat, Vector3.forward);
            float f2 = Vector3.Angle(flat, Vector3.left);
            float f3 = Vector3.Angle(flat, Vector3.right);
            return f3 > f2 ? f1 : 360 - f1;
        }


        /// <summary>
        /// Returns the vertical angle based on the up.
        /// </summary>
        private float GetVerticalAngle(Quaternion rot)
        {
            VirtualTransform vt = new VirtualTransform(rot);
            float f1 = Vector3.Angle(vt.Up, Vector3.up);
            float f2 = Vector3.Angle(vt.Forward, Vector3.up);
            return f2 < 90 ? f1 : 360 - f1;
        }

        /// <summary>
        /// Determines if the conditions for activating smart follow have been met.
        /// </summary>
        /// <returns>If smart follow conditions are met.</returns>
        private bool SmartFollowActivationConditionsMet()
        {
            if ((Cursor.lockState == CursorLockMode.None || !AutoRotation.SmartFollow.DontUpdateIfMouseLocked) &&
                AutoRotation.Enabled &&
                AutoRotation.SmartFollow.AutoActivate &&
                AutoRotation.SmartFollow.Enabled &&
                AutoRotation.SmartFollow.SmartFollowOrientationTransform != null &&
                Vector3.Distance(_smartFollowTargetLastPosition, AutoRotation.SmartFollow.SmartFollowOrientationTransform.position) >= AutoRotation.SmartFollow.DistanceActivation &&
                (!AutoRotation.SmartFollow.OnlyUpdateWhileMoving
                    || Vector3.Distance(_smartFollowTargetLastPosition, AutoRotation.SmartFollow.SmartFollowOrientationTransform.position) > AutoRotation.SmartFollow.OnlyUpdateWhileMovingTolerance))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Determines if the conditions to stop smart following have been met.
        /// </summary>
        /// <returns>If smart follow should stop running.</returns>
        private bool SmartFollowDeactivationConditionsMet()
        {
            if (   
                (AutoRotation.SmartFollow.OnlyUpdateWhileMoving
                    && Vector3.Distance(_smartFollowTargetLastPosition, AutoRotation.SmartFollow.SmartFollowOrientationTransform.position) <= AutoRotation.SmartFollow.OnlyUpdateWhileMovingTolerance) 
                ||
                (AutoRotation.SmartFollow.DontUpdateIfMouseLocked 
                    && Cursor.lockState == CursorLockMode.Locked)
                ||
                (Vector3.Angle(CameraTransform.Forward, AutoRotation.SmartFollow.SmartFollowOrientationTransform.forward) <= 0.00001f)
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region AutoRotation

        /// <summary>
        /// Updates the automatic rotation.
        /// This will progress the current automatic rotation.
        /// </summary>
        public void UpdateAutoRotate()
        {
            if (!AutoRotation.Enabled || _autoRotateTimeRemaining <= 0)
            {
                return;
            }

            // Before 't' value, which is the percentage of the way through the total rotation
            float bt = 1 - (_autoRotateTimeRemaining / _autoRotateTimeTotal);
            float bht = _autoRotateLerpHorizontalTransformer.Process(bt);
            float bvt = _autoRotateLerpVerticalTransformer.Process(bt);

            _autoRotateTimeRemaining -= Time.deltaTime;
            _autoRotateTimeRemaining = Mathf.Max(_autoRotateTimeRemaining, 0);

            // After 't' value, which is the percentage of the way through the total rotation
            float at = 1 - (_autoRotateTimeRemaining / _autoRotateTimeTotal);
            float aht = _autoRotateLerpHorizontalTransformer.Process(at);
            float avt = _autoRotateLerpVerticalTransformer.Process(at);

            // We use both values to figure out the difference to calculate how many degrees we need to rotate this update.
            float horizontalRotationThisUpdate = (aht - bht) * _autoRotateTotalHorizontalDegrees;
            float verticalRotationThisUpdate = (avt - bvt) * _autoRotateTotalVerticalDegrees;

            // We need to temporarily disable this value
            bool setting = AutoRotation.DisableExternalRotationWhileAutoRotating;
            AutoRotation.DisableExternalRotationWhileAutoRotating = false;
            // Perform our rotation while it's disabled.
            Rotate(horizontalRotationThisUpdate, verticalRotationThisUpdate);
            // And set it back to it's original value.
            AutoRotation.DisableExternalRotationWhileAutoRotating = setting;
        }

        /// <summary>
        /// Automatically rotates the camera by the given horizontal and vertical value, in the given amount of time.
        /// </summary>
        /// <param name="horizontal">Degrees of rotation for the horizontal.</param>
        /// <param name="vertical">Degrees of rotation for the vertical.</param>
        /// <param name="time">Total time to complete this rotation.</param>
        public void AutoRotateBy(float horizontal, float vertical, float time)
        {
            var lerpTransformer = new DoNothingLerpTransformer();
            AutoRotateBy(horizontal, vertical, time, lerpTransformer, lerpTransformer);
        }

        /// <summary>
        /// Automatically rotates the camera by the given horizontal and vertical value, in the given amount of time.
        /// </summary>
        /// <param name="horizontal">Degrees of rotation for the horizontal.</param>
        /// <param name="vertical">Degrees of rotation for the vertical.</param>
        /// <param name="time">Total time to complete this rotation.</param>
        /// <param name="lerpTransformer">Transformers the progression of the rotation for both rotations using the lerp transformer.</param>
        public void AutoRotateBy(float horizontal, float vertical, float time, ILerpTransformer lerpTransformer)
        {
            AutoRotateBy(horizontal, vertical, time, lerpTransformer, lerpTransformer);
        }

        /// <summary>
        /// Automatically rotates the camera by the given horizontal and vertical value, in the given amount of time.
        /// </summary>
        /// <param name="horizontal">Degrees of rotation for the horizontal.</param>
        /// <param name="vertical">Degrees of rotation for the vertical.</param>
        /// <param name="time">Total time to complete this rotation.</param>
        /// <param name="horizontalLerpTransformer">Transformers the progression of the rotation for the horizontal rotation using the lerp transformer.</param>
        /// <param name="verticalLerpTransformer">Transformers the progression of the rotation for the vertical rotation using the lerp transformer.</param>
        public void AutoRotateBy(float horizontal, float vertical, float time, ILerpTransformer horizontalLerpTransformer, ILerpTransformer verticalLerpTransformer)
        {
            // Already performing an auto rotation.
            if (!AutoRotation.AllowOverrides && _autoRotateTimeRemaining > 0)
            {
                return;
            }

            _autoRotateLerpHorizontalTransformer = horizontalLerpTransformer;
            _autoRotateLerpVerticalTransformer = verticalLerpTransformer;
            _autoRotateTimeRemaining = time;
            _autoRotateTimeTotal = time;
            _autoRotateTotalHorizontalDegrees = horizontal;
            _autoRotateTotalVerticalDegrees = vertical;
        }

        /// <summary>
        /// Automatically rotates the camera to the given horizontal and vertical value, in the given amount of time.
        /// This ends up actually doing a "rotate by", so if you add rotation ontop of the automatic rotation, you may not rotate to where you want to be.
        /// </summary>
        /// <param name="horizontal">Degrees of rotation for the horizontal.</param>
        /// <param name="vertical">Degrees of rotation for the vertical.</param>
        /// <param name="time">Total time to complete this rotation.</param>
        public void AutoRotateTo(float? horizontal, float? vertical, float time)
        {
            var lerpTransformer = new DoNothingLerpTransformer();
            AutoRotateTo(horizontal, vertical, time, lerpTransformer, lerpTransformer);
        }

        /// <summary>
        /// Automatically rotates the camera to the given horizontal and vertical value, in the given amount of time.
        /// This ends up actually doing a "rotate by", so if you add rotation ontop of the automatic rotation, you may not rotate to where you want to be.
        /// </summary>
        /// <param name="horizontal">Degrees of rotation for the horizontal.</param>
        /// <param name="vertical">Degrees of rotation for the vertical.</param>
        /// <param name="time">Total time to complete this rotation.</param>
        /// <param name="lerpTransformer">Transformers the progression of the rotation for both rotations using the lerp transformer.</param>
        public void AutoRotateTo(float? horizontal, float? vertical, float time, ILerpTransformer lerpTransformer)
        {
            AutoRotateTo(horizontal, vertical, time, lerpTransformer, lerpTransformer);
        }

        /// <summary>
        /// Automatically rotates the camera to the given horizontal and vertical value, in the given amount of time.
        /// This ends up actually doing a "rotate by", so if you add rotation ontop of the automatic rotation, you may not rotate to where you want to be.
        /// </summary>
        /// <param name="horizontal">Degrees of rotation for the horizontal.</param>
        /// <param name="vertical">Degrees of rotation for the vertical.</param>
        /// <param name="time">Total time to complete this rotation.</param>
        /// <param name="horizontalLerpTransformer">Transformers the progression of the rotation for the horizontal rotation using the lerp transformer.</param>
        /// <param name="verticalLerpTransformer">Transformers the progression of the rotation for the vertical rotation using the lerp transformer.</param>
        public void AutoRotateTo(float? horizontal, float? vertical, float time, ILerpTransformer horizontalLerpTransformer, ILerpTransformer verticalLerpTransformer)
        {
            float realHorizontal = horizontal.HasValue ? GetShortestDegreesDistance(HorizontalRotation, horizontal.Value) : 0;
            float realVertical = vertical.HasValue ? GetShortestDegreesDistance(VerticalRotation, vertical.Value) : 0;

            AutoRotateBy(realHorizontal, realVertical, time, horizontalLerpTransformer, verticalLerpTransformer);
        }

        /// <summary>
        /// Returns an amount of degrees needed to rotate to the target from the current in the shortest distance possible.
        /// This assumes that 360 and 0 are equal.
        /// </summary>
        /// <param name="current">Current amount of degrees [0, 360)</param>
        /// <param name="target">Target amount of degrees [0, 360)</param>
        /// <returns>Shortest distance in degrees.</returns>
        private float GetShortestDegreesDistance(float current, float target)
        {
            // The values are the same.
            if (current == target || (current == 0 && target == 360) || (current == 360 && target == 0))
            {
                return 0;
            }

            // the normal distance between the two targets
            float distance = Mathf.Abs(current - target);

            if (distance < 180)
            {
                float factor = target > current ? 1 : -1;
                return distance*factor;
            }
            else
            {
                distance = 360 - distance;
                float factor = target > current ? -1 : 1;
                return distance * factor;
            }
        }

        #endregion

        #region Rotation

        /// <summary>
        /// Rotates the Camera horizontally by the given degrees.
        /// </summary>
        /// <param name="degrees">Given degrees.</param>
        public void RotateHorizontally(float degrees)
        {
            if (!Enabled || _isExternalDisabled)
            {
                return;
            }

            _smartFollowActivated = _smartFollowRotationRetain;

            if (Limits.EnableHorizontalLimits)
            {
                degrees = GetEnforcedHorizontalDegrees(degrees);
            }

            CameraTransform.Rotate(_orientationTransform.Up, degrees, Space.World);

            if (HorizontalDegreesEvent.Enabled)
            {
                TrackHorizontalEventRotation(degrees);
            }

            TrackHorizontalLimitsRotation(degrees);
        }

        /// <summary>
        /// Rotates the Camera vertically by the given degrees
        /// </summary>
        /// <param name="degrees">Given degrees.</param>
        public void RotateVertically(float degrees)
        {
            if (!Enabled || _isExternalDisabled)
            {
                return;
            }

            _smartFollowActivated = _smartFollowRotationRetain;

            if (Limits.EnableVerticalLimits)
            {
                degrees = GetEnforcedVerticalDegrees(degrees);
            }

            CameraTransform.Rotate(CameraTransform.Right, degrees, Space.World);

            if (VerticalDegreesEvent.Enabled)
            {
                TrackVerticalEventRotation(degrees);
            }

            TrackVerticalLimitsRotation(degrees);
        }
        
        /// <summary>
        /// Rotates the Camera horizontally and then vertically at once by the given degrees.
        /// </summary>
        /// <param name="horizontalDegrees">Given degrees for horizontal.</param>
        /// <param name="verticalDegrees">Given degrees for vertical.</param>
        public void Rotate(float horizontalDegrees, float verticalDegrees)
        {
            RotateHorizontally(horizontalDegrees);
            RotateVertically(verticalDegrees);
        }

        /// <summary>
        /// Sets the rotation of the rotation component to the given settings.
        /// </summary>
        /// <param name="horizontalDegrees">Horizontal degrees of rotation to set it to.</param>
        /// <param name="verticalDegrees">Vertical degrees of rotation to set it to.</param>
        /// <param name="trackRotation">Whether or not rotation is tracked for events.</param>
        public void SetRotation(float horizontalDegrees, float verticalDegrees, bool trackRotation = false)
        {
            _smartFollowActivated = _smartFollowRotationRetain;

            bool horizontalTracking = HorizontalDegreesEvent.Enabled;
            bool verticalTracking = VerticalDegreesEvent.Enabled;
            
            if (!trackRotation)
            {
                HorizontalDegreesEvent.Enabled = false;
                VerticalDegreesEvent.Enabled = false;
            }

            float horizontalDiff = horizontalDegrees - HorizontalRotation;
            float verticalDiff = verticalDegrees - VerticalRotation;

            RotateHorizontally(horizontalDiff);
            RotateVertically(verticalDiff);

            HorizontalDegreesEvent.Enabled = horizontalTracking;
            VerticalDegreesEvent.Enabled = verticalTracking;
        }

        /// <summary>
        /// Sets the rotation of the rotation component to the given settings.
        /// </summary>
        /// <param name="rotation">Represents the rotation we want.</param>
        /// <param name="trackRotation">Whether or not rotation is tracked for events.</param>
        public void SetRotation(Quaternion rotation,bool trackRotation = false)
        {
            Vector3 rotationEuler = rotation.eulerAngles;
            Vector3 cameraEuler = CameraTransform.EulerAngles;
            
            float horizontalDiff = (rotationEuler.y - cameraEuler.y);
            float verticalDiff = (rotationEuler.x - cameraEuler.x);

            if (horizontalDiff > 180)
            {
                horizontalDiff -= 360;
            }
            else if (horizontalDiff < -180)
            {
                horizontalDiff += 360;
            }

            if (verticalDiff > 180)
            {
                verticalDiff -= 360;
            }
            else if (verticalDiff < -180)
            {
                verticalDiff += 360;
            }

            SetRotation(horizontalDiff + HorizontalRotation, verticalDiff + VerticalRotation, trackRotation);
        }
        
        #endregion

        #region Limited Rotation

        /// <summary>
        /// Returns the amount of degrees you can rotate by when enforcing horizontal limits when given an amount of degrees you want to rotate by.
        /// </summary>
        /// <param name="degrees">Degrees you want to attempt to rotate by.</param>
        /// <returns>Actual degrees you're allowed to rotate by.</returns>
        public float GetEnforcedHorizontalDegrees(float degrees)
        {
            // rotating right
            if (degrees < 0)
            {
                if (_horizontalRotationLimitsTotal + degrees < Limits.HorizontalRight)
                {
                    return Limits.HorizontalRight - _horizontalRotationLimitsTotal;
                }
            }
            // rotating left
            else if (degrees >= 0)
            {
                if (_horizontalRotationLimitsTotal + degrees > Limits.HorizontalLeft)
                {
                    return Limits.HorizontalLeft - _horizontalRotationLimitsTotal;
                }
            }

            return degrees;
        }

        /// <summary>
        /// Returns the amount of degrees you can rotate by when enforcing vertical limits when given an amount of degrees you want to rotate by.
        /// </summary>
        /// <param name="degrees">Degrees you want to attempt to rotate by.</param>
        /// <returns>Actual degrees you're allowed to rotate by.</returns>
        public float GetEnforcedVerticalDegrees(float degrees)
        {
            // rotating down
            if (degrees < 0)
            {
                if (_verticalRotationLimitsTotal + degrees < Limits.VerticalDown)
                {
                    return Limits.VerticalDown - _verticalRotationLimitsTotal;
                }
            }
            // rotating up
            else if (degrees >= 0)
            {
                if (_verticalRotationLimitsTotal + degrees > Limits.VerticalUp)
                {
                    return Limits.VerticalUp - _verticalRotationLimitsTotal;
                }
            }

            return degrees;
        }

        #endregion

        #region Tracking Rotation

        /// <summary>
        /// Tracks the horizontally rotated degrees for rotation events.
        /// </summary>
        /// <param name="degrees">Given degrees.</param>
        private void TrackHorizontalEventRotation(float degrees)
        {
            _horizontalRotationDegreesEventTotal += degrees;
        }

        /// <summary>
        /// Tracks the vertically rotated degrees for rotation events.
        /// </summary>
        /// <param name="degrees">Given degrees.</param>
        private void TrackVerticalEventRotation(float degrees)
        {
            _verticalRotationDegreesEventTotal += degrees;
        }

        /// <summary>
        /// Tracks the horizontal rotation for enforcing limits.
        /// </summary>
        /// <param name="degrees">Given degrees.</param>
        private void TrackHorizontalLimitsRotation(float degrees)
        {
            unchecked
            {
                _horizontalRotationLimitsTotal += degrees;
            }
        }

        /// <summary>
        /// Tracks the vertical rotation for enforcing limits.
        /// </summary>
        /// <param name="degrees">Given degrees.</param>
        private void TrackVerticalLimitsRotation(float degrees)
        {
            unchecked
            {
                _verticalRotationLimitsTotal += degrees;
            }
        }

        #endregion

        #region Rotation Events
        
        /// <summary>
        /// Checks the horizontal degrees events and fires events as needed first, then checks the vertical degrees events and fires events as needed.
        /// This does not fire if the rotation component is not enabled.
        /// </summary>
        public void CheckRotationDegreesEvents()
        {
            if (!Enabled)
            {
                return;
            }

            CheckHorizontalDegreesEvents();
            CheckVerticalDegreesEvents();
        }

        #region Horizontal Rotation Event

        /// <summary>
        /// Checks the horizontal degrees events and fires events as needed.
        /// This does not fire if the rotation component is not enabled or if the horizontal degrees event is not enabled.
        /// </summary>
        public void CheckHorizontalDegreesEvents()
        {
            // Even though we don't track when disabled, we're still going to return here as well to prevent any situation were 
            if (!HorizontalDegreesEvent.Enabled || !Enabled)
            {
                return;
            }

            float total = Mathf.Abs(_horizontalRotationDegreesEventTotal);

            if (total < HorizontalDegreesEvent.DegreesTrigger)
            {
                return;
            }

            float modifier = _horizontalRotationDegreesEventTotal > 0 ? 1 : -1;
            float amount = modifier * HorizontalDegreesEvent.DegreesTrigger;

            if (HorizontalDegreesEvent.ResetTotalAfterEachEvent)
            {
                FireHorizontalDegreesEvent(amount);
                _horizontalRotationDegreesEventTotal = 0;
            }
            else
            {
                while (total > HorizontalDegreesEvent.DegreesTrigger)
                {
                    FireHorizontalDegreesEvent(amount);
                    total -= HorizontalDegreesEvent.DegreesTrigger;
                    _horizontalRotationDegreesEventTotal -= amount;
                }
            }
        }

        /// <summary>
        /// Registers a listener to listen to when the camera has rotated a set number of degrees.
        /// The value will be positive or negative based on which direction the camera rotated.
        /// You shouldn't alter the camera during this event or bad things may happen.
        /// </summary>
        /// <param name="degreesListener">The listener</param>
        public void RegisterListener(HorizontalDegreesListener degreesListener)
        {
            if (!_isInitialized)
            {
                throw new ComponentNotInitializedException("Can't register a HorizontalDegreesListener until the RotationComponent has been initialized atleast once.");
            }

            if (!HorizontalDegreesEvent.RotationEventListeners.Contains(degreesListener))
            {
                HorizontalDegreesEvent.RotationEventListeners.Add(degreesListener);
            }
        }

        /// <summary>
        /// Unregisters a listener so that it no longer listens to when the camera has rotated a set number of degrees.
        /// </summary>
        /// <param name="degreesListener"></param>
        public void UnregisterListener(HorizontalDegreesListener degreesListener)
        {
            if (!_isInitialized)
            {
                throw new ComponentNotInitializedException("Can't unregister a HorizontalDegreesListener until the RotationComponent has been initialized atleast once.");
            }

            if (HorizontalDegreesEvent.RotationEventListeners.Contains(degreesListener))
            {
                HorizontalDegreesEvent.RotationEventListeners.Remove(degreesListener);
            }
        }

        /// <summary>
        /// Clears all HorizontalDegreeListeners from listening to this RotationComponent.
        /// </summary>
        public void ClearHorizontalListeners()
        {
            if (!_isInitialized)
            {
                throw new ComponentNotInitializedException("Can't clear a HorizontalDegreesListeners until the RotationComponent has been initialized atleast once.");
            }

            HorizontalDegreesEvent.RotationEventListeners.Clear();
        }

        /// <summary>
        /// Fires the rotation listener event to alert listeners when the camera has rotated a set number of degrees.
        /// </summary>
        /// <param name="amount">The amount of degrees the camera has rotated.</param>
        private void FireHorizontalDegreesEvent(float amount)
        {
            foreach (var listener in HorizontalDegreesEvent.RotationEventListeners)
            {
                listener.DegreesRotated(amount);
            }
        }

        #endregion

        #region Vertical Rotation Event

        /// <summary>
        /// Checks the vertical degrees events and fires events as needed.
        /// This does not fire if the rotation component is not enabled or if the vertical degrees event is not enabled.
        /// </summary>
        public void CheckVerticalDegreesEvents()
        {
            // Even though we don't track when disabled, we're still going to return here as well to prevent any situation were 
            if (!VerticalDegreesEvent.Enabled || !Enabled)
            {
                return;
            }

            float total = Mathf.Abs(_verticalRotationDegreesEventTotal);

            if (total < VerticalDegreesEvent.DegreesTrigger)
            {
                return;
            }

            float modifier = _verticalRotationDegreesEventTotal > 0 ? 1 : -1;
            float amount = modifier * VerticalDegreesEvent.DegreesTrigger;

            if (VerticalDegreesEvent.ResetTotalAfterEachEvent)
            {
                FireVerticalDegreesEvent(amount);
                _verticalRotationDegreesEventTotal = 0;
            }
            else
            {
                while (total > VerticalDegreesEvent.DegreesTrigger)
                {
                    FireVerticalDegreesEvent(amount);
                    total -= VerticalDegreesEvent.DegreesTrigger;
                    _verticalRotationDegreesEventTotal -= amount;
                }
            }
        }

        /// <summary>
        /// Registers a listener to listen to when the camera has rotated a set number of degrees.
        /// The value will be positive or negative based on which direction the camera rotated.
        /// You shouldn't alter the camera during this event or bad things may happen.
        /// </summary>
        /// <param name="degreesListener">The listener</param>
        public void RegisterListener(VerticalDegreesListener degreesListener)
        {
            if (!_isInitialized)
            {
                throw new ComponentNotInitializedException("Can't register a VerticalDegreesListener until the RotationComponent has been initialized atleast once.");
            }

            if (!VerticalDegreesEvent.RotationEventListeners.Contains(degreesListener))
            {
                VerticalDegreesEvent.RotationEventListeners.Add(degreesListener);
            }
        }

        /// <summary>
        /// Unregisters a listener so that it no longer listens to when the camera has rotated a set number of degrees.
        /// </summary>
        /// <param name="degreesListener"></param>
        public void UnregisterListener(VerticalDegreesListener degreesListener)
        {
            if (!_isInitialized)
            {
                throw new ComponentNotInitializedException("Can't unregister a VerticalDegreesListener until the RotationComponent has been initialized atleast once.");
            }

            if (VerticalDegreesEvent.RotationEventListeners.Contains(degreesListener))
            {
                VerticalDegreesEvent.RotationEventListeners.Remove(degreesListener);
            }
        }

        /// <summary>
        /// Clears all HorizontalDegreeListeners from listening to this RotationComponent.
        /// </summary>
        public void ClearVerticalListeners()
        {
            if (!_isInitialized)
            {
                throw new ComponentNotInitializedException("Can't clear a VerticalDegreesListeners until the RotationComponent has been initialized atleast once.");
            }

            VerticalDegreesEvent.RotationEventListeners.Clear();
        }

        /// <summary>
        /// Fires the rotation listener event to alert listeners when the camera has rotated a set number of degrees.
        /// </summary>
        /// <param name="amount">The amount of degrees the camera has rotated.</param>
        private void FireVerticalDegreesEvent(float amount)
        {
            foreach (var listener in VerticalDegreesEvent.RotationEventListeners)
            {
                listener.DegreesRotated(amount);
            }
        }

        #endregion

        #endregion
        
    }
}