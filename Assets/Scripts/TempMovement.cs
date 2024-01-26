using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TempMovement : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator _animator;
    [SerializeField] private Rigidbody2D _rb2D, _leftLegRb2D, _rightLegRb2D;
    [SerializeField] private HingeJoint2D _leftArmHJ2D, _rightArmHJ2D;
    [SerializeField] private SpriteRenderer _face;
    [SerializeField] private Sprite[] _faceSprites;

    [Header("Input & Stats")]
    [Range(1000.0f, 2000.0f)][SerializeField] private float _speed = 1500.0f;
    [Range(50.0f, 250.0f)][SerializeField] private float _jumpForce = 150.0f;
    [Range(1000.0f, 10000.0f)][SerializeField] private float _gravity = 9180.0f;
    [Range(0.0f, 5.0f)][SerializeField] private float _emoteTime = 2.0f;

    private float _xInput = 0;
    private bool _isGrabbing = false;
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
    [Range(6, 10)][SerializeField] private float _groundCheckOffset = 8.5f;
    [SerializeField] private bool _isGrounded;

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        _xInput = Input.GetAxisRaw("Horizontal");

        HandleJumps();
        HandleGrabs();
        HandleEmotes();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleGroundCheck();
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

    private void HandleMovement()
    {
        if (_xInput != 0)
        {
            if (_xInput < 0)
            {
                _face.flipX = true;
                _animator.Play(_walkLeftAnimationName);
                StartCoroutine(MoveLeftSequence(_stepWait));
            }
            else
            {
                _face.flipX = false;
                _animator.Play(_walkRightAnimationName);
                StartCoroutine(MoveRightSequence(_stepWait));
            }
        }
        else
            _animator.Play("anim_idle");
    }

    private void HandleGroundCheck()
    {
        Vector2 groundCheckPos = _rb2D.position;
        groundCheckPos.y -= _groundCheckOffset;

        _isGrounded = Physics2D.OverlapCircle(groundCheckPos, _groundCheckRadius, _groundLayer);
    }
    private void HandleJumps()
    {
        if (_isGrounded && Input.GetKeyDown(KeyCode.Space))
            _rb2D.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
        else if (!_isGrounded)
            _rb2D.AddForce(_gravity * Time.deltaTime * Vector2.down);
    }

    private void LimpHands()
    {
        _leftArmHJ2D.limits = _limpLeftArmLimits;
        _rightArmHJ2D.limits = _limpRightArmLimits;
        _isGrabbing = false;
    }
    private void StraightenArmsLeft()
    {
        _leftArmHJ2D.limits = _straightenLeftArmToLeft;
        _rightArmHJ2D.limits = _straightenRightArmToLeft;
        _isGrabbing = true;
    }
    private void StraightenArmsRight()
    {
        _leftArmHJ2D.limits = _straightenLeftArmToRight;
        _rightArmHJ2D.limits = _straightenRightArmToRight;
        _isGrabbing = true;
    }
    private void HandleGrabs()
    {
        if (Input.GetKeyDown(KeyCode.E))
            StraightenArmsRight();
        else if (Input.GetKeyUp(KeyCode.E))
            LimpHands();

        if (Input.GetKeyDown(KeyCode.Q))
            StraightenArmsLeft();
        else if (Input.GetKeyUp(KeyCode.Q))
            LimpHands();
    }

    private void HandleEmotes()
    {
        if (_isEmoting)
            return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            StartCoroutine(DoFaceEmote(3));
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            StartCoroutine(DoFaceEmote(2));
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            StartCoroutine(DoFaceEmote(1));
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
        _face.sprite = _faceSprites[faceIndex];
        _isEmoting = true;
        yield return new WaitForSeconds(_emoteTime);

        _face.sprite = _faceSprites[0];
        _isEmoting = false;
    }
}
