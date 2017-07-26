using UnityEngine;
using System.Collections;

/// <summary>
/// NOTE: THIS IS ONLY FOR EXAMPLE PURPOSES.
/// THIS CHARACTER CONTROLLER IS NOT PART OF THE PACKAGE.
/// I WILL NOT BE SUPPORTING IT.
/// 
/// That said, you're free to use it, or use it as for design.
/// I created this because the default Standard Assets Third Person Controller sucks and
/// it actually made demonstrating the follow camera controller difficult, mainly because
/// it moves upwards of 0.2f units when rotating in place, which wouldn't demonstrate the features
/// as well as a more solid camera could. That being said, it could demonstrate it with the settings
/// for threshold set higher, but I felt that still wouldn't show it off that well.
/// </summary>
public class TESTSCRIPT_ThirdPersonCharacterController : MonoBehaviour
{
    public float AdditionalGravity = 10f;

    public Rigidbody Rigidbody;
    public Animator CharacterAnimator;

    public float MovementSpeed = 3f;

    public float RotationSpeed = 90f;

    public float JumpPower = 5f;

    // Higher values mean faster adjustment between input.
    public float InputSensitivity = 0.5f;
    // Input for movement
    private float _horizontal, _veritcal;
    // Between left or right, which was the last input used first? (left=true, right=false)
    private bool _leftLastInput;
    // Between up or down, which was the last input used first? (up=true, down=false)
    private bool _upLastInput;
    // For jumping input
    private bool _jumpThisUpdate = false;

    private bool _isGrounded = true;
    private float _groundCheckDistance = 0.25f;
    private Vector3 _previousUpdateMovement = Vector3.zero;

    private float _dampenRotationAnimation = 0.25f;

    private void Start()
    {

    }

    void Update()
    {
        ProcessInput();
    }

    void FixedUpdate ()
    {
        CheckGroundStatus();

        // Move forward
        if (_veritcal < 0)
        {
            _veritcal = 0;
            _horizontal = 3f;
        }

        float movementAmount = MovementSpeed * _veritcal;
        Vector3 movement = this.transform.forward * movementAmount;

        if (!_isGrounded)
        {
            CharacterAnimator.SetBool("OnGround", false);
            movement = _previousUpdateMovement;
        }
        else
        {
            CharacterAnimator.SetBool("OnGround", true);
            _previousUpdateMovement = movement;
        }

        CharacterAnimator.SetFloat("Forward", MovementSpeed*_veritcal, 0.1f, Time.deltaTime);

        Rigidbody.velocity = movement;

        // Jump
        if (_jumpThisUpdate)
        {
            _jumpThisUpdate = false;
            if (_isGrounded)
            {
                Rigidbody.velocity += this.transform.up * JumpPower;
            }
        }

        Rigidbody.velocity += Vector3.down * AdditionalGravity;

        // Rotation (in place)
        float rotationAmount = _horizontal * RotationSpeed * Time.deltaTime;
        float rotationPercentage = _horizontal * RotationSpeed / 360;
        CharacterAnimator.SetFloat("Turn", _horizontal * _dampenRotationAnimation, 0.1f, Time.deltaTime);
        this.transform.Rotate(this.transform.up, rotationAmount);
    }

    void ProcessInput()
    {
        _jumpThisUpdate = Input.GetKey(KeyCode.Space);

        // read inputs
        bool leftDown = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        bool rightDown = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
        bool bothHorizontalDown = leftDown && rightDown;
        bool upDown = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        bool downDown = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
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
    }
    
    void CheckGroundStatus()
    {
        Vector3 startPosition = transform.position + (this.transform.up * 0.1f);
        Vector3 ray = -this.transform.up * _groundCheckDistance;

        RaycastHit hitInfo;
        // 0.1f is a small offset to start the ray from inside the character
        // it is also good to note that the transform position in the sample assets is at the base of the character
        if (Physics.Raycast(startPosition, ray, out hitInfo, _groundCheckDistance))
        {
            _isGrounded = true;
            CharacterAnimator.applyRootMotion = true;
        }
        else
        {
            _isGrounded = false;
            CharacterAnimator.applyRootMotion = false;
        }
    }
}
