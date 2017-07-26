using UnityEngine;
using System;
using AdvancedUtilities.LerpTransformers;

namespace AdvancedUtilities.Cameras.Components
{
    /// <summary>
    /// The state of this component is persistant between calls for calculations, 
    /// however it will handle any adjustments that you make to it on the fly based off the values you provide it.
    /// </summary>
    [Serializable]
    public class ZoomComponent : CameraComponent
    {
        #region Public Fields

        /// <summary>
        /// When the camera needs to move inward to adjust for collision, this is the minimum the camera will adjust to,
        /// regardless if collision determines the distance should be a smaller value.
        /// </summary>
        [Tooltip("When the camera needs to move inward to adjust for collision, this is the minimum the camera will adjust to, " +
                 "regardless if collision determines the distance should be a smaller value.")]
        public float MinimumDistance = 0;

        /// <summary>
        /// Whether or not readjusting inwards will be instantanous or not.
        /// </summary>
        [Tooltip("Whether or not readjusting inwards will be instantanous or not.")]
        public bool SnapIn = false;

        /// <summary>
        /// Whether or not readjusting outwards will be instantanous or not.
        /// </summary>
        [Tooltip("Whether or not readjusting outwards will be instantanous or not.")]
        public bool SnapOut = false;

        /// <summary>
        /// If true, the camera will travel the given Zoom Speeds in units each second,
        /// rather than the entire distance in Speed number of seconds.
        /// Changing this value while the camera is zooming is not supported."
        /// </summary>
        [Tooltip("If true, the camera will travel the given Zoom Speeds in units each second, " +
                 "rather than the entire distance in Speed number of seconds. " +
                 "Changing this value while the camera is zooming is not supported.")]
        public bool UniformSpeed = false;

        /// <summary>
        /// The speed the camera moves into the target. Varies by which zooming setting you have set.
        /// </summary>
        [Tooltip("The speed the camera moves into the target. Varies by which zooming setting you have set.")]
        public float ZoomInSpeed = 0.5f;

        /// <summary>
        /// The speed the camera moves out from the target. Varies by which zooming setting you have set.
        /// </summary>
        [Tooltip("The speed the camera moves out from the target. Varies by which zooming setting you have set.")]
        public float ZoomOutSpeed = 0.5f;

        /// <summary>
        /// Each time the Desired Distance changes, a percentage of the total travel time will be added to the travel time.
        /// </summary>
        [Tooltip("Each time the Desired Distance changes, a percentage of the total travel time will be added to the travel time.")]
        public bool EnableAddPercentageOnScroll = true;

        /// <summary>
        /// Each time the Desired Distance changes, this percentage of the total travel time will be added to travel time.
        /// </summary>
        [Tooltip("Each time the Desired Distance changes, this percentage of the total travel time will be added to travel time.")]
        [Range(0, 1)]
        public float PercentageToAdd = 0.15f;

        #endregion

        #region Public Properties

        /// <summary>
        /// Whether the camera is currently scrolling in or out at all
        /// </summary>
        public bool IsZooming
        {
            get
            {
                return IsZoomingIn || IsZoomingOut;
            }
        }

        /// <summary>
        /// Whether the camera is currently scrolling in
        /// </summary>
        public bool IsZoomingIn { get; private set; }

        /// <summary>
        /// Whether the camera is currently scrolling out
        /// </summary>
        public bool IsZoomingOut { get; private set; }

        /// <summary>
        /// This transformer alters the way the zooming lerps to new desired distances.
        /// </summary>
        public ILerpTransformer ZoomLerpTransformer { get; private set; }

        #endregion

        #region Private Fields & Properties

        /// <summary>
        /// The time the camera begins scrolling to a new desired distance.
        /// </summary>
        private float _zoomStartTime;

        /// <summary>
        /// The distance the camera is at when it begins scrolling to a new desired distance.
        /// </summary>
        private float _zoomStartDistance;

        /// <summary>
        /// Each time the distance from the target is calculated, the value is set here to compare if it changed from call to call.
        /// </summary>
        private float _previousDesiredDistance;

        /// <summary>
        /// Each time the zoom component begins zooming, it will snapshot the current speed. 
        /// When it is adjusted mid-flight, this will be used to preserve smoothness.
        /// </summary>
        private float _previousZoomOutSpeed;

        /// <summary>
        /// Each time the zoom component begins zooming, it will snapshot the current speed. 
        /// When it is adjusted mid-flight, this will be used to preserve smoothness.
        /// </summary>
        private float _previousZoomInSpeed;

        #endregion

        public override void Initialize(CameraController cameraController)
        {
            base.Initialize(cameraController);

            ZoomLerpTransformer = new DoNothingLerpTransformer();
        }

        /// <summary>
        /// Calculates the distance that the camera should be from the target given.
        /// This factors in how far the camera should be from the target during the scrolling period.
        /// </summary>
        /// <param name="currentDistance">Current distance that camera is from the target.</param>
        /// <param name="calculatedDistance">The maximum calculated distance from the target at this point.</param>
        /// <param name="desiredDistance">The distance we want to move towards.</param>
        /// <returns></returns>
        public float CalculateDistanceFromTarget(float currentDistance, float calculatedDistance, float desiredDistance)
        {
            SetZoomingState(calculatedDistance, currentDistance, desiredDistance);
            AdjustForChanges(desiredDistance);

            float resultDistance;
            
            if (IsZooming)
            {
                float speed = IsZoomingIn
                    ? ZoomInSpeed
                    : ZoomOutSpeed;

                if (UniformSpeed)
                {
                    speed = IsZoomingIn
                    ? -ZoomInSpeed
                    : ZoomOutSpeed;

                    resultDistance = currentDistance + Time.deltaTime*speed;
                    // We can't be further out than the calculated maximum distance.
                    if (resultDistance > calculatedDistance)
                    {
                        resultDistance = calculatedDistance;
                    }
                }
                else
                {
                    float t = Mathf.Clamp01((Time.time - _zoomStartTime)/speed);

                    t = ZoomLerpTransformer.Process(t);

                    float lerped = Mathf.Lerp(_zoomStartDistance, desiredDistance, t);

                    resultDistance = Math.Min(lerped, calculatedDistance);
                }
            }
            else
            {
                resultDistance = Mathf.Min(calculatedDistance, desiredDistance);
            }

            // If we want to enforce a minimum no matter what even on collision with other things
            if ( resultDistance < MinimumDistance)
            {
                resultDistance = MinimumDistance;
            }

            return resultDistance;
        }
        
        /// <summary>
        /// Adjusts the ZoomComponent's targeted desired distance. 
        /// This adjusts the desired distance by a negative or positive amount
        /// </summary>
        /// <param name="delta">Amount you wish to add to the current desired distance.</param>
        private void AdjustDesiredDistance(float delta)
        {
            if (!IsZooming)
            {
                return;
            }

            // Adjust the previous desired distance, but don't go negative.
            if (_previousDesiredDistance + delta < 0)
            {
                delta = -_previousDesiredDistance;
            }

            _previousDesiredDistance += delta;

            // Recalculate starting distance to maintain smooth lerping.
            // The general idea is, if you add distance to the desired distance, then you remove distance from the start based off a fraction of the passed time.
            float t = IsZoomingIn
                ? (Time.time - _zoomStartTime) / ZoomInSpeed
                : (Time.time - _zoomStartTime) / ZoomOutSpeed;

            t = ZoomLerpTransformer.Process(t);

            float adjust = (-1.0f * delta * t) / (1 - t);

            _zoomStartDistance += adjust;
        }

        /// <summary>
        /// Adds an amount of time to the remaining travel time of the zoom. 
        /// The amount of time added is a percentage of the speed of the camera's current zooming.
        /// The provided delta is between [0,1].
        /// </summary>
        /// <param name="delta">Percentage of the current speed to add to the travel time of the camera.</param>
        public void AddPercentageToTimeRemaining(float delta)
        {
            if (!IsZooming)
            {
                return;
            }

            delta = Mathf.Clamp01(delta);

            float t = IsZoomingIn
                ? (Time.time - _zoomStartTime) / ZoomInSpeed
                : (Time.time - _zoomStartTime) / ZoomOutSpeed;
            
            // Limit value so that t doesn't become negative
            if (t - delta < 0)
            {
                delta = t;
            }

            float speed = IsZoomingIn
                ? ZoomInSpeed
                : ZoomOutSpeed;

            _zoomStartTime += delta * speed;

            float curDist = Mathf.Lerp(_zoomStartDistance, _previousDesiredDistance, t);

            t = IsZoomingIn
                ? (Time.time - _zoomStartTime) / ZoomInSpeed
                : (Time.time - _zoomStartTime) / ZoomOutSpeed;

            _zoomStartDistance = (curDist - _previousDesiredDistance * t) / (1 - t);
        }

        /// <summary>
        /// Modifies the Zoom Out Speed to the given value.
        /// </summary>
        /// <param name="value">The speed value you want to set the zoom out speed to.</param>
        public void SetZoomOutSpeed(float value)
        {
            // This method will be automatically called when zoom is calculated if the zoom speed was directly changed.
            // That means that ZoomOutSpeed is going to be different than _previousZoomInSpeed.
            // However if we call this method directly, they will be the same, so we want to make them different so we
            // can work under the assumption that _previousZoomOutSpeed != ZoomOutSpeed, and since we need to set it to that eventually anyway.

            ZoomOutSpeed = value;
            if (!IsZoomingOut || _previousZoomOutSpeed == value || UniformSpeed)
            {
                // If the value didn't actually change, or we're not zooming in, or we're using uniform speed, 
                // Then we don't need any calculations because we don't need to worry about these private fields when not zooming/changing, and uniform speed has a different formula it uses.
                return;
            }

            float t = (Time.time - _zoomStartTime) / _previousZoomOutSpeed;

            float adjustT = Time.time - _zoomStartTime - t * value;

            _zoomStartTime += adjustT;

            _previousZoomOutSpeed = value;
        }

        /// <summary>
        /// Modifies the Zoom In Speed to the given value.
        /// </summary>
        /// <param name="value">The speed value you want to set the zoom in speed to.</param>
        public void SetZoomInSpeed(float value)
        {
            // This method will be automatically called when zoom is calculated if the zoom speed was directly changed.
            // That means that ZoomInSpeed is going to be different than _previousZoomInSpeed.
            // However if we call this method directly, they will be the same, so we want to make them different so we
            // can work under the assumption that _previousZoomInSpeed != ZoomInSpeed, and since we need to set it to that eventually anyway.

            ZoomInSpeed = value;
            if (!IsZoomingIn || _previousZoomInSpeed == value || UniformSpeed)
            {
                // If the value didn't actually change, or we're not zooming in, or we're using uniform speed, 
                // Then we don't need any calculations because we don't need to worry about these private fields when not zooming/changing, and uniform speed has a different formula it uses.
                return;
            }
            
            float t = (Time.time - _zoomStartTime) / _previousZoomInSpeed;

            float adjustT = Time.time - _zoomStartTime - t * value;

            _zoomStartTime += adjustT;

            _previousZoomInSpeed = value;
        }

        /// <summary>
        /// Since the state of the Zoom Component percists, in order for things to remain smooth, when a change comes in, it needs to be adjusted for.
        /// </summary>
        /// <param name="desiredDistance">The desired distance being targeted by the newest calculation.</param>
        private void AdjustForChanges(float desiredDistance)
        {
            // None of this matters if we aren't zooming because speed and desired distance will have no baring.
            if (!IsZooming)
            {
                return;
            }

            // If the desired distance was changed since the last time it was called, that will affect the calculations and needs to be adjusted.
            // Note: We want exact values here, so no float tolerances.
            if (_previousDesiredDistance != desiredDistance)
            {
                AdjustDesiredDistance(desiredDistance - _previousDesiredDistance);

                if (EnableAddPercentageOnScroll)
                {
                    AddPercentageToTimeRemaining(PercentageToAdd);
                }
            }
            _previousDesiredDistance = desiredDistance;

            if (IsZoomingIn && _previousZoomInSpeed != ZoomInSpeed)
            {
                SetZoomInSpeed(ZoomInSpeed);
            }

            if (IsZoomingOut && _previousZoomOutSpeed != ZoomOutSpeed)
            {
                SetZoomOutSpeed(ZoomOutSpeed);
            }
        }

        /// <summary>
        /// Sets whether or not we are currently zooming in or out, as well as when the camera began zooming in or out.
        /// </summary>
        /// <param name="calculatedDistance">The calculated maximum distance from the target factoring in collision.</param>
        /// <param name="currentDistance">The current distance.</param>
        /// <param name="desiredDistance">The distance the camera wants to be at.</param>
        private void SetZoomingState(float calculatedDistance, float currentDistance, float desiredDistance)
        {
            // calculated < current
            if (currentDistance - calculatedDistance > FLOAT_TOLERANCE)
            {
                // snap in
                IsZoomingIn = false;
                IsZoomingOut = false;
            }
            // calculates == current && current > desired
            else if ((Math.Abs(calculatedDistance - currentDistance) < FLOAT_TOLERANCE) && (currentDistance - desiredDistance > FLOAT_TOLERANCE) && !SnapIn)
            {
                if (!IsZoomingIn)
                {
                    IsZoomingIn = true;
                    IsZoomingOut = false;

                    _zoomStartTime = Time.time;
                    _zoomStartDistance = currentDistance;
                    _previousDesiredDistance = desiredDistance;
                    _previousZoomInSpeed = ZoomInSpeed;
                }
            }
            // current < calculated && current < desired
            else if ((calculatedDistance - currentDistance > FLOAT_TOLERANCE) && (desiredDistance - currentDistance > FLOAT_TOLERANCE) && !SnapOut)
            {
                if (!IsZoomingOut)
                {
                    IsZoomingIn = false;
                    IsZoomingOut = true;

                    _zoomStartTime = Time.time;
                    _zoomStartDistance = currentDistance;
                    _previousDesiredDistance = desiredDistance;
                    _previousZoomOutSpeed = ZoomOutSpeed;
                }
            }
            else
            {
                // we are not changing distance
                IsZoomingIn = false;
                IsZoomingOut = false;
            }
        }
    }
}
