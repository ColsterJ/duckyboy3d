using System;
using System.Collections.Generic;
using UnityEngine;

namespace AdvancedUtilities.Cameras.Components
{
    /// <summary>
    /// This component describes the location in World Space where a target currently is.
    /// </summary>
    [Serializable]
    public class TargetComponent : CameraComponent
    {
        #region Public Editor Properties

        /// <summary>
        /// This will be the target that the camera will attempt to aim at.
        /// </summary>
        [Tooltip("This will be the target that the camera will attempt to aim at.")]
        public Transform Target;

        #endregion

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

        #endregion

        #region Private Properties

        /// <summary>
        /// The stored world offsets.
        /// </summary>
        private IList<Vector3> _worldSpaceOffsets;

        /// <summary>
        /// The stored local space offsets.
        /// </summary>
        private IList<Vector3> _localSpaceOffsets;

        #endregion

        public override void Initialize(CameraController cameraController)
        {
            base.Initialize(cameraController);

            _worldSpaceOffsets = new List<Vector3>();
            _localSpaceOffsets = new List<Vector3>();

            if (Target == null)
            {
                Debug.LogWarning("There is no Target for this TargetComponent.");
            }
        }

        #region Additional Offsets

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

        /// <summary>
        /// Returns the target that the camera should be looking at.
        /// </summary>
        /// <returns>The position in 3D space that the camera should be looking at.</returns>
        public Vector3 GetTarget()
        {
            Vector3 pos = Target.position;
            
            pos += WorldSpaceOffset;

            pos += (Target.rotation * LocalSpaceOffset);

            return pos;
        }

        /// <summary>
        /// Returns the actual distance from the Camera to the current target.
        /// This is the distance between the two positions in 3D space.
        /// </summary>
        /// <returns>Distance from camera position to the target position</returns>
        public float GetDistanceFromTarget()
        {
            return GetDistanceFromTarget(GetTarget());
        }

        /// <summary>
        /// Returns the actual distance from the Camera to the given target.
        /// This is the distance between the two positions in 3D space.
        /// </summary>
        /// <param name="target">The target you want to know how far the camera is from.</param>
        /// <returns>Distance from camera position to the target position</returns>
        public float GetDistanceFromTarget(Vector3 target)
        {
            return Vector3.Distance(CameraTransform.Position, target);
        }
    }
}
