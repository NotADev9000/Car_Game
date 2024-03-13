using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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
    [SerializeField] private float _rotationSpeed = 60f; // deg/s
    [SerializeField][Tooltip("Distance from center of mass to apply movement force")] private Vector3 _applyForceOffset = Vector3.zero;
    [Space(5)]
    [SerializeField] private float _dragOnGround = 0.5f;
    [SerializeField] private float _dragInAir = 0f;
    [Space(5)]
    [SerializeField] private float _timeBetweenVelocityTracking = 0.1f;
    [SerializeField] private float _maxGroundAngle = 70f;


    [Header("Traction Settings")]
    [SerializeField][Range(0, 1)][Tooltip("0 = no grip, 1 = max grip")] private float _gripFactor = 0.5f;

    // Components
    private Rigidbody _rb;

    // Suspension Vars
    private List<Vector3> _suspensionContactNormals = new List<Vector3>();
    private bool IsGrounded => _suspensionContactNormals.Count > 1;

    // Movement Vars
    private Vector3 _previousVel;
    private Vector3 _projectedDirection = Vector3.zero;
    private Vector3 ApplyMovementForceAt => transform.position + _rb.centerOfMass + _applyForceOffset;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        StartCoroutine(TrackPreviousVelocity());
    }

    private void OnEnable()
    {
        _inputReader.MoveEvent += OnMove;
    }

    private void OnDisable()
    {
        _inputReader.MoveEvent -= OnMove;
    }

    private void OnCollisionEnter(Collision collision)
    {
        ApplyPreviousVelocityIfHitGround(collision);
    }

    private void FixedUpdate()
    {
        UpdateSuspension();
        UpdateDrag();

        if (IsGrounded)
        {
            UpdateProjectedDirection();
            UpdateTraction();
            UpdateSteering();
            UpdateMovement();

            SpeedTest();
        }
    }

    //--------------------
    #region Input

    private void OnMove(Vector2 moveVector)
    {
        _inputVector = moveVector;
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
        _rb.drag = IsGrounded ? _dragOnGround : _dragInAir;
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

    /// <summary>
    /// Applies a sideways force to prevent the vehicle from sliding
    /// </summary>
    private void UpdateTraction()
    {
        float sidewaysVelocity = Vector3.Dot(transform.right, _rb.velocity);
        float desiredSidewaysVelocity = -sidewaysVelocity * _gripFactor;
        Vector3 tractionForce = transform.right * (desiredSidewaysVelocity / Time.fixedDeltaTime);

        _rb.AddForce(tractionForce, ForceMode.Acceleration);
    }

    private void UpdateSteering()
    {
        if (_rb.velocity.magnitude > 0.1f)
        {
            Vector3 steerForce = _inputVector.x * _rotationSpeed * transform.up;
            _rb.AddTorque(steerForce, ForceMode.Acceleration);
        }
    }

    private void UpdateMovement()
    {
        if (_inputVector.y != 0)
        {
            Vector3 force = _inputVector.y * _maxSpeed * _projectedDirection;
            AccelerateTo(force, _acceleration);
        }
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
        if (IsCollisionWithGround(collision) && _rb.velocity.sqrMagnitude < _previousVel.sqrMagnitude)
        {
            ApplyPreviousVelocity();
        }
    }

    private void ApplyPreviousVelocity()
    {
        _rb.velocity = new Vector3(_previousVel.x, _rb.velocity.y, _previousVel.z);
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

    // DEBUG
    bool _timerOn = false;
    float _timer = 0f;
    private void SpeedTest()
    {
        if (_timerOn)
        {
            float t = Time.time;
            if (t - _timer >= 1f)
            {
                Debug.Log(_rb.velocity.magnitude);
                _timerOn = false;
            }
        }
        else
        {
            _timer = Time.time;
        }
    }
#endif

}
