using System;
using AdvancedUtilities.LerpTransformers;
using UnityEngine;

namespace AdvancedUtilities.Cameras.Components
{
    /// <summary>
    /// A camera component that handles rotation for following behind a target automatically.
    /// </summary>
    [Serializable]
    public class FollowRotationComponent : CameraComponent
    {
        #region Public Fields

        /// <summary>
        /// Whether the camera will rotate to follow the target or not.
        /// </summary>
        [Tooltip("Whether the camera will rotate to follow the target or not.")]
        public bool Enabled = true;

        /// <summary>
        /// If set, the rotation will always be snapped to the nuetral position.
        /// </summary>
        [Tooltip("If set, the rotation will always be snapped to the nuetral position.")]
        public bool SnapRotation = false;

        /// <summary>
        /// The offset of the Y rotation from the target's actual rotation.
        /// If set to a positive value, then this camera will always try to rotate to look at the target from above
        /// as a nuetral state.
        /// </summary>
        [Tooltip("The offset of the Y rotation from the target's actual rotation. "+
        "If set to a positive value, then this camera will always try to rotate to look at the target from above "+
        "as a nuetral state.")]
        public float YRotationOffset = 5f;

        /// <summary>
        /// The offset of the X rotation from the target's actual rotation.
        /// If set to a positive value, then this camera will always try to rotate to look at the target from the right
        /// as a nuetral state.
        /// </summary>
        [Tooltip("The offset of the X rotation from the target's actual rotation. "+
        "If set to a positive value, then this camera will always try to rotate to look at the target from the right "+
        "as a nuetral state.")]
        public float XRotationOffset = 0;

        /// <summary>
        /// The speed that the camera renormalizes back to nuetral orientation.
        /// </summary>
        [Tooltip("The speed that the camera renormalizes back to nuetral orientation.")]
        public float AnglePerSecond = 45f;

        /// <summary>
        /// The amount of movement needed to trigger the AdjustCompletelyOnMovement and the AdjustWhileMoving modes.
        /// </summary>
        [Tooltip("The amount of movement needed to trigger the AdjustCompletelyOnMovement and the AdjustWhileMoving modes.")]
        public float MovementTolerance = 0.001f;

        /// <summary>
        /// The amount of rotation needed to trigger the AdjustCompletelyOnRotation mode.
        /// </summary>
        [Tooltip("The amount of rotation needed to trigger the AdjustCompletelyOnRotation mode.")]
        public float RotationTolerance = 0f;

        /// <summary>
        /// The mode of adjusting the rotation of the camera.
        /// </summary>
        [Tooltip("The mode of adjusting the rotation of the camera.")]
        public FollowRotationMode Mode = FollowRotationMode.AdjustCompletelyOnRotation;

        /// <summary>
        /// Rotation will be determined by the difference in movement between updates instead of the rotation of the actual target itself.
        /// </summary>
        [Tooltip("Rotation will be determined by the difference in movement between updates instead of the rotation of the actual target itself.")]
        public bool DetermineRotationFromMovement = false;

        /// <summary>
        /// When determining rotation from movement, this will flatten the rotation using the target's UP vector so that it doesn't not rotate at angles.
        /// </summary>
        [Tooltip("When determining rotation from movement, this will flatten the rotation using the target's UP vector so that it doesn't not rotate at angles.")]
        public bool FlattenRotationWhenDeterminingFromMovement = true;

        /// <summary>
        /// A new rotation will only be determined after the character moves this many units.
        /// </summary>
        [Tooltip("A new rotation will only be determined after the character moves this many units.")]
        public float DetermineTolerance = 0.001f;

        #endregion

        #region Private fields and properties

        /// <summary>
        /// Whether or not the camera is currently adjusting it's rotation to the nuetral state of the target's rotation.
        /// </summary>
        public bool IsRotating
        {
            get
            {
                return !_finishedAdjustment;
            }
        }

        /// <summary>
        /// The target the camera is looking at.
        /// </summary>
        private TargetComponent _target;
        
        /// <summary>
        /// The state of the target in the previous update.
        /// </summary>
        private VirtualTransform _previousTargetState;

        /// <summary>
        /// Used to ensure that we continue to rotate
        /// </summary>
        private bool _finishedAdjustment = true;

        /// <summary>
        /// Lerp transformer used for rotation.
        /// </summary>
        public ILerpTransformer RotationLerpTransformer { get; set; }

        /// <summary>
        /// The y rotation offset in the last update.
        /// Used to fix changes dynamically.
        /// </summary>
        private float _previousYRotationOffset;

        /// <summary>
        /// The x rotation offset in the last update.
        /// Used to fix changes dynamically.
        /// </summary>
        private float _previousXRotationOffset;

        /// <summary>
        /// Whether or not the camera will be forced to rotate back to nuetral completely next update.
        /// </summary>
        private bool _forceAdjustment = false;

        /// <summary>
        /// The rotation that was determined from movement the last time it was checked.
        /// </summary>
        private Quaternion _previouslyDeterminedRotation = Quaternion.identity;

        #endregion

        public override void Initialize(CameraController cameraController)
        {
            base.Initialize(cameraController);
            _target = GetCameraComponent<TargetComponent>();

            _previouslyDeterminedRotation = _target.Target.rotation;
            _previousTargetState = new VirtualTransform(_target.Target);

            _finishedAdjustment = true;
            _forceAdjustment = false;

            FixOffsets();

            if (RotationLerpTransformer == null)
            {
                RotationLerpTransformer = new DoNothingLerpTransformer();
            }
        }

        /// <summary>
        /// The camera will rotate to it's neutral position behind the target.
        /// </summary>
        public void ForceAdjustment()
        {
            _forceAdjustment = true;
        }

        /// <summary>
        /// Updates the rotation of the camera based on the target and 
        /// </summary>
        public void UpdateRotation()
        {
            if (!Enabled)
            {
                return;
            }

            FixOffsets();

            Quaternion targetRotation = GetCurrentRotation();

            if (!DoesRotationNeedUpdating(targetRotation))
            {
                return;
            }

            // The actual angle between the previous rotation and the current rotation.
            float angle = Quaternion.Angle(_previousTargetState.Rotation, targetRotation);

            // The t for lerping consistantly at the angles per second value.
            // If we're snapping, then we always do 1, otherwise, we do math.
            // We use mathf.abs because negative angles per second doesn't make sense.
            // I could force the AnglePerSecond value to change, but I decided to leave it incase people track angle per second values negatively here.
            float t = SnapRotation ? 1 : Mathf.Clamp01(Mathf.Abs(AnglePerSecond * Time.deltaTime) / angle);

            // We're going to be finished if t is equal to 1 (or greater, but because we're clamping that shouldn't be an issue.)
            _finishedAdjustment = t >= 1;

            if (t >= 1)
            {
                _forceAdjustment = false;
            }

            // Hit the t with the lerp transformer after we've done everything we have to with the real t value.
            // This honestly probably won't be used by many if any people, but the option is here if you want to mess with it.
            t = RotationLerpTransformer.Process(t);
            
            // Calculate the rotation we want for this update
            Quaternion rotation = Quaternion.Lerp(_previousTargetState.Rotation, targetRotation, t);

            // Actually rotate the camera
            CameraTransform.Rotation = rotation;
            CameraTransform.Rotate(CameraTransform.Right, YRotationOffset);
            CameraTransform.Rotate(_target.Target.up, -XRotationOffset);

            // Set the previous state information
            _previousTargetState.Rotation = rotation;
            _previousTargetState.Position = _target.Target.position;
            _previouslyDeterminedRotation = targetRotation;
        }
        
        /// <summary>
        /// Whether or not the FollowRotationComponent needs to rotate the camera towards the nuetral position.
        /// </summary>
        /// <param name="currentRotation">The current rotation of the target.</param>
        /// <returns>Rotate towards nuetral.</returns>
        private bool DoesRotationNeedUpdating(Quaternion currentRotation)
        {
            bool rotationNeedsUpdating = false;

            switch (Mode)
            {
                case FollowRotationMode.AdjustCompletelyOnRotation:
                    rotationNeedsUpdating = Quaternion.Angle(_previousTargetState.Rotation, currentRotation) >= RotationTolerance || !_finishedAdjustment;
                    break;
                case FollowRotationMode.AdjustCompletelyOnMovement:
                    rotationNeedsUpdating = Vector3.Distance(_previousTargetState.Position, _target.Target.position) > MovementTolerance || !_finishedAdjustment;
                    break;
                case FollowRotationMode.AdjustWhileMoving:
                    rotationNeedsUpdating = Vector3.Distance(_previousTargetState.Position, _target.Target.position) > MovementTolerance;
                    break;
            }

            return rotationNeedsUpdating || _forceAdjustment;
        }

        /// <summary>
        /// If the Offsets for rotation are adjusted at run time, this will fix the rotation.
        /// </summary>
        private void FixOffsets()
        {
            if (_previousXRotationOffset != XRotationOffset)
            {
                float diff = XRotationOffset - _previousXRotationOffset;
                _previousXRotationOffset = XRotationOffset;
                CameraTransform.Rotate(_target.Target.up, -diff);
            }

            if (_previousYRotationOffset != YRotationOffset)
            {
                float diff = YRotationOffset - _previousYRotationOffset;
                _previousYRotationOffset = YRotationOffset;
                CameraTransform.Rotate(CameraTransform.Right, diff);
            }
        }

        /// <summary>
        /// Returns the current rotation of the target.
        /// </summary>
        /// <returns>The target's rotation.</returns>
        private Quaternion GetCurrentRotation()
        {
            if (!DetermineRotationFromMovement)
            {
                _previouslyDeterminedRotation = _target.Target.rotation;

                return _target.Target.rotation;
            }
            else if (Vector3.Distance(_previousTargetState.Position, _target.Target.position) <= (DetermineTolerance == 0 ? FLOAT_TOLERANCE : DetermineTolerance))
            {
                return _previouslyDeterminedRotation;
            }
            else
            {
                VirtualTransform temp = new VirtualTransform(_previousTargetState.Position);
                temp.LookAt(_target.Target.position);

                if (FlattenRotationWhenDeterminingFromMovement)
                {
                    // This will keep things level with the up vector.
                    // If you go up a ramp, you won't angle the camera up the ramp with this.
                    Vector3 up = _target.Target.up.normalized;
                    Vector3 euler = temp.EulerAngles;
                    Vector3 eliminated = new Vector3(euler.x * (up.x), euler.y * (up.y), euler.z * (up.z));
                    temp.Rotation = Quaternion.Euler(eliminated);
                }

                return temp.Rotation;
            }
        }
    }
}
