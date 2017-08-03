using UnityEngine;

namespace AdvancedUtilities.Cameras.Components
{
    /// <summary>
    /// The mode that the follow target component uses.
    /// </summary>
    public enum FollowRotationMode
    {
        /// <summary>
        /// When the target transform rotates any amount, this will trigger the FollowRotationComponent to rotate
        /// back to it's nuetral orientation in relation to the target's rotation.
        /// </summary>
        [Tooltip("When the target transform rotates any amount, this will trigger the FollowRotationComponent to rotate " +
                 "back to it's nuetral orientation in relation to the target's rotation.")]
        AdjustCompletelyOnRotation,

        /// <summary>
        /// When the target transform moves any amount, this will trigger the FollowRotationComponent to rotate
        /// back to it's nuetral orientation in relation to the target's rotation.
        /// </summary>
        [Tooltip("When the target transform moves any amount, this will trigger the FollowRotationComponent to rotate " +
                 "back to it's nuetral orientation in relation to the target's rotation.")]
        AdjustCompletelyOnMovement,

        /// <summary>
        /// While the target is moving, the FollowRotationComponent will rotate to it's nuetral position, but only
        /// while the target continues to move. If the target stops moving, it will stop rotating back to nuetral.
        /// </summary>
        [Tooltip("While the target is moving, the FollowRotationComponent will rotate to it's nuetral position, but only " +
                 "while the target continues to move. If the target stops moving, it will stop rotating back to nuetral.")]
        AdjustWhileMoving
    }
}
