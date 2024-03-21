using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[SelectionBase]
[RequireComponent (typeof (Rigidbody))]
public class HoverVehicleController : MonoBehaviour
{
    [Header("Input Settings")]
    [SerializeField] private InputReader _inputReader;
    private Vector2 _inputVector;

    [Header("Suspension Settings")]
    [SerializeField] private Suspension[] _suspensions;
    [SerializeField] private float _suspensionRestLength = 0.5f;
    [SerializeField] private float _springStrength = 15f;
    [SerializeField] private float _springDamper = 2f;

    [Header("Movement Settings")]
    [SerializeField] private float _maxSpeed = 25f; // m/s
    [SerializeField] private float _acceleration = 15f;  // m/s^2
    [SerializeField][Tooltip("Distance from center of mass to apply movement force")] private Vector3 _applyForceOffset = Vector3.zero;
    [Space(5)]
    [SerializeField] private float _dragOnGround = 0.5f;
    [SerializeField] private float _dragWhenMoving = 0f;
    [SerializeField] private float _dragInAir = 0f;
    [Space(5)]
    [SerializeField] private float _timeBetweenVelocityTracking = 0.1f;
    [SerializeField] private float _maxGroundAngle = 70f;

    [Header("Steering Settings")]
    [SerializeField] private float _normalMaxRotationSpeed = 7f;
    [SerializeField] private float _driftMaxRotationSpeed = 8f;
    [SerializeField] private float _inAirRotationSpeed = 1f;
    [Space(5)]
    [SerializeField][Tooltip("speed needed before max rotation is applied")] private float _minSpeedForMaxTorque = 5f;
    [SerializeField][Tooltip("speed needed before any rotation is applied")] private float _minSpeedForAnyTorque = 0.1f;
    [Space(5)]
    [SerializeField][Tooltip("% of max torque (y) to apply when velocity (x) is lower than a threshold")] private AnimationCurve _torqueFactorCurve;

    [Header("Traction Settings")]
    [SerializeField][Range(0, 1)][Tooltip("0 = no grip, 1 = max grip")] private float _normalGripFactor = 0.5f;
    [SerializeField][Range(0, 1)][Tooltip("0 = no grip, 1 = max grip")] private float _driftGripFactor = 0.1f;
    [SerializeField][Tooltip("length of lerp from normal grip to drifting grip")] private float _gripLerpToDriftTime = 0.2f;
    [SerializeField][Tooltip("length of lerp from drift grip to normal grip")] private float _gripLerpFromDriftTime = 1f;

    // Components
    private Rigidbody _rb;

    // Input
    private bool IsApplyingMovementInput => _inputVector.y != 0;
    private bool IsApplyingSteeringInput => _inputVector.x != 0;

    // Suspension
    private List<Vector3> _suspensionContactNormals = new List<Vector3>();
    private bool IsGrounded => _suspensionContactNormals.Count > 1;

    // Movement
    private Vector3 _previousVel;
    private Vector3 _projectedDirection = Vector3.zero;
    private Vector3 ApplyMovementForceAt => transform.position + _rb.centerOfMass + _applyForceOffset;

    // Steering
    private float _currentMaxRotationSpeed;

    // Traction
    private float _currentGripFactor;
    private float _targetGripFactor;
    private float _lerpStartGripFactor = 0f;
    private float _gripLerpTotalTime;
    private float _gripLerpTimer = 0f;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        InitSettings();
        StartCoroutine(TrackPreviousVelocity());
    }

    private void OnEnable()
    {
        _inputReader.MoveEvent += OnMove;
        _inputReader.FireEvent += OnDriftCheck;
    }

    private void OnDisable()
    {
        _inputReader.MoveEvent -= OnMove;
        _inputReader.FireEvent -= OnDriftCheck;
    }

    private void OnCollisionEnter(Collision collision)
    {
        ApplyPreviousVelocityIfHitGround(collision);
    }

    private void Update()
    {
        UpdateGripFactor();
    }

    private void FixedUpdate()
    {
        UpdateSuspension();
        UpdateDrag();
        UpdateSteering();

        if (IsGrounded)
        {
            UpdateProjectedDirection();
            UpdateTraction();
            UpdateMovement();
        }
    }

    //--------------------
    #region Input

    private void OnMove(Vector2 moveVector)
    {
        _inputVector = moveVector;
    }

    private void OnDriftCheck(bool isPressed)
    {
        UpdateDriftValues(isPressed);
    }

    #endregion
    //--------------------

    //--------------------
    #region Initialize Settings

    private void InitSettings()
    {
        _currentMaxRotationSpeed = _normalMaxRotationSpeed;
        _currentGripFactor = _targetGripFactor = _normalGripFactor;
        _gripLerpTotalTime = _gripLerpToDriftTime;
    }

    #endregion
    //--------------------

    //--------------------
    #region Vehicle Physics

    private void UpdateSuspension()
    {
        _suspensionContactNormals.Clear();

        for (int i = 0; i < _suspensions.Length; i++)
        {
            Suspension suspension = _suspensions[i];
            suspension.CastSpring(_suspensionRestLength);

            if (suspension.IsGrounded)
            {
                Vector3 suspensionWorldVelocity = _rb.GetPointVelocity(suspension.Transform.position);
                float springForce = suspension.CalculateSpringForces(_suspensionRestLength, _springStrength, suspensionWorldVelocity, _springDamper);

                _rb.AddForceAtPosition(springForce * suspension.Transform.up, suspension.Transform.position, ForceMode.Acceleration);

                _suspensionContactNormals.Add(suspension.HitNormal);
            }
        }
    }

    private void UpdateDrag()
    {
        float drag;
        if (!IsGrounded)
        {
            drag = _dragInAir;
        }
        else
        {
            drag = IsApplyingMovementInput ? _dragWhenMoving : _dragOnGround;
        }

        _rb.drag = drag;
    }

    /// <summary>
    /// Projects vehicle forward direction onto ground below
    /// This is the average of each suspension contact point
    /// </summary>
    private void UpdateProjectedDirection()
    {
        Vector3 suspensionContactAverage = Vector3.zero;
        int suspensionContactAmount = _suspensionContactNormals.Count;

        for (int i = 0; i < suspensionContactAmount; i++)
        {
            suspensionContactAverage += _suspensionContactNormals[i];
        }
        if (suspensionContactAmount > 0)
            suspensionContactAverage = (suspensionContactAverage / suspensionContactAmount).normalized;

        _projectedDirection = Vector3.ProjectOnPlane(transform.forward, suspensionContactAverage);
    }

    private void UpdateGripFactor()
    {
        if (_currentGripFactor != _targetGripFactor)
        {
            //Debug.Log("Lerping Grip!: " + _gripLerpTimer + " / " + _gripLerpTotalTime);
            //Debug.Log("GFactor: " + _currentGripFactor);
            _currentGripFactor = Mathf.Lerp(_lerpStartGripFactor, _targetGripFactor, _gripLerpTimer / _gripLerpTotalTime);
            _gripLerpTimer += Time.deltaTime;
        }
        else
        {
            _gripLerpTimer = 0f;
        }
    }

    /// <summary>
    /// Applies a sideways force to prevent the vehicle from sliding
    /// </summary>
    private void UpdateTraction()
    {
        float sidewaysVelocity = Vector3.Dot(transform.right, _rb.velocity);
        float desiredSidewaysVelocity = -sidewaysVelocity * _currentGripFactor;
        Vector3 tractionForce = transform.right * (desiredSidewaysVelocity / Time.fixedDeltaTime);

        _rb.AddForce(tractionForce, ForceMode.Acceleration);
    }

    private void UpdateSteering()
    {
        if (IsApplyingSteeringInput && _rb.velocity.magnitude >= _minSpeedForAnyTorque)
        {
            float torqueSpeed;
            if (IsGrounded)
            {
                float speedToTorqueRatio = _rb.velocity.magnitude / _minSpeedForMaxTorque;
                float torquePercentageToApply = _torqueFactorCurve.Evaluate(speedToTorqueRatio);
                torqueSpeed = _currentMaxRotationSpeed * torquePercentageToApply;
            }
            else
            {
                torqueSpeed = _inAirRotationSpeed;
            }

            Vector3 steerForce = _inputVector.x * torqueSpeed * transform.up;
            _rb.AddTorque(steerForce, ForceMode.Acceleration);
        }
    }

    private void UpdateMovement()
    {
        if (IsApplyingMovementInput)
        {
            Vector3 force = _inputVector.y * _maxSpeed * _projectedDirection;
            AccelerateTo(force, _acceleration);
        }
    }

    private void UpdateDriftValues(bool isDrifting)
    {
        _currentMaxRotationSpeed = isDrifting ? _driftMaxRotationSpeed : _normalMaxRotationSpeed;

        _targetGripFactor = isDrifting ? _driftGripFactor : _normalGripFactor;
        _lerpStartGripFactor = _currentGripFactor;
        _gripLerpTotalTime = isDrifting ? _gripLerpToDriftTime : _gripLerpFromDriftTime;
        _gripLerpTimer = 0f;
    }

    #endregion
    //--------------------

    //--------------------
    #region Velocity Tracking

    private IEnumerator TrackPreviousVelocity()
    {
        while (true)
        {
            _previousVel = _rb.velocity;
            yield return new WaitForSeconds(_timeBetweenVelocityTracking);
        }
    }

    private void ApplyPreviousVelocityIfHitGround(Collision collision)
    {
        if (IsCollisionWithGround(collision) && IsPreviousVelocityFasterThanCurrent())
        {
            ApplyPreviousVelocity();
        }
    }

    private void ApplyPreviousVelocity()
    {
        _rb.velocity = new Vector3(_previousVel.x, _rb.velocity.y, _previousVel.z);
    }

    private bool IsPreviousVelocityFasterThanCurrent()
    {
        return _rb.velocity.sqrMagnitude < _previousVel.sqrMagnitude;
    }

    private bool IsCollisionWithGround(Collision collision)
    {
        Vector3 hitNormal = collision.GetContact(0).normal;
        if (Vector3.Angle(hitNormal, transform.up) <= _maxGroundAngle)
        {
           return true;
        }

        return false;
    }

    #endregion
    //--------------------

    //--------------------
    #region Movement Methods

    private void AccelerateTo(Vector3 targetVelocity, float maxAccel)
    {
        Vector3 deltaV = targetVelocity - _rb.velocity;
        Vector3 accel = deltaV / Time.deltaTime;

        if (accel.sqrMagnitude > maxAccel * maxAccel)
            accel = accel.normalized * maxAccel;

        _rb.AddForceAtPosition(accel, ApplyMovementForceAt, ForceMode.Acceleration);
    }

    #endregion
    //--------------------

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // movement direction
        Debug.DrawRay(transform.position, _projectedDirection * 2f, Color.yellow);

        // springs
        for (int i = 0; i < _suspensions.Length; i++)
        {
            Suspension s = _suspensions[i];
            Color c = s.IsGrounded ? Color.green : Color.red;
            Debug.DrawRay(s.Transform.position, -s.Transform.up * _suspensionRestLength, c);
        }

        if (_rb)
        {
            Handles.Label(transform.position, Math.Round(_rb.velocity.magnitude, 2).ToString());
        }
    }
#endif

}
