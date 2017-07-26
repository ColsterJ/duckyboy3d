using System;
using System.Collections.Generic;
using UnityEngine;

namespace AdvancedUtilities.Cameras.Components
{
    /// <summary>
    /// This component calculates positions using the Camera's rotation and provided values to determine if there are any game objects
    /// between the target and a position outside of the target.
    /// </summary>
    [Serializable]
    public class ViewCollisionComponent : CameraComponent
    {
        #region Public Fields

        /// <summary>
        /// The camera will take collision into account when determining how far away from the target it should be.
        /// </summary>
        [Tooltip("The camera will take collision into account when determining how far away from the target it should be.")]
        public bool Enabled = true;

        /// <summary>
        /// These layers will indicate layers that are considered line of sight blockers and will force the camera forward.
        /// </summary>
        [Tooltip("These layers will indicate layers that are considered line of sight blockers and will force the camera forward.")]
        public LayerMask LineOfSightLayers;

        /// <summary>
        /// These GameObjects will be ignored when handling view collision even if they are apart of the LineOfSightLayers LayerMask.
        /// </summary>
        [Tooltip("These GameObjects will be ignored when handling view collision even if they are apart of the LineOfSightLayers LayerMask.")]
        [SerializeField]
        public List<GameObject> IgnoreTheseGameObjects;

        /// <summary>
        /// X value: Left or right on the screen. Greater values are further to the right.
        /// Y value: Up or down on the screen. Greater values are further to the top of the screen.
        /// Z value: In or out, depth of the screen. Greater values are closer to you, while negative values extend past the target.
        /// </summary>
        [Tooltip("The camera with sample these points around the target. By default it samples 5 points to make a cross. " +
                 "Sampled points are offsets from the center of the screen to the target. " +
                 "X value: Left or right on the screen. Greater values are further to the right. " +
                 "Y value: Up or down on the screen. Greater values are further to the top of the screen. " +
                 "Z value: In or out, depth of the screen. Greater values are closer to you, while negative values extend past the target.")]
        public List<Vector3> SamplingPoints = new List<Vector3>
                {
                    Vector3.zero,
                    new Vector3(0, 0.5f, 0),
                    new Vector3(0, -0.5f, 0),
                    new Vector3(0.5f, 0, 0),
                    new Vector3(-0.5f, 0, 0),
                };

        /// <summary>
        /// Encapsulates the settings for thickness checking on the ViewCollisionComponent.
        /// </summary>
        [Serializable]
        public class ViewCollisionThicknessChecking
        {
            /// <summary>
            /// When enabled, the camera will perform many raycasts for each sampling point to try to determine if
            /// the objects it is hitting are below a thickness threshhold. If they are below the thickness threshhold,
            /// then they won't be considered collision.
            /// </summary>
            [Tooltip("When enabled, the camera will perform many raycasts for each sampling point to try to determine if " +
                     "the objects it is hitting are below a thickness threshhold. If they are below the thickness threshhold, " +
                     "then they won't be considered collision.")]
            public bool Enabled = false;

            /// <summary>
            /// Any object determined to be less thick than this in units will not be considered collision. Objects of
            /// the exact size will only be noticed if they are perfectly on the original point. It is recommended that
            /// you use a smaller size than the objects you are trying to exclude, so if you want to exclude objects 1 and
            /// greater, use 0.95f, or a similar value.
            /// </summary>
            [Tooltip("Any object determined to be less thick than this in units will not be considered collision. Objects of " +
                     "the exact size will only be noticed if they are perfectly on the original point. It is recommended that " +
                     "you use a smaller size than the objects you are trying to exclude, so if you want to exclude objects 1 and " +
                     "greater, use 0.95f, or a similar value.")]
            public float ThicknessThreshhold = 0.1f;

            /// <summary>
            /// The number of checks performed on each side of a raycast to determine it's accuracy. Each raycast sampling
            /// point that hits will test this number * 2 times to determine thickness. The accuracy of a check is actually
            /// 2^(this value), as the binary search used to determine width increases accuracy exponentially with more checks.
            /// </summary>
            [Tooltip("The number of checks performed on each side of a raycast to determine it's accuracy. Each raycast sampling " +
                     "point that hits will test this number * 2 times to determine thickness. The accuracy of a check is actually " +
                     "2^(this value), as the binary search used to determine width increases accuracy exponentially with more checks.")]
            [Range(1, 32)]
            public int NumberOfChecks = 5;

            /// <summary>
            /// The number of sets of checks. One set of checks would check both left and right of the camera view.
            /// Two sets of checks would check both left and right, and up and down. The more angles, the more
            /// accurately thickness can be determined at different camera angles.
            /// </summary>
            [Tooltip("The number of sets of checks. One set of checks would check both left and right of the camera view. " +
                     "Two sets of checks would check both left and right, and up and down. The more angles, the more " +
                     "accurately thickness can be determined at different camera angles.")]
            [Range(4, 32)]
            public int NumberOfAngles = 4;
        }
        /// <summary>
        /// Allows you to ignore objects that are below a certain level of thickness.
        /// There is an inherit inaccuracy based off the number of checks you perform.
        /// Objects near the targeted width may not have desired results if too few checks are done, or if they are too similar to the targeted width.
        /// </summary>
        [Tooltip("Allows you to ignore objects that are below a certain level of thickness. " +
                 "There is an inherit inaccuracy based off the number of checks you perform. " +
                 "Objects near the targeted width may not have desired results if too few checks are done, or if they are too similar to the targeted width.")]
        public ViewCollisionThicknessChecking ThicknessChecking = new ViewCollisionThicknessChecking();

        #endregion

        public override void Initialize(CameraController cameraController)
        {
            base.Initialize(cameraController);
        }

        /// <summary>
        /// Calculates the maximum distance that the camera can be at from the target when factoring in view collision.
        /// This method assumes the Camera's rotation is set up properly and calculates outwards from the target based on that rotation.
        /// </summary>
        /// <param name="target">The target the camera will calculate the distance to.</param>
        /// <param name="furthestDistance">The maximum distance from the target that the camera can be at.</param>
        /// <returns>The maximum distance from the target when factoring in view collision, if set to true.</returns>
        public float CalculateMaximumDistanceFromTarget(Vector3 target, float furthestDistance)
        {
            return CalculateMaximumDistanceFromTarget(target, furthestDistance, Enabled);
        }

        /// <summary>
        /// Calculates the maximum distance that the camera can be at from the target when factoring in view collision.
        /// This method assumes the Camera's rotation is set up properly and calculates outwards from the target based on that rotation.
        /// This method overrides the settings for Enabled with the provided value for viewCollisionEnabled.
        /// </summary>
        /// <param name="target">The target the camera will calculate the distance to.</param>
        /// <param name="furthestDistance">The maximum distance from the target that the camera can be at.</param>
        /// <param name="viewCollisionEnabled">Whether or not view collision will be considered or not.</param>
        /// <returns>The maximum distance from the target when factoring in view collision, if set to true.</returns>
        public float CalculateMaximumDistanceFromTarget(Vector3 target, float furthestDistance, bool viewCollisionEnabled)
        {
            if (!viewCollisionEnabled)
            {
                return furthestDistance;
            }

            float closestDistance = furthestDistance;

            Vector3 backTowardsCamera = -CameraTransform.Forward;

            foreach (var samplingPoint in SamplingPoints)
            {
                // The target's position is our starting point, then we modify it by how we defined we would based off the sampling point.
                // The tooltip and comment on the sampling points list describes how the Vector3 will be used to determine where it is actually taken.
                Vector3 startingPosition = target;
                startingPosition += CameraTransform.Right * samplingPoint.x;
                startingPosition += CameraTransform.Up * samplingPoint.y;
                startingPosition += CameraTransform.Forward * samplingPoint.z;

                // We're not going to check past where the camera is, so we reduce it by the samplingPoint.z.
                // samplingPoint.z being the depth defined by the tooltip and comment on the sampling points list.
                float distanceToCheck = closestDistance - samplingPoint.z;

                RaycastHit hit;
                if (CollisionViewRaycast(startingPosition, backTowardsCamera, out hit, distanceToCheck, Color.green))
                {
                    if (!ThicknessChecking.Enabled)
                    {
                        // No special thickness checking, so we can just set this as the new closest distance.
                        closestDistance = hit.distance;
                    }
                    else
                    {
                        if (ThicknessCollisionViewRaycast(startingPosition, backTowardsCamera, distanceToCheck))
                        {
                            // we've hit something with the thickness check, so just set it to the original hit distance.
                            closestDistance = hit.distance;
                        }
                        // otherwise we can ignore this hit since it is thinner than the specified parameters in the thickness checking settings.
                    }
                }
            }

            return closestDistance;
        }

        /// <summary>
        /// Returns whether or not the give GameObject should be considered something the camera ray casting should colide with.
        /// </summary>
        /// <param name="collisionObject">The game object that may have view collision.</param>
        /// <returns>The game object has view collision.</returns>
        public bool IsViewCollidable(GameObject collisionObject)
        {
            if (collisionObject == null)
            {
                return false;
            }

            // If something is supposed to be ignored, it will be ignored even if it says to not ignore it in the other list.
            return !IgnoreTheseGameObjects.Contains(collisionObject) && (LineOfSightLayers.value & (1 << collisionObject.layer)) > 0;
        }

        /// <summary>
        /// Performs a series of raycasts until the raycasts either hit an object that is "View Collidable" or the max distance is reached.
        /// </summary>
        /// <param name="startingPosition">Starting point of the raycast</param>
        /// <param name="direction">Direction the raycast goes in</param>
        /// <param name="hit">Hit details if we hit an object we're interested in.</param>
        /// <param name="maxDistance">The maximum distance we'll try to raycast out to.</param>
        /// <param name="debugColor">The color of raycasts for debugging.</param>
        /// <returns>An object that is View Collidable was hit</returns>
        private bool CollisionViewRaycast(Vector3 startingPosition, Vector3 direction, out RaycastHit hit, float maxDistance, Color debugColor)
        {
            float currentDistance = maxDistance;

            while (currentDistance > 0)
            {
                Debug.DrawLine(startingPosition, startingPosition + direction * maxDistance, debugColor);
                if (Physics.Raycast(startingPosition, direction, out hit, maxDistance))
                {
                    if (IsViewCollidable(hit.collider.gameObject))
                    {
                        // We've hit an object that is view collidable within the distance allowed.
                        return true;
                    }
                    else
                    {
                        // We've hit an object, but it isn't view collidable.
                        // Move the starting point to the where the hit occured so we can test past it.
                        // Also lower the currentDistance as we've already checked part of the distance.
                        startingPosition += direction.normalized * (hit.distance + FLOAT_TOLERANCE);
                        currentDistance -= hit.distance + FLOAT_TOLERANCE;
                    }
                }
                else
                {
                    // We've hit no object at all, so obviously we haven't hit something view collidable.
                    return false;
                }
            }

            // This probably won't happen because the "no object was hit" return should take it, 
            // but if it is, then we haven't hit a collidable object and just return false,
            hit = new RaycastHit();
            return false;
        }

        /// <summary>
        /// Performs a thickness check for whether or not a point passes the thickness check as defined by the thickness settings.
        /// The original point should be a point that already has been checked for collision and has collided with something.
        /// This method does not recheck the original position.
        /// </summary>
        /// <param name="startingPosition">The original starting point that triggered the thickness check</param>
        /// <param name="direction">Direction towards the camera</param>
        /// <param name="maxDistance">The original max distance of the camera that was passed</param>
        /// <returns>The thickness check at this point has found the hit object to be acceptable, and thus has no collision.</returns>
        private bool ThicknessCollisionViewRaycast(Vector3 startingPosition, Vector3 direction, float maxDistance)
        {
            // We check on both the left and right side of the starting position.
            // We store what "check" we found a hit on. So if we have 4 checks, we check each side 4 times.
            // If we found something on the 2/4 check and not the 3/4 check, then we know the thickest we're able to estimate is at the 2 position
            // [4, 3, 2, 1, 0, 1, 2, 3, 4]
            //             |--------|
            // Thus we find that in this case, that the object can be hit at 0 left, but not 1 left, and at 2 right, but not 3 right.
            // That makes the object in our estimate "2/4th the size of the acceptable width."
            // If we found that the left was actually at the 3 left marker, then we'd have "4/4th of the acceptable width point", 
            // thus it's too thick and we'll return false.
            //
            // In reality, we do 2^(number of checks) increments, so it would look more like 8/16ths instead of 2/4ths (8 points out of 16 were determined to be collision)

            // Added: NumberOfAngles
            // At first I had approached this only check left and right. Later I quickly realized that if the camera was at a different angle, then
            // the thickness check could fail. NumberOfAngles will be used to increase the angles we check things at.
            // By default it will be set to 4, which means that we'll check [left/right], [up/down], [leftup/downright], and [rightup/leftdown]
            // It isn't recommended it go below that.
            // If any of these checks pass, then it is below the thickness threshhold and thus has no collision.

            int checks = (int) Mathf.Pow(2, ThicknessChecking.NumberOfChecks);

            float angle = 180f / ThicknessChecking.NumberOfAngles;

            var sideDirection = new VirtualTransform();
            sideDirection.LookAt(CameraTransform.Right);
            for (float currentAngle = 0; currentAngle < 180; currentAngle += angle)
            {
                sideDirection.Rotate(CameraTransform.Forward, angle);

                Debug.DrawRay(startingPosition, sideDirection.Forward, Color.cyan);
                Debug.DrawRay(startingPosition, sideDirection.Forward * -1, Color.cyan);

                int right = ThicknessCollisionViewRaycastCheckSide(sideDirection.Forward, startingPosition, direction, maxDistance);
                int left = ThicknessCollisionViewRaycastCheckSide(sideDirection.Forward * -1, startingPosition, direction, maxDistance);

                // If any of the checks passes, then it is thin enough.
                if (right + left < checks)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Performs a binary search of checks on a given side in order to determine how far out we can hit an object from a given point
        /// based on the thickness parameters we were given. The returned value extend up to 100% of the number of increments given.
        /// So if we were told to check 4 increments, and the outed value was 3, we know that we collided with 3/4th the acceptable width.
        /// If we did that check on both sides and got 3 each time, then we know we got 6/4ths the acceptable width and wouldn't pass the acceptable
        /// thickness check. (In reality, we do 2^(number of checks) increments with the bianary search, so it is much more accurate)
        /// 
        /// This method is meant to be used internally for the ThicknessCollisionViewRaycast method.
        /// </summary>
        /// <param name="sideDirection">Which direction we would like to check. This should be transform.right or left.</param>
        /// <param name="startingPosition">The original starting point that triggered the thickness check</param>
        /// <param name="direction">Direction towards the camera</param>
        /// <param name="maxDistance">The original max distance of the camera that was passed</param>
        /// <returns>How many increments in did we find collision.</returns>
        private int ThicknessCollisionViewRaycastCheckSide(Vector3 sideDirection, Vector3 startingPosition, Vector3 direction, float maxDistance)
        {
            // Increased accuracy by checks^2.
            // This will cause us to always use the NumberOfChecks # of raycasts, and allow us to be as accurate as possible doing so.
            int numberOfChecks = (int)Mathf.Pow(2, ThicknessChecking.NumberOfChecks);

            int max = Math.Max(1, numberOfChecks);
            int min = 1;
            int current = Math.Max(1, max / 2);

            int lastCheck = -1;
            int value = 0;

            // Binary search for the width
            while (current != lastCheck && current > 0 && current != value)
            {
                lastCheck = current;
                var percentage = (current / (float)numberOfChecks);

                var adjustedPosition = startingPosition + (sideDirection * ThicknessChecking.ThicknessThreshhold * percentage);

                RaycastHit hit;
                if (CollisionViewRaycast(adjustedPosition, direction, out hit, maxDistance, Color.red))
                {
                    value = current;
                    min = current;
                    current = current + (max - min + 1) / 2;
                }
                else
                {
                    max = current;
                    current = current - (max - min + 1) / 2;
                }
            }

            return value;
        }
    }
}
