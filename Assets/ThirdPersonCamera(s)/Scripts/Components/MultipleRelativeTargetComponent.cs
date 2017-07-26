using System;
using System.Collections.Generic;
using AdvancedUtilities.LerpTransformers;
using UnityEngine;

namespace AdvancedUtilities.Cameras.Components
{
    /// <summary>
    /// A complex target system that can choose a target from a list of targets based off the proximity 
    /// of each target to a world space point, and can lerp to the location of new targets.
    /// </summary>
    [Serializable]
    public class MultipleRelativeTargetComponent : CameraComponent
    {
        /// <summary>
        /// Possible targets for the component to choose from. Local space offsets will use each individual transform's rotation when factoring it in.
        /// </summary>
        [Tooltip("Possible targets for the component to choose from. Local space offsets will use each individual transform's rotation when factoring it in.")]
        public List<Transform> Targets;

        /// <summary>
        /// When getting the target, this will determine the target. Closest will get the target closest to a given world space point.
        /// </summary>
        [Tooltip("When getting the target, this will determine the target. Closest will get the target closest to a given world space point.")]
        public MultipleRelativeTargetSwitchMode SwitchMode = MultipleRelativeTargetSwitchMode.Closest;

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
        /// The current target won't be switched regardless of the mode.
        /// </summary>
        [Tooltip("The current target won't be switched regardless of the mode.")]
        public bool MaintainTarget = false;

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
        /// The target before this target.
        /// </summary>
        private Transform _previousTarget;

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

            if (Targets.Count == 0)
            {
                Debug.LogError("There is no target for this MultipleRelativeTargetComponent.");
            }

            _worldSpaceOffsets = new List<Vector3>();
            _localSpaceOffsets = new List<Vector3>();

            if (LerpTransformer == null)
            {
                LerpTransformer = new DoNothingLerpTransformer();
            }

            _switchStartTime = -SwitchSpeed;

            _previousTarget = null;
            _currentTarget = null;
        }

        #region Target(s)

        /// <summary>
        /// Gets a random target.
        /// </summary>
        /// <returns>Random target.</returns>
        public Vector3 GetTarget()
        {
            // We want to use random for this call
            MultipleRelativeTargetSwitchMode currentMode = SwitchMode;
            SwitchMode = MultipleRelativeTargetSwitchMode.Random;

            Vector3 target = GetTarget(Vector3.zero);;

            SwitchMode = currentMode;

            return target;
        }

        /// <summary>
        /// Gets a random target. You can override the current maintain target setting for this call.
        /// </summary>
        /// <param name="maintainTarget">Temporary override for maintain target.</param>
        /// <returns>Random target, or maintained target.</returns>
        public Vector3 GetTarget(bool maintainTarget)
        {
            // We want to override maintain target, but only temporarily
            bool currentMaintainTarget = MaintainTarget;
            MaintainTarget = maintainTarget;

            // We want to use random for this call
            MultipleRelativeTargetSwitchMode currentMode = SwitchMode;
            SwitchMode = MultipleRelativeTargetSwitchMode.Random;

            Vector3 target = GetTarget(Vector3.zero);

            MaintainTarget = currentMaintainTarget;
            SwitchMode = currentMode;

            return target;
        }

        /// <summary>
        /// Gets the target based off the current mode in relation to the world space point given.
        /// Maintain target can be overridden for this call.
        /// </summary>
        /// <param name="worldSpacePoint">Position to determine position by.</param>
        /// <param name="maintainTarget">Temporary override for maintain target.</param>
        /// <returns>Position of the target.</returns>
        public Vector3 GetTarget(Vector3 worldSpacePoint, bool maintainTarget)
        {
            // We want to override maintain target, but only temporarily
            bool currentMaintainTarget = MaintainTarget;
            MaintainTarget = maintainTarget;

            Vector3 target = GetTarget(worldSpacePoint);

            MaintainTarget = currentMaintainTarget;

            return target;
        }

        /// <summary>
        /// Gets the target in relation to the world space.
        /// </summary>
        /// <param name="worldSpacePoint">Point in world space to determine the target by.</param>
        /// <returns>The current target's position in world space.</returns>
        public Vector3 GetTarget(Vector3 worldSpacePoint)
        {
            Vector3 worldSpaceOffset = WorldSpaceOffset;
            Vector3 localSpaceOffset = LocalSpaceOffset;

            float t = Time.time - _switchStartTime;
            if (t < _switchSpeed)
            {
                t /= _switchSpeed;
                t = LerpTransformer.Process(t);

                Vector3 previous = _previousTarget.position + worldSpaceOffset + _previousTarget.rotation * localSpaceOffset;
                Vector3 current = _currentTarget.position + worldSpaceOffset + _currentTarget.rotation * localSpaceOffset;

                return Vector3.Lerp(previous, current, t);
            }
            else
            {
                if (MaintainTarget && _currentTarget != null)
                {
                    return _currentTarget.position + worldSpaceOffset + _currentTarget.rotation * localSpaceOffset;
                }
                else
                {
                    // Get the target
                    Transform target;
                    switch (SwitchMode)
                    {
                        case MultipleRelativeTargetSwitchMode.Closest:
                            target = GetTargetClosestOrFurthest(worldSpacePoint, true);
                            break;
                        case MultipleRelativeTargetSwitchMode.Furthest:
                            target = GetTargetClosestOrFurthest(worldSpacePoint, false);
                            break;
                        case MultipleRelativeTargetSwitchMode.Random:
                            target = GetRandomTarget();
                            break;
                        default:
                            throw new InvalidOperationException("The switch mode must be set to closest, furthest, or random.");
                    }

                    if (target == null)
                    {
                        throw new InvalidOperationException("No target could be found.");
                    }

                    // If it's the same target, then just return the position
                    if (target == _currentTarget)
                    {
                        return _currentTarget.position + worldSpaceOffset + _currentTarget.rotation*localSpaceOffset;
                    }
                    // Else switch
                    else
                    {
                        _previousTarget = _currentTarget;
                        _currentTarget = target;

                        // Instant switch if speed is 0 or less, or no previous target since it's our first target get
                        if (SwitchSpeed <= 0 || _previousTarget == null)
                        {
                            return _currentTarget.position + worldSpaceOffset + _currentTarget.rotation * localSpaceOffset;
                        }
                        // Lerped switch otherwise, and return the previous position since we're at time 0.
                        else
                        {
                            _switchStartTime = Time.time;
                            _switchSpeed = SwitchSpeed;

                            if (ConstantSwitchSpeedInit)
                            {
                                Vector3 previous = _previousTarget.position + worldSpaceOffset + _previousTarget.rotation * localSpaceOffset;
                                Vector3 current = _currentTarget.position + worldSpaceOffset + _currentTarget.rotation * localSpaceOffset;
                                _switchSpeed = Vector3.Distance(previous, current) / SwitchSpeed;
                            }

                            return _previousTarget.position + worldSpaceOffset + _previousTarget.rotation * localSpaceOffset;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns the Transform target that is closest or furthest from the world space position.
        /// </summary>
        /// <param name="worldSpacePosition">Position to measure in respect to.</param>
        /// <param name="getClosest">true: get closest, false: get furthest.</param>
        /// <returns>Transform that is closest or furthest from the world space position.</returns>
        private Transform GetTargetClosestOrFurthest(Vector3 worldSpacePosition, bool getClosest)
        {
            Vector3 worldSpaceOffset = WorldSpaceOffset;
            Vector3 localSpaceOffset = LocalSpaceOffset;

            Transform currentTarget = Targets[0];
            Vector3 currentPosition = currentTarget.position + worldSpaceOffset + currentTarget.rotation * localSpaceOffset;
            float currentDistance = Vector3.Distance(worldSpacePosition, currentPosition);

            for (int i = 0; i < Targets.Count; i++)
            {
                Transform target = Targets[i];
                Vector3 position = target.position + worldSpaceOffset + target.rotation * localSpaceOffset;
                float distance = Vector3.Distance(position, worldSpacePosition);
                if (getClosest && distance < currentDistance)
                {
                    currentDistance = distance;
                    currentTarget = target;
                }
                else if (!getClosest && distance > currentDistance)
                {
                    currentDistance = distance;
                    currentTarget = target;
                }
            }

            return currentTarget;
        }

        /// <summary>
        /// Returns a random target.
        /// </summary>
        /// <returns>A random transform from the target.</returns>
        private Transform GetRandomTarget()
        {
            return Targets[UnityEngine.Random.Range(0, Targets.Count)];
        }

        #endregion

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
