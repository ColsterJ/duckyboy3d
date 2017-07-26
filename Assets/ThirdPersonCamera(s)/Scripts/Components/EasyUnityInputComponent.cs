using System;
using System.Collections.Generic;
using UnityEngine;

namespace AdvancedUtilities.Cameras.Components
{
    /// <summary>
    /// A component to automatically grab input in a simple way for Camera Controllers to use without the need to create InputValues objects and pass them manually.
    /// </summary>
    [Serializable]
    public class EasyUnityInputComponent : CameraComponent
    {
        #region Public Fields

        /// <summary>
        /// Whether or not this component will do anything when it's called to append its input.
        /// </summary>
        [Tooltip("Whether or not this component will do anything when it's called to append its input.")]
        public bool Enabled = true;

        /// <summary>
        /// Easy input will be appended over other input. If false, it will be appended under.
        /// </summary>
        [Tooltip("Easy input will be appended over other input. If false, it will be appended under.")]
        public bool AppendOver = false;

        [Tooltip("While the mouse input is down, 0 values will be passed instead of null when no input is present.")]
        public bool Use0NotNullWhileMouseInputDown = true;

        /// <summary>
        /// The axis input used to rotate the camera's view horizontally.
        /// </summary>
        [Tooltip("The axis input used to rotate the camera's view horizontally.")]
        public string HorizontalInputName = "Mouse X";

        /// <summary>
        /// The axis input used to rotate the camera's view vertically.
        /// </summary>
        [Tooltip("The axis input used to rotate the camera's view vertically.")]
        public string VerticalInputName = "Mouse Y";

        /// <summary>
        /// The axis input used to scroll the distance inwards.
        /// </summary>
        [Tooltip("The axis input used to scroll the distance inwards.")]
        public string ZoomInInputName = "Mouse ScrollWheel";

        /// <summary>
        /// The axis input used to scroll the distance outwards.
        /// </summary>
        [Tooltip("The axis input used to scroll the distance outwards.")]
        public string ZoomOutInputName = "Mouse ScrollWheel";

        /// <summary>
        /// Horizontal input will be found using the given input name.
        /// </summary>
        [Tooltip("Horizontal input will be found using the given input name.")]
        public bool EnableHorizontal = true;

        /// <summary>
        /// Vertical input will be found using the given input name.
        /// </summary>
        [Tooltip("Vertical input will be found using the given input name.")]
        public bool EnableVertical = true;

        /// <summary>
        /// ZoomIn input will be found using the given input name.
        /// </summary>
        [Tooltip("ZoomIn input will be found using the given input name.")]
        public bool EnableZoomIn = true;

        /// <summary>
        /// ZoomOut input will be found using the given input name.
        /// </summary>
        [Tooltip("ZoomOut input will be found using the given input name.")]
        public bool EnableZoomOut = true;

        /// <summary>
        /// The input name for the button to support EnableRotationOnlyWhenMousePressed.
        /// </summary>
        [Tooltip("The input name for the button to support EnableRotationOnlyWhenMousePressed.")]
        public string MouseInputButtonName = "Fire1";

        /// <summary>
        /// When true, the input will only pass Horizontal and Vertical input when the mouse button specified is pressed.
        /// </summary>
        [Tooltip("When true, the input will only pass Horizontal and Vertical input when the mouse button specified is pressed.")]
        public bool EnableRotationOnlyWhenMousePressed = true;

        /// <summary>
        /// When true, the input will only pass Horizontal and Vertical input when the cursor is locked.
        /// </summary>
        [Tooltip("When true, the input will only pass Horizontal and Vertical input when the cursor is locked.")]
        public bool EnableRotationOnlyWhenCursorLocked = true;

        /// <summary>
        /// Input will disabled when the pointer meets the conditions for the PointerOverGui object.
        /// </summary>
        [Tooltip("Input will disabled when the pointer meets the conditions for the PointerOverGui object.")]
        public PointerOverGui DisableInputWhenOver = new PointerOverGui();

        #endregion

        #region Private Fields & Properties

        /// <summary>
        /// A privately stored input component for use by this class for appending.
        /// </summary>
        private InputComponent _input;

        #endregion

        public override void Initialize(CameraController cameraController)
        {
            base.Initialize(cameraController);

            _input = GetCameraComponent<InputComponent>();
        }

        /// <summary>
        /// Appends the Easy input to the current Input component.
        /// </summary>
        public void AppendInput()
        {
            if (!Enabled)
            {
                return;
            }

            InputValues input = GetEasyUnityInput();

            EnforceRotationRestrictions(input);
            Enforce0NotNull(input);

            if (AppendOver)
            {
                _input.Input.ApplyOver(input);
            }
            else
            {
                _input.Input.ApplyUnder(input);
            }
        }

        /// <summary>
        /// Reads and returns the input from the easy input.
        /// </summary>
        /// <returns>Easy to setup input.</returns>
        private InputValues GetEasyUnityInput()
        {
            if (DisableInputWhenOver.IsPointerOverGui())
            {
                return new InputValues();
            }

            float? horizontal = Input.GetAxis(HorizontalInputName);
            float? vertical = Input.GetAxis(VerticalInputName);
            float? zoomIn = -1f * Input.GetAxis(ZoomInInputName);
            float? zoomOut = -1f * Input.GetAxis(ZoomOutInputName);

            // Mainly for ScrollWheel use, but if you use the same name for both, 
            // then I'll assume that zoomIn should be negative and zoomout positive (or the other way around since I multiply them by -1) 
            if (string.Equals(ZoomInInputName, ZoomOutInputName))
            {
                if (zoomIn.Value >= 0)
                {
                    zoomIn = null;
                }
                if (zoomOut <= 0)
                {
                    zoomOut = null;
                }
            }

            return new InputValues()
            {
                Horizontal = horizontal != 0 && EnableHorizontal ? horizontal : null,
                Vertical = vertical != 0 && EnableVertical ? vertical : null,
                ZoomIn = zoomIn != 0 && EnableZoomIn ? zoomIn : null,
                ZoomOut = zoomOut != 0 && EnableZoomOut ? zoomOut : null
            };
        }

        /// <summary>
        /// Modifies the given InputValues object based on the component's settings so that
        /// it only can rotate when a mouse button is pushed if that is specified.
        /// </summary>
        /// <param name="input"></param>
        private void EnforceRotationRestrictions(InputValues input)
        {
            if (!EnableRotationOnlyWhenMousePressed && !EnableRotationOnlyWhenCursorLocked)
            {
                return;
            }

            // Button isn't pushed, so null out rotation.
            if (EnableRotationOnlyWhenMousePressed && !Input.GetButton(MouseInputButtonName))
            {
                input.Horizontal = null;
                input.Vertical = null;
            }

            // Cursor isn't locked, so null out rotation.
            else if (EnableRotationOnlyWhenCursorLocked && Cursor.lockState == CursorLockMode.None)
            {
                input.Horizontal = null;
                input.Vertical = null;
            }
        }

        /// <summary>
        /// Forces null values to be 0 instead if the mouse is down and it's set to do that.
        /// </summary>
        /// <param name="input">Input to enforce</param>
        private void Enforce0NotNull(InputValues input)
        {
            if (!Use0NotNullWhileMouseInputDown || !Input.GetButton(MouseInputButtonName))
            {
                return;
            }

            input.Horizontal = input.Horizontal.HasValue ? input.Horizontal.Value : 0;
            input.Vertical = input.Vertical.HasValue ? input.Vertical.Value : 0;
            input.ZoomIn = input.ZoomIn.HasValue ? input.ZoomIn.Value : 0;
            input.ZoomOut = input.ZoomOut.HasValue ? input.ZoomOut.Value : 0;
        }
    }
}