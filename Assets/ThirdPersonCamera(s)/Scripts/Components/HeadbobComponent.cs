using System;
using AdvancedUtilities.LerpTransformers;
using UnityEngine;

namespace AdvancedUtilities.Cameras.Components
{
    /// <summary>
    /// A component that allows the camera to perform a headbob like motion.
    /// </summary>
    [Serializable]
    public class HeadbobComponent : CameraComponent
    {
        #region Public Fields

        /// <summary>
        /// Whether Headbob is enabled or not. Headbobbing will happen at 0 distance if enabled.
        /// </summary>
        [Tooltip("Whether Headbob is enabled or not. Headbobbing will happen at 0 distance if enabled.")]
        public bool Enabled = false;

        /// <summary>
        /// If activated, the Headbob Component will bob regardless of the settings.
        /// </summary>
        [Tooltip("If activated, the Headbob Component will bob regardless of the settings.")]
        public bool Forced = false;

        /// <summary>
        /// If true, then headbobbing will occur at distances from the target up to the given Range, otherwise it will only happen at 0.
        /// </summary>
        [Tooltip("If true, then headbobbing will occur at distances from the target up to the given Range, otherwise it will only happen at 0.")]
        public bool EnableRangedActivation = false;

        /// <summary>
        /// The distance from the target that the camera will perform a headbobbing motion.
        /// </summary>
        [Tooltip("The distance from the target that the camera will perform a headbobbing motion.")]
        public float ActivationRange = 0f;

        /// <summary>
        /// The curve that the camera will headbob on modifying the target's position by the camera's up vector.
        /// </summary>
        [Tooltip("The curve that the camera will headbob on modifying the target's position by the camera's up vector.")]
        public AnimationCurve MagnitudeCurve = GetDefaultCurve();

        /// <summary>
        /// The total time it takes to complete the headbob curve.
        /// </summary>
        [Tooltip("The total time it takes to complete the headbob curve.")]
        public float DurationOfCurve = 0.5f;

        /// <summary>
        /// This number will be multiplied against the number of units the camera is offset by during headbobbing.
        /// </summary>
        [Tooltip("This number will be multiplied against the number of units the camera is offset by during headbobbing.")]
        public float ScaleOfCurve = 0.5f;

        /// <summary>
        /// When you stop headbobbing, the camera will readjust itself back to normal viewing levels. This is the total time it will take in seconds to do so.
        /// </summary>
        [Tooltip("When you stop headbobbing, the camera will readjust itself back to normal viewing levels. This is the total time it will take in seconds to do so.")]
        public float ReadjustToNormalTime = 0.5f;

        #endregion

        #region Public Properties

        /// <summary>
        /// Whether the camera is currently headbobbing or not.
        /// </summary>
        public bool IsHeadbobbing { get; private set; }

        /// <summary>
        /// That headbobbing is currently readjusting itself back to neutral after being disabled.
        /// </summary>
        public bool Reorienting { get; private set; }

        /// <summary>
        /// When the Headbobbing is reorienting back to normal, this transformer modifies the way it lerps back to normal.
        /// Altering this when the headbob component is reorienting may result is errors.
        /// </summary>
        public ILerpTransformer ReorientingLerpTransformer { get; set; }

        #endregion

        #region Private Fields & Propeties
        
        /// <summary>
        /// The time the camera began headbobbing.
        /// </summary>
        private float _headbobStartTime;

        /// <summary>
        /// The time the reorienting of the headbob started.
        /// </summary>
        private float _headbobReorientStartTime;

        /// <summary>
        /// The magnitude of the headbob when beginning to reorient back to neutral.
        /// </summary>
        private float _headbobReorientStartMagnitude;
        
        #endregion

        public override void Initialize(CameraController cameraController)
        {
            base.Initialize(cameraController);

            IsHeadbobbing = false;
            Forced = false;
            Reorienting = false;
            _headbobReorientStartTime = -ReadjustToNormalTime;

            ReorientingLerpTransformer = new DoNothingLerpTransformer();
        }

        /// <summary>
        /// Returns the offset needed to maintain headbobbing properly when both enabled and disabled.
        /// When disabled, this method will gradually reduce the offset to Vector3.zero.
        /// </summary>
        /// <param name="cameraDistanceFromTarget">The units the camera is currently from the target.</param>
        /// <returns>The offset needed to headbob.</returns>
        public Vector3 GetHeadbobModifier(float cameraDistanceFromTarget)
        {
            SetHeadbobState(cameraDistanceFromTarget);

            // Find the actual modifier and apply it
            Vector3 offset = GetHeadbobModifier();

            return offset;
        }

        /// <summary>
        /// Sets the current state of the Headbob component.
        /// This tells the component what it's supposed to be doing at any point.
        /// </summary>
        /// <param name="cameraDistanceFromTarget"></param>
        private void SetHeadbobState(float cameraDistanceFromTarget)
        {
            // Set up the state of the HeadbobComponent
            // Even if we are disabled, forced allows you to continue headbobbing
            if (Enabled || Forced)
            {
                // Forced ignores condition, 
                // otherwise we headbob when our distance is 'basically' 0, 
                // or we headbob when we're within activation range (and when activation range is enabled).
                if ( Forced ||
                     cameraDistanceFromTarget < FLOAT_TOLERANCE ||
                    (EnableRangedActivation && cameraDistanceFromTarget <= ActivationRange))
                {
                    // We require that you not be reorienting, because if you are there will be a jump.
                    // One solution to that would be to calculate a point where the reorienting and the animation curve are equal, but that would
                    // be difficult since you can provide custom curves and there's no guarentee. My suggestion is to use a low reorient time
                    // So that it happens quick enough to no be noticed if you decide to start bobbing again quickly.
                    if (!IsHeadbobbing && !Reorienting)
                    {
                        IsHeadbobbing = true;
                        _headbobStartTime = Time.time;
                    }
                }
                else
                {
                    TryStartReorienting();
                }
            }
            else
            {
                TryStartReorienting();
            }

            if (Time.time - _headbobReorientStartTime >= ReadjustToNormalTime)
            {
                Reorienting = false;
            }
        }

        /// <summary>
        /// Gets the Vector3 offset that should be applied to simulate headbobbing at any point in time.
        /// This take into account the headbobbing component being disabled and it reorienting itself to the neutral position.
        /// This does not handle setting up what state the Headbobbing component should be in.
        /// </summary>
        /// <returns>Offset of position in space to simulate headbobbing.</returns>
        private Vector3 GetHeadbobModifier()
        {
            float magnitude = GetHeadbobMagnitude();

            Vector3 offset = CameraTransform.Up * magnitude;

            return offset;
        }

        /// <summary>
        /// Sets the Headbob component to being reorienting back to neutral.
        /// </summary>
        private void TryStartReorienting()
        {
            // You don't start reorienting if you are already doing so.
            if (Reorienting)
            {
                return;
            }

            float currentMagnitude = GetHeadbobMagnitude();

            // If we're basically at 0, then there's no reason to reorient to 0
            if (currentMagnitude < FLOAT_TOLERANCE)
            {
                IsHeadbobbing = false;
                Reorienting = false;
            }
            // If we are not at 0, then we need to reorient to 0
            else
            {
                IsHeadbobbing = false;

                Reorienting = true;
                _headbobReorientStartTime = Time.time;
                _headbobReorientStartMagnitude = currentMagnitude;
            }
        }

        /// <summary>
        /// Gets the magnitude of the of the offset of the Headbob offset.
        /// </summary>
        /// <returns>Magnitude of what the offset should be.</returns>
        private float GetHeadbobMagnitude()
        {
            if (IsHeadbobbing)
            {
                float t = (Time.time - _headbobStartTime) / DurationOfCurve;
                float magnitude = MagnitudeCurve.Evaluate(t) * ScaleOfCurve;

                return magnitude;
            }
            else if (Reorienting)
            {
                float t = Mathf.Clamp01((Time.time - _headbobReorientStartTime) / ReadjustToNormalTime);
                t = ReorientingLerpTransformer.Process(t);
                float magnitude = Mathf.Lerp(_headbobReorientStartMagnitude, 0, t);

                return magnitude;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Constructs a default animation curve for the Headbob Component.
        /// </summary>
        /// <returns>Default animation curve.</returns>
        private static AnimationCurve GetDefaultCurve()
        {
            var animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

            animationCurve.preWrapMode = WrapMode.PingPong;
            animationCurve.postWrapMode = WrapMode.PingPong;

            return animationCurve;
        }
    }
}