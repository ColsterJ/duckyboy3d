using System;
using UnityEngine;

namespace AdvancedUtilities.Cameras.Components
{
    /// <summary>
    /// A component that can take control of the cursor's locking and visiblity states based off settings.
    /// </summary>
    [Serializable]
    public class CursorComponent : CameraComponent
    {
        #region Public Fields

        /// <summary>
        /// Sets whether the controller should control locking the mouse or not.
        /// </summary>
        [Tooltip("Sets whether the controller should control locking the mouse or not.")]
        public bool Enabled = true;

        /// <summary>
        /// The type of lock that will be engaged when the cursor is locked.
        /// </summary>
        [Tooltip("The type of lock that will be engaged when the cursor is locked.")]
        public CursorLockMode LockMode = CursorLockMode.Locked;

        /// <summary>
        /// The button input for LockCursorButton.
        /// </summary>
        [Tooltip("The button input for LockCursorButton.")]
        public string LockButtonInputName = "Fire1";

        /// <summary>
        /// The mouse will lock and unlock automatically when the provided LockButtonInputName is being pressed and released.
        /// </summary>
        [Tooltip("The mouse will lock and unlock automatically when the provided LockButtonInputName is being pressed and released.")]
        public bool LockCursorButton = true;

        /// <summary>
        /// The button input for HoldToReleaseLock.
        /// </summary>
        [Tooltip("The button input for HoldToReleaseLock.")]
        public string HoldToReleaseInputName = "Fire2";

        /// <summary>
        /// If true, the HoldToReleaseInputName will unlock the cursor when held and will be relocked when released.
        /// </summary>
        [Tooltip("If true, the HoldToReleaseInputName will unlock the cursor when held and will be relocked when released.")]
        public bool HoldToReleaseLock = true;

        /// <summary>
        /// Whether or not the cursor will be visible when locked.
        /// </summary>
        [Tooltip("Whether or not the cursor will be visible when locked.")]
        public bool VisibleWhenLocked = false;

        /// <summary>
        /// Whether or not the cursor will be visible when unlocked.
        /// </summary>
        [Tooltip("Whether or not the cursor will be visible when unlocked.")]
        public bool VisibleWhenUnlocked = true;

        /// <summary>
        /// Locking will disabled when the pointer meets the conditions for the PointerOverGui object.
        /// </summary>
        [Tooltip("Locking will disabled when the pointer meets the conditions for the PointerOverGui object.")]
        public PointerOverGui DisableLockingWhenOver = new PointerOverGui();

        #endregion

        #region Private Fields & Properties

        /// <summary>
        /// Whether or not the cursor should be currently locked.
        /// </summary>
        private bool _cursorLocked = false;

        #endregion

        public override void Initialize(CameraController cameraController)
        {
            base.Initialize(cameraController);
        }

        /// <summary>
        /// Sets the cursor lock and visiblity based off the current input and settings.
        /// </summary>
        public void SetCursorLock()
        {
            if (Enabled == false)
            {
                return;
            }

            if (LockCursorButton)
            {
                _cursorLocked = Input.GetButton(LockButtonInputName) && !DisableLockingWhenOver.IsPointerOverGui();
            }

            if (HoldToReleaseLock)
            {
                // When we hold to release, if we are holding, then we release, otherwise, we lock it
                if (Input.GetButton(HoldToReleaseInputName))
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = VisibleWhenUnlocked;
                }
                else if (_cursorLocked)
                {
                    Cursor.lockState = LockMode;
                    Cursor.visible = VisibleWhenLocked;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = VisibleWhenUnlocked;
                }
            }
            else
            {
                // If we aren't holding to release, then we just lock it when it's supposed to be locked and unlock it when it's not.
                if (_cursorLocked)
                {
                    Cursor.lockState = LockMode;
                    Cursor.visible = VisibleWhenLocked;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = VisibleWhenUnlocked;
                }
            }
        }
    }
}