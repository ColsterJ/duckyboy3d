using System;
using System.Collections.Generic;
using AdvancedUtilities.Cameras.Components;
using UnityEngine;

namespace AdvancedUtilities.Cameras
{
    /// <summary>
    /// The base class for creating CameraControllers.
    /// </summary>
    [Serializable]
    public abstract class CameraController : MonoBehaviour
    {
        #region Public Editor Members

        [Tooltip("The camera to be controlled.")]
        public Camera Camera;

        #endregion

        /// <summary>
        /// If you override the Awake method, be sure to call the base.Awake() in the new awake method.
        /// </summary>
        protected void Awake()
        {
            IsInitialized = false;

            if (Camera != null)
            {
                CameraTransform = new VirtualTransform();
            }
            else
            {
                throw new MissingMemberException("The Camera was not set on this CameraController. Please add a Camera reference to this CameraController.");
            }

            if (!ComponentsAdded)
            {
                _components = new Dictionary<Type, CameraComponent>();

                AddCameraComponents();

                ComponentsAdded = true;
            }

            InitializeComponents();
        }

        /// <summary>
        /// Represents the Transform of the Camera being controlled.
        /// </summary>
        public VirtualTransform CameraTransform { get; private set; }
        
        /// <summary>
        /// A dictionary of each component attached to this camera controller. 
        /// Each component type can only be attached once.
        /// </summary>
        private Dictionary<Type, CameraComponent> _components;

        /// <summary>
        /// Whether or not this CameraController has been initialized yet.
        /// If the CameraController has not been initialized, it's component's may not function properly.
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Returns whether or not the components for this controller have been added already or not.
        /// </summary>
        public bool ComponentsAdded { get; private set; }

        /// <summary>
        /// Performs the camera's update.
        /// </summary>
        public abstract void UpdateCamera();

        /// <summary>
        /// A method where camera components should be added.
        /// </summary>
        protected abstract void AddCameraComponents();

        /// <summary>
        /// Initializes the CameraController and the CameraComponents attached to it.
        /// </summary>
        protected void InitializeComponents()
        {
            foreach (CameraComponent component in _components.Values)
            {
                component.Initialize(this);
            }

            IsInitialized = true;
        }

        /// <summary>
        /// Add a component to this Camera Controller.
        /// </summary>
        /// <param name="component">The component to add.</param>
        public void AddCameraComponent(CameraComponent component)
        {
            if (ComponentsAdded)
            {
                throw new InvalidOperationException("You cannot add a component to the CameraController after components have been added.");
            }

            if (IsInitialized)
            {
                throw new InvalidOperationException("You cannot add a component to the CameraController after it has been initialized.");
            }

            Type type = component.GetType();

            if (_components.ContainsKey(type))
            {
                throw new ArgumentException(string.Format("The component of type '{0}' has already been added to this Camera Controller.", type));
            }

            _components.Add(type, component);
        }

        /// <summary>
        /// Sets the public fields of the matching attached component on this Camera Controller to the fields of the given component.
        /// This throws if the given component's type is not attached to the Camera Controller.
        /// This does not replace this controller's component with the given component.
        /// </summary>
        /// <param name="component">The given component whose public fields you want.</param>
        protected void CopyPublicFields(CameraComponent component)
        {
            Type type = component.GetType();

            if (!_components.ContainsKey(type))
            {
                throw new ArgumentException(string.Format("The component of type '{0}' has not yet been added to this Camera Controller.", type));
            }

            _components[type].CopyPublicFields(component);
        }

        /// <summary>
        /// For each Camera Component that is shared between this Camera Controller and the given Camera Controller, this will 
        /// copy the public fields from the given controller's components onto this controller's components.
        /// This does not replace this controller's components with the given controller's components.
        /// </summary>
        /// <param name="otherController">The controller whose component's you want to override this controller's components.</param>
        public void CopyPublicFields(CameraController otherController)
        {
            foreach (var component in _components.Values)
            {
                CameraComponent otherComponent;
                if (otherController._components.TryGetValue(component.GetType(), out otherComponent))
                {
                    component.CopyPublicFields(otherComponent);
                }
            }
        }

        /// <summary>
        /// Returns a CameraComponent attached to this CameraController
        /// </summary>
        /// <typeparam name="T">The type of the CameraComponent.</typeparam>
        /// <returns>The CameraComponent.</returns>
        public T GetCameraComponent<T>() where T : CameraComponent
        {
            Type type = typeof (T);

            if (!_components.ContainsKey(type))
            {
                throw new KeyNotFoundException(string.Format("The component type '{0}' was not attached to this Camera Controller.", type));
            }

            return _components[type] as T;
        }
    }
}