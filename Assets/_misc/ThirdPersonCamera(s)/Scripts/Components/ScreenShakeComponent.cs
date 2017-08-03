using System;
using AdvancedUtilities.Cameras.Components.Enums;
using UnityEngine;
using Random = System.Random;

namespace AdvancedUtilities.Cameras.Components
{
    /// <summary>
    /// A component that allows the camera to shake vertically, horizontally, or both at the same time.
    /// </summary>
    [Serializable]
    public class ScreenShakeComponent : CameraComponent
    {
        #region Public Fields

        /// <summary>
        /// Whether the screen is shaking or not.
        /// </summary>
        [Tooltip("Whether the screen is shaking or not.")]
        public bool Enabled = false;

        /// <summary>
        /// The type of shaking the Camera will do.
        /// </summary>
        [Tooltip("The type of shaking the Camera will do.")]
        public ScreenShakeMode Mode = ScreenShakeMode.HorizontalAndVertical;

        /// <summary>
        /// The intensity in units that the Camera will shake Horizontally.
        /// </summary>
        [Tooltip("The intensity in units that the Camera will shake Horizontally.")]
        public float HorizontalIntensity = 0.3f;

        /// <summary>
        /// The intensity in units that the Camera will shake Vertically.
        /// </summary>
        [Tooltip("The intensity in units that the Camera will shake Vertically.")]
        public float VerticalIntensity = 0.3f;

        /// <summary>
        /// Uses the specified seed for Random shaking rather than a time dependant seed.
        /// </summary>
        [Tooltip("Uses the specified seed for Random shaking rather than a time dependant seed.")]
        public bool UseSpecificSeed = true;

        /// <summary>
        /// A seed for Random shaking.
        /// </summary>
        [Tooltip("A seed for Random shaking.")]
        public int Seed = 0;

        /// <summary>
        /// Amount of seconds between each position being randomized.
        /// </summary>
        [Tooltip("Amount of seconds between each position being randomized.")]
        public float RandomizeTime = 0.035f;

        #endregion

        #region Private Fields & Propeties

        /// <summary>
        /// Random object for generating shaking.
        /// </summary>
        private Random _random;

        /// <summary>
        /// Previous values so we know when to generate new Random objects.
        /// </summary>
        private bool _previousUseSpecifiedSeed;

        /// <summary>
        /// The previous seed used during the last update.
        /// </summary>
        private int _previousSeed;

        /// <summary>
        /// The previous mode used during the last update.
        /// </summary>
        private ScreenShakeMode _previousMode;

        /// <summary>
        /// The previous offset that was returned.
        /// </summary>
        private Vector3 _previousShakeOffset = Vector3.zero;

        /// <summary>
        /// The time the last offset was created.
        /// </summary>
        private float _lastOffsetTime;

        #endregion

        public override void Initialize(CameraController cameraController)
        {
            base.Initialize(cameraController);

            _previousUseSpecifiedSeed = UseSpecificSeed;
            _previousSeed = Seed;
            _previousMode = Mode;

            _random = null;
        }

        /// <summary>
        /// Returns a random shaking offset Vector3.
        /// </summary>
        /// <returns>Offset for randomly shaking.</returns>
        public Vector3 GetShaking()
        {
            // Going to setup before hand so that when we stop shaking we invalidate the previous shaking.
            // Needs to happen before we return Vector3.zero so that it nulls the _random out.
            SetupRandom();

            if (!Enabled)
            {
                _previousShakeOffset = Vector3.zero;
                _lastOffsetTime = float.MinValue;

                return Vector3.zero;
            }

            Vector3 result = _previousShakeOffset;

            if (Time.time - _lastOffsetTime > RandomizeTime)
            {
                float h = (Mode == ScreenShakeMode.Horizontal || Mode == ScreenShakeMode.HorizontalAndVertical) ? (float)_random.NextDouble() - 0.5f : 0;
                float v = (Mode == ScreenShakeMode.Vertical || Mode == ScreenShakeMode.HorizontalAndVertical) ? (float)_random.NextDouble() - 0.5f : 0;
                
                result = (CameraTransform.Up * v * VerticalIntensity)
                       + (CameraTransform.Right * h * HorizontalIntensity);

                _previousShakeOffset = result;
                _lastOffsetTime = Time.time;
            }

            return result;
        }

        /// <summary>
        /// Sets the Random object up.
        /// </summary>
        private void SetupRandom()
        {
            if (!Enabled)
            {
                _random = null;

                return;
            }

            bool reset = false;

            reset |= _random == null;
            reset |= _previousSeed != Seed;
            reset |= _previousUseSpecifiedSeed != UseSpecificSeed;
            reset |= _previousMode != Mode;

            if (reset)
            {
                _random = GetRandom();

                _previousMode = Mode;
                _previousUseSpecifiedSeed = UseSpecificSeed;
                _previousSeed = Seed;
            }
        }

        /// <summary>
        /// Gets a Random object for creating random shaking.
        /// </summary>
        /// <returns>A Random object.</returns>
        private Random GetRandom()
        {
            if (UseSpecificSeed)
            {
                return new Random(Seed);
            }
            return new Random();
        }
    }
}
