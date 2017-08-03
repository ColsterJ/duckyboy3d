using System;
using AdvancedUtilities.LerpTransformers;
using UnityEngine;

namespace AdvancedUtilities.Cameras.Components
{
    /// <summary>
    /// A component that can take and process input values that can then be passed to other components.
    /// </summary>
    [Serializable]
    public class InputComponent : CameraComponent
    {
        #region Public Fields

        /// <summary>
        /// Whether or not horizontal input will be processed.
        /// </summary>
        [Tooltip("Whether or not horizontal input will be processed.")]
        public bool EnableHorziontal = true;

        /// <summary>
        /// Whether or not vertical input will be processed.
        /// </summary>
        [Tooltip("Whether or not vertical input will be processed.")]
        public bool EnableVertical = true;

        /// <summary>
        /// Whether or not zooming in input will be processed.
        /// </summary>
        [Tooltip("Whether or not zooming in input will be processed.")]
        public bool EnableZoomIn = true;

        /// <summary>
        /// Whether or not zooming out input will be processed.
        /// </summary>
        [Tooltip("Whether or not zooming out input will be processed.")]
        public bool EnableZoomOut = true;

        /// <summary>
        /// Encapsulates input sensitivity for the InputComponent.
        /// </summary>
        [Serializable]
        public class InputSensitivity
        {
            /// <summary>
            /// A value that adjusts the sensitivity of input meant to rotate the camera horizontally.
            /// </summary>
            [Tooltip("A value that adjusts the sensitivity of input meant to rotate the camera horizontally.")]
            public float Horizontal = 6f;

            /// <summary>
            /// A value that adjusts the sensitivity of input meant to rotate the camera veritcally.
            /// </summary>
            [Tooltip("A value that adjusts the sensitivity of input meant to rotate the camera veritcally.")]
            public float Vertical = 6f;

            /// <summary>
            /// A value that adjusts the sensitivity of input meant to zoom the camera in.
            /// </summary>
            [Tooltip("A value that adjusts the sensitivity of input meant to zoom the camera in.")]
            public float ZoomIn = 6f;

            /// <summary>
            /// A value that adjusts the sensitivity of input meant to zoom the camera out.
            /// </summary>
            [Tooltip("A value that adjusts the sensitivity of input meant to zoom the camera out.")]
            public float ZoomOut = 6f;
        }
        /// <summary>
        /// Sensitivity settings for this InputComponent.
        /// </summary>
        public InputSensitivity Sensitivity = new InputSensitivity();

        /// <summary>
        /// Encapsulates input inversion settings for the InputComponent.
        /// </summary>
        [Serializable]
        public class InputInversion
        {
            /// <summary>
            /// Horizontal input values will be multiplied by -1f when processed.
            /// </summary>
            [Tooltip("Horizontal input values will be multiplied by -1f when processed.")]
            public bool Horizontal = false;

            /// <summary>
            /// Vertical input values will be multiplied by -1f when processed.
            /// </summary>
            [Tooltip("Vertical input values will be multiplied by -1f when processed.")]
            public bool Vertical = true;

            /// <summary>
            /// ZoomIn input values will be multiplied by -1f when processed.
            /// </summary>
            [Tooltip("ZoomIn input values will be multiplied by -1f when processed.")]
            public bool ZoomIn = false;

            /// <summary>
            /// ZoomOut input values will be multiplied by -1f when processed.
            /// </summary>
            [Tooltip("ZoomOut input values will be multiplied by -1f when processed.")]
            public bool ZoomOut = false;
        }
        /// <summary>
        /// The input inversion settings for this InputComponent
        /// </summary>
        public InputInversion Invert = new InputInversion();

        /// <summary>
        /// Encapsulates input dimming to make rotation smoothing when being used.
        /// </summary>
        [Serializable]
        public class InputSmoothing
        {
            /// <summary>
            /// Settings for a specific input axis.
            /// </summary>
            [Serializable]
            public class Smoothing
            {
                [Tooltip("Whether this input is smoothed or not.")]
                public bool Enabled = false;

                [Tooltip("When you input 0 for an input value, it will consider that as valid. You will not smooth out if you input 0 values, only if you input null values.")]
                public bool Consider0AsValidInput = true;

                [Tooltip("The duration it takes to begin inputing.")]
                public float DurationIn = 0.15f;

                [Tooltip("The duration it takes to end inputing.")]
                public float DurationOut = 0.15f;

                [Tooltip("Value cannot be recieved that are greater than this value. This may have a smoothing effect based on how you're inputing.")]
                public float Limitation = 40;

                [Tooltip("Used to manipulate the smoothing progression as you begin inputing.")]
                public ILerpTransformer LerpIn = new DoNothingLerpTransformer();

                [Tooltip("Used to manipulate the smoothing progression as you exit inputing.")]
                public ILerpTransformer LerpOut = new DoNothingLerpTransformer();
            }

            [Tooltip("Must be anables for any of the smoothing to work.")]
            public bool Enabled = false;

            [Tooltip("Smoothing for horizontal input.")]
            public Smoothing Horizontal = new Smoothing();

            [Tooltip("Smoothing for vertical input.")]
            public Smoothing Vertical = new Smoothing();

            [Tooltip("Smoothing for zoom in input.")]
            public Smoothing ZoomIn = new Smoothing();

            [Tooltip("Smoothing for zoom out input.")]
            public Smoothing ZoomOut = new Smoothing();
        }
        [Tooltip("Smoothing settings for inputs.")]
        public InputSmoothing Smoothing = new InputSmoothing();

        #endregion

        #region Public Properties

        /// <summary>
        /// The current InputValues object on the InputComponent.
        /// This is the actual InputValues object used by the component.
        /// </summary>
        public InputValues Input { get; private set; }

        /// <summary>
        /// The processed InputValue modified by the settings on this InputComponent.
        /// This is a copy of the InputValues object used by the component and will not affect the component.
        /// </summary>
        public InputValues ProcessedInput
        {
            get
            {
                // Smooth after processing, since we want the sensitivity modified values, and smoothing may do nothing.
                return _smoothing.TrySmooth(ProcessInput(Input));
            }
        }

        #endregion

        #region Private Fields & Properties

        /// <summary>
        /// Keeps track of stuff for and handles smoothing.
        /// </summary>
        private InputSmoothingStatus _smoothing;

        #endregion

        public override void Initialize(CameraController cameraController)
        {
            base.Initialize(cameraController);

            Input = new InputValues();
            _smoothing = new InputSmoothingStatus(this);
        }

        /// <summary>
        /// Processes the given input and returns a newly created object with the input sensitivities, inversions, and enablings of this component setup properly.
        /// </summary>
        /// <param name="input">Input to process.</param>
        /// <returns></returns>
        public InputValues ProcessInput(InputValues input)
        {
            InputValues result = input.Copy();

            result.NullOutValues(!EnableHorziontal, !EnableVertical, !EnableZoomIn, !EnableZoomOut);
            result.MultiplyBy(Sensitivity.Horizontal, Sensitivity.Vertical, Sensitivity.ZoomIn, Sensitivity.ZoomOut);
            result.InvertValues(Invert.Horizontal, Invert.Vertical, Invert.ZoomIn, Invert.ZoomOut);

            return result;
        }

        #region Manipulation

        /// <summary>
        /// Sets the input for the InputComponent.
        /// This completely overrides the current input replacing it.
        /// </summary>
        /// <param name="input">Input you want to set it to.</param>
        public void SetInput(InputValues input)
        {
            if (input == null)
            {
                ClearInput();
                return;
            }

            Input = input;
        }

        /// <summary>
        /// Sets the input for the InputComponent.
        /// This completely overrides the current input replacing it.
        /// </summary>
        /// <param name="horizontal">Input for horizontal.</param>
        /// <param name="vertical">Input for vertical.</param>
        /// <param name="zoomIn">Input for zooming in.</param>
        /// <param name="zoomOut">Input for zooming out.</param>
        public void SetInput(float? horizontal, float? vertical, float? zoomIn, float? zoomOut)
        {
            InputValues input = new InputValues(horizontal, vertical, zoomIn, zoomOut);
            SetInput(input);
        }

        /// <summary>
        /// Clears the input values on the InputComponent.
        /// </summary>
        public void ClearInput()
        {
            Input = new InputValues();
        }

        /// <summary>
        /// Inverts the input of each input multiplying it by -1.
        /// </summary>
        public void InvertInput()
        {
            Input.InvertValues(true, true, true, true);
        }

        /// <summary>
        /// Applies all non-null input properties in the given Input to the existing input.
        /// If appendOver is set to true, then all non-null values will replace any values currently existing.
        /// If appendOver is set to false, then all non-null values will only replace null values in the current input.
        /// </summary>
        /// <param name="input">Input to apply properties under this objects properties.</param>
        /// <param name="appendOver">Appends these values over existing values.</param>
        public void AppendInput(InputValues input, bool appendOver = true)
        {
            if (appendOver)
            {
                Input.ApplyOver(input);
            }
            else
            {
                Input.ApplyUnder(input);
            }
        }

        /// <summary>
        /// Applies all non-null input properties in the given Input to the existing input.
        /// If appendOver is set to true, then all non-null values will replace any values currently existing.
        /// If appendOver is set to false, then all non-null values will only replace null values in the current input.
        /// </summary>
        /// <param name="horizontal">Input for horizontal.</param>
        /// <param name="vertical">Input for vertical.</param>
        /// <param name="zoomIn">Input for zooming in.</param>
        /// <param name="zoomOut">Input for zooming out.</param>
        /// <param name="appendOver">Appends these values over existing values.</param>
        public void AppendInput(float? horizontal, float? vertical, float? zoomIn, float? zoomOut, bool appendOver = true)
        {
            InputValues input = new InputValues(horizontal, vertical, zoomIn, zoomOut);
            AppendInput(input, appendOver);
        }

        /// <summary>
        /// Adds all non-null values to the current input.
        /// Null values on the current input become whatever the given input provides.
        /// </summary>
        /// <param name="input">Input you want to add.</param>
        public void AddInput(InputValues input)
        {
            AddInput(input.Horizontal, input.Vertical, input.ZoomIn, input.ZoomOut);
        }

        /// <summary>
        /// Adds all non-null values to the current input.
        /// Null values on the current input become whatever the given input provides.
        /// </summary>
        /// <param name="horizontal">Input for horizontal.</param>
        /// <param name="vertical">Input for vertical.</param>
        /// <param name="zoomIn">Input for zooming in.</param>
        /// <param name="zoomOut">Input for zooming out.</param>
        public void AddInput(float? horizontal, float? vertical, float? zoomIn, float? zoomOut)
        {
            Input.AddValues(horizontal, vertical, zoomIn, zoomOut);
        }

        /// <summary>
        /// Multiplies all non-null values to the current input.
        /// </summary>
        /// <param name="input">The given input containing the values to multiply by.</param>
        public void MultiplyInput(InputValues input)
        {
            MultiplyInput(input.Horizontal, input.Vertical, input.ZoomIn, input.ZoomOut);
        }

        /// <summary>
        /// Multiplies all non-null values to the current input.
        /// </summary>
        /// <param name="horizontal">Input for horizontal.</param>
        /// <param name="vertical">Input for vertical.</param>
        /// <param name="zoomIn">Input for zooming in.</param>
        /// <param name="zoomOut">Input for zooming out.</param>
        public void MultiplyInput(float? horizontal, float? vertical, float? zoomIn, float? zoomOut)
        {
            Input.MultiplyBy(horizontal, vertical, zoomIn, zoomOut);
        }

        /// <summary>
        /// Multiplies the current input by delta time.
        /// </summary>
        public void MultiplyInputByDeltaTime()
        {
            Input.MultiplyByDeltaTime();
        }

        /// <summary>
        /// Sets the horizontal input value to the given value.
        /// </summary>
        /// <param name="horizontal">Value to override with.</param>
        public void SetHorizontalInput(float? horizontal)
        {
            Input.Horizontal = horizontal;
        }

        /// <summary>
        /// Sets the vertical input value to the given value.
        /// </summary>
        /// <param name="vertical">Value to override with.</param>
        public void SetVerticalInput(float? vertical)
        {
            Input.Vertical = vertical;
        }

        /// <summary>
        /// Sets the ZoomIn input value to the given value.
        /// </summary>
        /// <param name="zoomIn">Value to override with.</param>
        public void SetZoomInInput(float? zoomIn)
        {
            Input.ZoomIn = zoomIn;
        }

        /// <summary>
        /// Sets the ZoomOut input value to the given value.
        /// </summary>
        /// <param name="zoomOut">Value to override with.</param>
        public void SetZoomOutInput(float? zoomOut)
        {
            Input.ZoomOut = zoomOut;
        }

        /// <summary>
        /// This will override any null values currently on the Input and set them to 0.
        /// </summary>
        public void SetNullInputValuesToZero()
        {
            Input.SetNullValuesToZero();
        }

        #endregion

        #region Smoothing
        
        /// <summary>
        /// Used for keeping track of stuff for smoothing inputs out.
        /// </summary>
        private class InputSmoothingStatus
        {
            public readonly SmoothingStatus Horizontal = new SmoothingStatus();
            public readonly SmoothingStatus Vertical = new SmoothingStatus();
            public readonly SmoothingStatus ZoomIn = new SmoothingStatus();
            public readonly SmoothingStatus ZoomOut = new SmoothingStatus();

            private readonly InputComponent _inputComponent;

            public InputSmoothingStatus(InputComponent inputComponent)
            {
                _inputComponent = inputComponent;
            }

            /// <summary>
            /// The status of a single axis for smoothing.
            /// </summary>
            public class SmoothingStatus
            {
                private float _timeStarted;
                private bool _lastUpdateHasInput;
                private float _lastValue;

                public float? TrySmooth(float? value, InputSmoothing.Smoothing smoothing)
                {
                    if (!smoothing.Enabled)
                    {
                        return value;
                    }
                    
                    bool valid = value.HasValue && (smoothing.Consider0AsValidInput || value.Value != 0);

                    if (valid)
                    {
                        if (!_lastUpdateHasInput)
                        {
                            // minus delta time so we get a first update right away
                            _timeStarted = Time.time - Time.deltaTime;
                        }
                        _lastUpdateHasInput = true;

                        if (value.Value > smoothing.Limitation)
                        {
                            value = smoothing.Limitation;
                        }

                        float t = Mathf.Clamp01((Time.time - _timeStarted) / smoothing.DurationIn);
                        float result = Mathf.Lerp(0, value.Value, t);
                        _lastValue = result;
                        return result;
                    }
                    else
                    {
                        if (_lastUpdateHasInput)
                        {
                            // minus delta time so we get a first update right away
                            _timeStarted = Time.time - Time.deltaTime;
                        }
                        _lastUpdateHasInput = false;

                        float t = Mathf.Clamp01((Time.time - _timeStarted) / smoothing.DurationIn);
                        return Mathf.Lerp(_lastValue, 0, t);
                    }
                }
            }

            /// <summary>
            /// Tries to smooth out input values if enabled.
            /// </summary>
            public InputValues TrySmooth(InputValues inputValues)
            {
                if (!_inputComponent.Smoothing.Enabled)
                {
                    return inputValues;
                }

                return new InputValues()
                {
                    Horizontal = Horizontal.TrySmooth(inputValues.Horizontal, _inputComponent.Smoothing.Horizontal),
                    Vertical = Vertical.TrySmooth(inputValues.Vertical, _inputComponent.Smoothing.Vertical),
                    ZoomIn = ZoomIn.TrySmooth(inputValues.ZoomIn, _inputComponent.Smoothing.ZoomIn),
                    ZoomOut = ZoomOut.TrySmooth(inputValues.ZoomOut, _inputComponent.Smoothing.ZoomOut),
                };
            }
        }

        #endregion
    }
}
