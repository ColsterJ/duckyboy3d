using System;
using System.Collections.Generic;
using AdvancedUtilities.LerpTransformers;
using UnityEngine;

namespace AdvancedUtilities.Cameras.Components
{
    /// <summary>
    /// A target that when switched can lerp to the new target rather than snapping to it.
    /// </summary>
    [Serializable]
    public class SwitchTargetComponent : CameraComponent
    {
        /// <summary>
        /// The current target we want to be referencing.
        /// </summary>
        [Tooltip("The current target we want to be referencing.")]
        public Transform Target;

        /// <summary>
        /// The total number of seconds for a switch to occur.
        /// If ConstantSwitchSpeedInit is set, then it will be the number of units per second at the beginning of the switch.
        /// If target position's change, that speed may end up not being linear.
        /// </summary>
        [Tooltip("The total number of seconds for a switch to occur. " +
                 "If ConstantSwitchSpeedInit is set, then it will be the number of units per second at the beginning of the switch. " +
                 "If target position's change, that speed may end up not being linear.")]
        public float SwitchSpeed = 0.1f;

        /// <summary>
        /// Makes the switch speed during each switch a constant based off the switch speed.
        /// If transforms move to different locations in relation to each other, then the speed may not remain constant.
        /// </summary>
        [Tooltip("Makes the switch speed during each switch a constant based off the switch speed. " +
                 "If transforms move to different locations in relation to each other, then the speed may not remain constant.")]
        public bool ConstantSwitchSpeedInit = false;

        #region Publicly Accessible Properties

        /// <summary>
        /// This is the current list of offsets in the World Space that modify the target.
        /// This is a copied list, so that modifying it does not damage the offsets.
        /// </summary>
        public IList<Vector3> WorldSpaceOffsets
        {
            get
            {
                return new List<Vector3>(_worldSpaceOffsets);
            }
        }

        /// <summary>
        /// This is the sum of the World Space offsets
        /// </summary>
        public Vector3 WorldSpaceOffset
        {
            get
            {
                Vector3 offset = Vector3.zero;

                foreach (var additionalOffset in _worldSpaceOffsets)
                {
                    offset += additionalOffset;
                }

                return offset;
            }
        }

        /// <summary>
        /// This is the current list of offsets in the Local relative Space that modify the target.
        /// These offsets of relative to the Transform used for the Target and are modified by it's rotation.
        /// This is a copied list, so that modifying it does not damage the offsets.
        /// </summary>
        public IList<Vector3> LocalSpaceOffsets
        {
            get
            {
                return new List<Vector3>(_localSpaceOffsets);
            }
        }

        /// <summary>
        /// This is the sum of the Local relative Space offsets.
        /// This offsets is relative to the Transform used for the Target and are modified by it's rotation.
        /// </summary>
        public Vector3 LocalSpaceOffset
        {
            get
            {
                Vector3 offset = Vector3.zero;

                foreach (var additionalOffset in _localSpaceOffsets)
                {
                    offset += additionalOffset;
                }

                return offset;
            }
        }

        /// <summary>
        /// Modifies the way the lerping is transformed when switching targets.
        /// </summary>
        public ILerpTransformer LerpTransformer { get; set; }

        /// <summary>
        /// Whether or not the target is currently switching or not.
        /// </summary>
        public bool IsSwitching
        {
            get
            {
                return Time.time - _switchStartTime < _switchSpeed;
            }
        }

        #endregion

        #region Private Properties

        /// <summary>
        /// Offset list for world space offsets.
        /// </summary>
        private IList<Vector3> _worldSpaceOffsets;

        /// <summary>
        /// Offset list for local space offsets.
        /// </summary>
        private IList<Vector3> _localSpaceOffsets;

        /// <summary>
        /// The position that the last transform left off at.
        /// </summary>
        private Vector3 _previousPosition;

        /// <summary>
        /// The current transform target.
        /// </summary>
        private Transform _currentTarget;

        /// <summary>
        /// The speed the target switches at.
        /// </summary>
        private float _switchSpeed;

        /// <summary>
        /// The time that a switch starts.
        /// </summary>
        private float _switchStartTime;

        #endregion

        public override void Initialize(CameraController cameraController)
        {
            base.Initialize(cameraController);
            
            if (Target == null)
            {
                Debug.LogError("There is no target for this SwitchTargetComponent.");
            }

            _worldSpaceOffsets = new List<Vector3>();
            _localSpaceOffsets = new List<Vector3>();

            if (LerpTransformer == null)
            {
                LerpTransformer = new DoNothingLerpTransformer();
            }

            _currentTarget = Target;
            _previousPosition = Target.position + WorldSpaceOffset + Target.rotation * LocalSpaceOffset;

            _switchStartTime = -SwitchSpeed;
        }

        /// <summary>
        /// Returns the target's location.
        /// </summary>
        /// <returns>Target vector3.</returns>
        public Vector3 GetTarget()
        {
            Vector3 worldSpaceOffset = WorldSpaceOffset;
            Vector3 localSpaceOffset = LocalSpaceOffset;

            // If we need to switch, then we initialize it here
            // Because we are setting it up before we get the lerp target, if the speed is 0, we'll get the new position this update
            if (Target != _currentTarget)
            {
                Vector3 position = _currentTarget.position + worldSpaceOffset + _currentTarget.rotation*localSpaceOffset;
                _previousPosition = position;
                _currentTarget = Target;

                _switchStartTime = Time.time;
                _switchSpeed = SwitchSpeed;

                if (ConstantSwitchSpeedInit)
                {
                    Vector3 previous = _previousPosition;
                    Vector3 current = _currentTarget.position + worldSpaceOffset + _currentTarget.rotation * localSpaceOffset;
                    _switchSpeed = Vector3.Distance(previous, current) / SwitchSpeed;
                }
            }

            float t = Time.time - _switchStartTime;
            // Switch
            if (t < _switchSpeed)
            {
                t /= _switchSpeed;
                t = LerpTransformer.Process(t);

                Vector3 current = _currentTarget.position + worldSpaceOffset + _currentTarget.rotation*localSpaceOffset;

                return Vector3.Lerp(_previousPosition, current, t);
            }
            // Maintain
            else
            {
                _previousPosition = _currentTarget.position + worldSpaceOffset + _currentTarget.rotation * localSpaceOffset;

                return _previousPosition;
            }
        }

        #region Additional offset methods

        /// <summary>
        /// Adds a World Space offset that modifies where the target is in the 3D World Space.
        /// </summary>
        /// <param name="offset">Offset of the target to add.</param>
        public void AddWorldSpaceOffset(Vector3 offset)
        {
            _worldSpaceOffsets.Add(offset);
        }

        /// <summary>
        /// Adds a Local relative Space offset that modifies where the target is in the 3D Local Space.
        /// This offset factors in the Target Transform's rotation to maintain it's relativity.
        /// </summary>
        /// <param name="offset">Offset of the target to add.</param>
        public void AddLocalSpaceOffset(Vector3 offset)
        {
            _localSpaceOffsets.Add(offset);
        }

        /// <summary>
        /// Clears all current World Space offsets and Local Space offsets.
        /// </summary>
        public void ClearAdditionalOffsets()
        {
            _worldSpaceOffsets.Clear();
            _localSpaceOffsets.Clear();
        }

        /// <summary>
        /// Clears all World Space offsets.
        /// </summary>
        public void ClearWorldSpaceOffsets()
        {
            _worldSpaceOffsets.Clear();
        }

        /// <summary>
        /// Clears all Local Space offsets.
        /// </summary>
        public void ClearLocalSpaceOffsets()
        {
            _localSpaceOffsets.Clear();
        }

        #endregion
    }
}
