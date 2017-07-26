using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace AdvancedUtilities.Cameras.Components
{
    /// <summary>
    /// A component that can be attached to a CameraController.
    /// 
    /// Fields that are publically accessible from the Editor should not be used to track information.
    /// Publically accessible fields should only be used to hold settings, so that copying public fields functions properly.
    /// </summary>
    public abstract class CameraComponent
    {
        /// <summary>
        /// Calculations by Camera Components involving floats will use this tolerance when comparing floats.
        /// </summary>
        public const float FLOAT_TOLERANCE = 0.001f;

        /// <summary>
        /// Information on camera component's public fields for reflection based field swapping.
        /// Indexed by the type of component that's fields are being saved.
        /// </summary>
        private readonly static IDictionary<Type, FieldInfo[]> _fields = new Dictionary<Type, FieldInfo[]>();

        /// <summary>
        /// The Camera that is being controlled by the CameraController.
        /// </summary>
        protected Camera Camera
        {
            get
            {
                return _cameraController.Camera;
            }
        }

        /// <summary>
        /// The VirtualTransform representing the Transform of the Camera.
        /// </summary>
        protected VirtualTransform CameraTransform
        {
            get
            {
                return _cameraController.CameraTransform;
            }
        }

        /// <summary>
        /// The camera controller that this component is attached to.
        /// </summary>
        private CameraController _cameraController;

        /// <summary>
        /// Sets up this component for the given Camera Controller that it is attached to.
        /// </summary>
        /// <param name="cameraController">The camera controller that this component is attached to.</param>
        public virtual void Initialize(CameraController cameraController)
        {
            _cameraController = cameraController;
        }

        /// <summary>
        /// Returns a CameraComponent attached to this Component's CameraController
        /// </summary>
        /// <typeparam name="T">The type of the CameraComponent.</typeparam>
        /// <returns>The CameraComponent.</returns>
        protected T GetCameraComponent<T>() where T : CameraComponent
        {
            if (_cameraController == null)
            {
                throw new NullReferenceException("The Camera Controller for this Component has not been set.");
            }

            return _cameraController.GetCameraComponent<T>();
        }

        /// <summary>
        /// Copies the given Camera Component's public fields to this Camera Components public fields.
        /// The Camera Component given must be the same type as this Camera Component.
        /// 
        /// The purpose of this method is to allow Camera Controllers to swap all their public variables of their components with the fields from a given component.
        /// This is useful for when you need to switch camera controller's and want to preserve the settings of individual components.
        /// </summary>
        /// <param name="component">The component whose properties you want. This must be the same type of component as the component you are setting it to.</param>
        public void CopyPublicFields(CameraComponent component)
        {
            Type otherType = component.GetType();
            Type thisType = this.GetType();

            if (otherType != thisType)
            {
                throw new ArgumentException(string.Format("The given component was of type '{0}' and not of type '{1}'.", otherType, thisType));
            }

            FieldInfo[] fields;
            if (!_fields.TryGetValue(thisType, out fields))
            {
                const System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance;

                fields = thisType.GetFields(flags);
                _fields.Add(thisType, fields);
            }

            foreach (var field in fields)
            {
                field.SetValue(this, field.GetValue(component));
            }
        }
    }
}