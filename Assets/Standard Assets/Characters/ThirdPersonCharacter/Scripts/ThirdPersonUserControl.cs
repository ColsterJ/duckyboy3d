// MODIFIED by Colin Jones - 

using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Characters.ThirdPerson;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof (ThirdPersonCharacter))]
    public class ThirdPersonUserControl : MonoBehaviour
    {
        private ThirdPersonCharacter m_Character; // A reference to the ThirdPersonCharacter on the object
        private Transform m_Cam;                  // A reference to the main camera in the scenes transform
        private Vector3 m_CamForward;             // The current forward direction of the camera
        private Vector3 m_Move;
        private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.

        // Input for movement
        private float _horizontal, _veritcal;
        public float InputSensitivity = 0.5f;
        // Between left or right, which was the last input used first? (left=true, right=false)
        private bool _leftLastInput;
        // Between up or down, which was the last input used first? (up=true, down=false)
        private bool _upLastInput;
        private bool player_made_move = false;
        
        private void Start()
        {
            // get the transform of the main camera
            if (Camera.main != null)
            {
                m_Cam = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning(
                    "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.");
                // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
            }

            // get the third person character ( this should never be null due to require component )
            m_Character = GetComponent<ThirdPersonCharacter>();
        }


        private void Update()
        {
            ProcessInput();

            if (!m_Jump)
            {
                m_Jump = CrossPlatformInputManager.GetButtonDown("ActionA");
            }
        }


        // Fixed update is called in sync with physics
        private void FixedUpdate()
        {
            bool crouch = Input.GetKey(KeyCode.C);

            // calculate move direction to pass to character
            if (m_Cam != null)
            {
                // calculate camera relative direction to move:
                m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
                m_Move = _veritcal * m_CamForward + _horizontal * m_Cam.right;
            }
            else
            {
                // we use world-relative directions in the case of no main camera
                m_Move = _veritcal * Vector3.forward + _horizontal*Vector3.right;
            }
#if !MOBILE_INPUT
			// walk speed multiplier
	        if (Input.GetKey(KeyCode.LeftShift)) m_Move *= 0.5f;
#endif

            // pass all parameters to the character control script
            if (player_made_move)     // This ensures movement vector isn't passed to character controller unless the player intends to be moving
                                      // (otherwise, when at a stand-still but moving the camera, player character will still turn to reorient himself relative
                                      // to the camera forward position, which is how it works in for example Jedi Outcast, Indiana Jones Emperor's Tomb IIRC
                                      // but is not used in newer games like Sleeping Dogs or even Zelda Wind Waker iirc
                m_Character.Move(m_Move, crouch, m_Jump);
            else
                m_Character.Move(Vector3.zero, crouch, m_Jump);
            m_Jump = false;
        }
        
        void ProcessInput()
        {
            // read inputs
            bool leftDown = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) || (Input.GetAxis("Horizontal")<0);
            bool rightDown = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) || (Input.GetAxis("Horizontal")>0);
            bool bothHorizontalDown = leftDown && rightDown;
            bool upDown = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || (Input.GetAxis("Vertical") > 0);
            bool downDown = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) || (Input.GetAxis("Vertical") < 0);
            bool bothVerticalDown = upDown && downDown;

            if (leftDown && !rightDown)
            {
                _leftLastInput = true;
            }
            else if (!leftDown && rightDown)
            {
                _leftLastInput = false;
            }
            if (upDown && !downDown)
            {
                _upLastInput = true;
            }
            else if (!upDown && downDown)
            {
                _upLastInput = false;
            }

            leftDown = bothHorizontalDown ? _leftLastInput : leftDown;
            rightDown = bothHorizontalDown ? !_leftLastInput : rightDown;
            upDown = bothVerticalDown ? _upLastInput : upDown;
            downDown = bothVerticalDown ? !_upLastInput : downDown;

            float horizontalTarget = leftDown ? -1 : 0;
            horizontalTarget = rightDown ? 1 : horizontalTarget;
            float verticalTarget = upDown ? 1 : 0;
            verticalTarget = downDown ? -1 : verticalTarget;

            _horizontal = Mathf.Lerp(_horizontal, horizontalTarget, InputSensitivity);
            _veritcal = Mathf.Lerp(_veritcal, verticalTarget, InputSensitivity);

            player_made_move = (leftDown || rightDown || upDown || downDown) ? true : false;
        }
    }
}
