using System;
using System.Collections.Generic;
using AdvancedUtilities.LerpTransformers;
using UnityEngine;

namespace AdvancedUtilities.Cameras
{
    /// <summary>
    /// A camera controller that can control multiple other camera controllers and switch/lerp between them.
    /// </summary>
    [Serializable]
    public class MultiCameraController : CameraController
    {
        /// <summary>
        /// The Camera Controllers that are controlled by this MultiCamera Controller.
        /// </summary>
        [Tooltip("The Camera Controllers that are controlled by this MultiCamera Controller.")]
        public List<CameraController> CameraControllers;

        /// <summary>
        /// The Camera Controller in the list that is being used.
        /// </summary>
        [Tooltip("The Camera Controller in the list that is being used.")]
        public int CurrentIndex = 0;

        /// <summary>
        /// The number of seconds it takes to switch between cameras.
        /// </summary>
        [Tooltip("The number of seconds it takes to switch between cameras.")]
        public float SwitchSpeed = 2;

        /// <summary>
        /// When switching, if set, rotation will be Slerp'd instead of Lerp'd.
        /// </summary>
        [Tooltip("When switching, if set, rotation will be Slerp'd instead of Lerp'd.")]
        public bool SwitchSlerpRotation = true;

        /// <summary>
        /// When the lerping begins, the distance between position will be measured and used to calculate the duration of the lerp. 
        /// The TargetSwitchSpeed will represent the units/second that camera will lerp at when lerping begins.
        /// That speed is not guarenteed throughout the lerp unless the camera positions and rotation remain stationary.
        /// </summary>
        [Tooltip("When the lerping begins, the distance between position will be measured and used to calculate the duration of the lerp. " +
                 "The TargetSwitchSpeed will represent the units/second that camera will lerp at when lerping begins. " +
                 "That speed is not guarenteed throughout the lerp unless the camera positions and rotation remain stationary.")]
        public bool ConstantSwitchSpeedInit = false;

        #region Public Properties & Private Fields

        /// <summary>
        /// The current camera controller being controlled.
        /// </summary>
        public CameraController CurrentCameraController
        {
            get
            {
                return CameraControllers[CurrentIndex];
            }
        }

        /// <summary>
        /// Whether or not the target is currently switching or not.
        /// </summary>
        public bool IsSwitching
        {
            get
            {
                return Time.time - _switchBeginTime < _switchSpeed;
            }
        }

        /// <summary>
        /// This transformer alters the way the position lersp to new positions.
        /// </summary>
        public ILerpTransformer PositionLerpTransformer { get; private set; }

        /// <summary>
        /// This transformer alters the way therotation lerps to new positions.
        /// </summary>
        public ILerpTransformer RotationLerpTransformer { get; private set; }

        /// <summary>
        /// The current camera being focused on.
        /// </summary>
        private int _currentCameraIndex = 0;
        
        /// <summary>
        /// The time that the current switch began at.
        /// </summary>
        private float _switchBeginTime;

        /// <summary>
        /// The position that the camera was at when the switch began.
        /// </summary>
        private VirtualTransform _switchTransform;

        /// <summary>
        /// The speed the the switch will lerp at.
        /// </summary>
        private float _switchSpeed;

        #endregion

        protected override void AddCameraComponents()
        {
            // Nothing
        }

        void Start()
        {
            if (this.CameraControllers.Count == 0)
            {
                this.enabled = false;
                throw new ArgumentException("There are no camera controllers attached to this Multi CameraController.");
            }

            if (PositionLerpTransformer == null)
            {
                PositionLerpTransformer = new DoNothingLerpTransformer();
            }
            if (RotationLerpTransformer == null)
            {
                RotationLerpTransformer = new DoNothingLerpTransformer();
            }

            _currentCameraIndex = CurrentIndex;
            _switchBeginTime = -SwitchSpeed;

            if (CurrentIndex < 0 || CurrentIndex > CameraControllers.Count + 1)
            {
                this.enabled = false;
                throw new ArgumentOutOfRangeException("The index: " + CurrentIndex + " is not a valid index for a Camera.");
            }

            foreach (var cam in CameraControllers)
            {
                cam.enabled = false;
            }
        }
        
        void Update()
        {
            UpdateCamera();

            CameraTransform.ApplyTo(Camera); // Apply the virtual transform to the actual transform
        }

        public override void UpdateCamera()
        {
            // This was a public variable can switch the camera.
            if (CurrentIndex != _currentCameraIndex)
            {
                SwitchCamera(CurrentIndex);
            }

            CameraController cam = CameraControllers[_currentCameraIndex];

            cam.UpdateCamera();

            // Lerping, we have to calculate between positions.
            if (Time.time - _switchBeginTime < _switchSpeed)
            {
                float t = (Time.time - _switchBeginTime) / _switchSpeed;
                float pt = PositionLerpTransformer.Process(t);
                float rt = RotationLerpTransformer.Process(t);

                var lerpVirtualTransform = new VirtualTransform();

                // Lerp position
                lerpVirtualTransform.Position = Vector3.Lerp(_switchTransform.Position, cam.CameraTransform.Position, pt);

                // Lerp rotation
                lerpVirtualTransform.Rotation = SwitchSlerpRotation ? 
                    Quaternion.Slerp(_switchTransform.Rotation, cam.CameraTransform.Rotation, rt) : 
                    Quaternion.Lerp(_switchTransform.Rotation, cam.CameraTransform.Rotation, rt);

                lerpVirtualTransform.ApplyTo(CameraTransform);
            }
            // Not Lerping, we just set the position.
            else
            {
                cam.CameraTransform.ApplyTo(CameraTransform);
            }
        }

        /// <summary>
        /// Switches the active camera to the given index.
        /// </summary>
        /// <param name="index">Index in the list of cameras.</param>
        /// <param name="copyComponents">Whether or not components fields will be copied from the current camera to the next.</param>
        public void SwitchCamera(int index, bool copyComponents = false)
        {
            if (index < 0 || index > CameraControllers.Count - 1)
            {
                throw new ArgumentOutOfRangeException("The index: "+index +" is not a valid index for a Camera.");
            }

            CameraController previousCameraController = CameraControllers[_currentCameraIndex];

            // Setup out new index, and the position we left off of
            _switchTransform = new VirtualTransform(CameraTransform);
            _currentCameraIndex = index;
            _switchBeginTime = Time.time;

            // Copy the fields before we initialize
            if (copyComponents)
            {
                CameraControllers[_currentCameraIndex].CopyPublicFields(previousCameraController);
            }

            // We want the camera to initialize, but we don't want it actually updating. Enabling and disabling will do that.
            CameraControllers[_currentCameraIndex].enabled = true;
            CameraControllers[_currentCameraIndex].enabled = false;

            // Setting up the speed after the new camera has been initialized so that we have the correct position for measurements.
            // If we're at 0 or less, then set our position to the new camera's position immediately.
            if (SwitchSpeed <= 0)
            {
                // Snap if equal or less than 0
                _switchSpeed = float.MaxValue;
                CameraControllers[_currentCameraIndex].CameraTransform.ApplyTo(CameraTransform);
            }
            // Otherwise, we set up the speed.
            else
            {
                _switchSpeed = SwitchSpeed;
                if (ConstantSwitchSpeedInit)
                {
                    _switchSpeed = Vector3.Distance(_switchTransform.Position, CameraControllers[_currentCameraIndex].CameraTransform.Position) / SwitchSpeed;
                }
            }
        }

        /// <summary>
        /// Switches the active camera to the first instance of the given type found in the CameraControllers list.
        /// </summary>
        /// <typeparam name="T">Type of Camera to switch to.</typeparam>
        /// <param name="copyComponents">Whether or not components fields will be copied from the current camera to the next.</param>
        public void SwitchCamera<T>(bool copyComponents = false)
        {
            for (int i = 0; i < CameraControllers.Count; i++)
            {
                if (typeof(T) == CameraControllers[i].GetType())
                {
                    SwitchCamera(i, copyComponents);
                    return;
                }
            }

            throw new ArgumentException("There is no Camera in the CameraControllers list of type: "+ typeof(T));
        }
    }
}
