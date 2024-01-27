using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator _animator;
    [SerializeField] private HingeJoint2D _leftArmHJ2D, _rightArmHJ2D;

    [SerializeField] private Rigidbody2D _rb2D, _leftLegRb2D, _rightLegRb2D;
    public Rigidbody2D Rb2D => _rb2D;

    [SerializeField] private SpriteRenderer _faceRenderer;
    public SpriteRenderer FaceRenderer => _faceRenderer;

    [SerializeField] private SpriteRenderer[] _allRenderersButFace;
    public SpriteRenderer[] AllRenderersButFace => _allRenderersButFace;

    [SerializeField] private Sprite[] _faceSprites;
    [SerializeField] private Grab _leftGrab, _rightGrab;


    [Header("Input & Stats")]
    [SerializeField] private PlayerInput _inputMap;
    [Range(1000.0f, 2000.0f)][SerializeField] private float _speed = 1500.0f;
    [Range(50.0f, 250.0f)][SerializeField] private float _jumpForce = 150.0f;
    [Range(1000.0f, 10000.0f)][SerializeField] private float _gravity = 9180.0f;
    [Range(0.0f, 5.0f)][SerializeField] private float _emoteTime = 2.0f;

    private PlayerControls _controls;
    public PlayerControls Controls => _controls;

    private Vector2 _moveInput = Vector2.zero;
    private Vector2 _dPadInput = Vector2.zero;
    private bool _isGrabbing = false;
    private bool _isSwinging = false, _isRightSwing = false, _isLeftSwing = false;
    private bool _isEmoting = false;

    [Header("Animation")]
    [SerializeField] private float _stepWait = 0.5f;
    [SerializeField] private JointAngleLimits2D _limpRightArmLimits, _limpLeftArmLimits;
    [SerializeField] private JointAngleLimits2D _straightenRightArmToRight = new(), _straightenRightArmToLeft = new();
    [SerializeField] private JointAngleLimits2D _straightenLeftArmToRight = new(), _straightenLeftArmToLeft = new();
    [SerializeField] private string _walkLeftAnimationName = "anim_walk_left", _walkRightAnimationName = "anim_walk_right";

    [Header("GroundCheck")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _groundCheckRadius;
    [Range(6, 10)][SerializeField] private float _groundCheckOffset = 9.3f;
    [SerializeField] private bool _isGrounded;

    private void Awake()
    {
        _controls = new PlayerControls();
        _inputMap.onActionTriggered += OnActionTriggered;
    }
    private void Start()
    {
        Init();
    }
    private void FixedUpdate()
    {
        HandleGroundCheck();

        if (!_isGrounded)
            _rb2D.AddForce(_gravity * Time.deltaTime * Vector2.down);

        if (_isSwinging)
        {
            if (_isLeftSwing)
                SwingLeft();
            else if (_isRightSwing)
                SwingRight();
        }
    }
    private void OnDestroy()
    {
        _inputMap.onActionTriggered -= OnActionTriggered;
    }

    private void OnMove(InputAction.CallbackContext input)
    {
        _moveInput = input.ReadValue<Vector2>();

        if (!_isGrabbing && _moveInput.x != 0)
        {
            //_currentSpeed = _speed;
            _isSwinging = false;
            _isLeftSwing = false;
            _isRightSwing = false;

            if (_moveInput.x < 0)
            {
                _faceRenderer.flipX = true;
                _animator.Play(_walkLeftAnimationName);
                StartCoroutine(MoveLeftSequence(_stepWait));
            }
            else
            {
                _faceRenderer.flipX = false;
                _animator.Play(_walkRightAnimationName);
                StartCoroutine(MoveRightSequence(_stepWait));
            }
        }
        else if (_leftGrab.IsHolding || _rightGrab.IsHolding)
        {
            _isSwinging = true;
            _animator.Play("anim_idle");

            if (_moveInput.x < 0)
            {
                _isRightSwing = false;
                _faceRenderer.flipX = true;
                _isLeftSwing = true;
            }
            else
            {
                _isLeftSwing = false;
                _faceRenderer.flipX = false;
                _isRightSwing = true;
            }
        }
        else
            _animator.Play("anim_idle");
    }
    private void OnJump(InputAction.CallbackContext input)
    {
        if (_isGrounded && input.action.WasPressedThisFrame())
            _rb2D.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
    }
    private void OnLGrab(InputAction.CallbackContext input)
    {
        if (input.action.WasPressedThisFrame())
            StraightenArmsLeft();
        else if (input.action.WasReleasedThisFrame())
            LimpHands();
    }
    private void OnRGrab(InputAction.CallbackContext input)
    {
        if (input.action.WasPressedThisFrame())
            StraightenArmsRight();
        else if (input.action.WasReleasedThisFrame())
            LimpHands();
    }
    private void OnEmote1(InputAction.CallbackContext input)
    { 
        if (input.action.WasPressedThisFrame())
            StartCoroutine(DoFaceEmote(3));
    }
    private void OnEmote2(InputAction.CallbackContext input)
    {
        if (input.action.WasPressedThisFrame())
            StartCoroutine(DoFaceEmote(2));
    }
    private void OnEmote3(InputAction.CallbackContext input)
    {
        if (input.action.WasPressedThisFrame())
            StartCoroutine(DoFaceEmote(1));
    }

    private void OnActionTriggered(InputAction.CallbackContext input)
    {
        if (input.action.name == _controls.Player.Move.name)
            OnMove(input);

        if (input.action.name == _controls.Player.Jump.name)
            OnJump(input);

        if (input.action.name == _controls.Player.LGrab.name)
            OnLGrab(input);
        else if (input.action.name == _controls.Player.RGrab.name)
            OnRGrab(input);

        if (_isEmoting)
            return;

        if (input.action.name == _controls.Player.Emote1.name)
            OnEmote1(input);
        else if (input.action.name == _controls.Player.Emote2.name)
            OnEmote2(input);
        else if (input.action.name == _controls.Player.Emote3.name)
            OnEmote3(input);
    }

    private void Init()
    {
        _limpLeftArmLimits = _leftArmHJ2D.limits;
        _limpRightArmLimits = _rightArmHJ2D.limits;

        _straightenLeftArmToRight.min = _straightenLeftArmToRight.max = -180.0f;
        _straightenLeftArmToLeft.min = _straightenLeftArmToLeft.max = 0.0f;

        _straightenRightArmToRight.min = _straightenRightArmToRight.max = 0.0f;
        _straightenRightArmToLeft.min = _straightenRightArmToLeft.max = 180.0f;
    }

    private void HandleGroundCheck()
    {
        Vector2 groundCheckPos = _rb2D.position;
        groundCheckPos.y -= _groundCheckOffset;

        if (!_isSwinging)
            _isGrounded = Physics2D.OverlapCircle(groundCheckPos, _groundCheckRadius, _groundLayer);
        else
            _isGrounded = true;
    }

    private void LimpHands()
    {
        _leftArmHJ2D.limits = _limpLeftArmLimits;
        _rightArmHJ2D.limits = _limpRightArmLimits;
        _isGrabbing = false;

        _leftGrab.IsHolding = false;
        _rightGrab.IsHolding = false;

        Destroy(_leftGrab.TempFJ2D);
        Destroy(_rightGrab.TempFJ2D);
    }
    private void StraightenArmsLeft()
    {
        _leftArmHJ2D.limits = _straightenLeftArmToLeft;
        _rightArmHJ2D.limits = _straightenRightArmToLeft;
        _isGrabbing = true;

        _leftGrab.IsHolding = true;
        _rightGrab.IsHolding = true;
    }
    private void StraightenArmsRight()
    {
        _leftArmHJ2D.limits = _straightenLeftArmToRight;
        _rightArmHJ2D.limits = _straightenRightArmToRight;
        _isGrabbing = true;

        _leftGrab.IsHolding = true;
        _rightGrab.IsHolding = true;
    }
    private void HandleGrabs()
    {
        if (_isGrabbing)
        {
            _leftGrab.IsHolding = true;
            _rightGrab.IsHolding = true;
        }
        else
        {
            _leftGrab.IsHolding = false;
            _rightGrab.IsHolding = false;

            Destroy(_leftGrab.TempFJ2D);
            Destroy(_rightGrab.TempFJ2D);
        }
    }

    private void SwingRight()
    {
        _rightLegRb2D.AddForce(_speed * Time.fixedDeltaTime * Vector2.right);
        _leftLegRb2D.AddForce(_speed * Time.fixedDeltaTime * Vector2.right);
    }
    private void SwingLeft()
    {
        _rightLegRb2D.AddForce(_speed * Time.fixedDeltaTime * Vector2.left);
        _leftLegRb2D.AddForce(_speed * Time.fixedDeltaTime * Vector2.left);
    }

    private IEnumerator MoveLeftSequence(float sec)
    {
        _rightLegRb2D.AddForce(_speed * Time.fixedDeltaTime * Vector2.left);
        yield return new WaitForSeconds(sec);

        _leftLegRb2D.AddForce(_speed * Time.fixedDeltaTime * Vector2.left);
    }
    private IEnumerator MoveRightSequence(float sec)
    {
        _leftLegRb2D.AddForce(_speed * Time.fixedDeltaTime * Vector2.right);
        yield return new WaitForSeconds(sec);

        _rightLegRb2D.AddForce(_speed * Time.fixedDeltaTime * Vector2.right);
    }

    private IEnumerator DoFaceEmote(int faceIndex)
    {
        _faceRenderer.sprite = _faceSprites[faceIndex];
        _isEmoting = true;
        yield return new WaitForSeconds(_emoteTime);

        _faceRenderer.sprite = _faceSprites[0];
        _isEmoting = false;
    }
}
