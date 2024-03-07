using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[SelectionBase]
[RequireComponent (typeof (Rigidbody))]
public class HoverVehicleController : MonoBehaviour
{
    [SerializeField] private InputReader _inputReader;
    private Vector2 _inputVector;

    [Header("Suspension Settings")]
    [SerializeField] private Transform[] _springs = Array.Empty<Transform>();
    [SerializeField] private float _suspensionRestLength = 0.5f;
    [SerializeField] private float _suspensionStrength = 15f;
    [SerializeField] private float _suspensionDamper = 2f;

    [Header("Movement Settings")]
    [SerializeField] private float _maxSpeed = 25f; // m/s
    [SerializeField] private float _acceleration = 15f;  // m/s^2
    [SerializeField] private float _rotationSpeed = 60f; // deg/s
    [SerializeField][Range(0, 1)][Tooltip("0 = no grip, 1 = max grip")] private float _gripFactor = 0.5f;

    [Header("Force Settings")]
    [SerializeField][Tooltip("Distance from center of mass to apply movement force")] private Vector3 _applyForceOffset = Vector3.zero;

    private Rigidbody _rb;

    private List<Vector3> _suspensionContactNormals = new List<Vector3>();
    private Vector3 _projectedDirection = Vector3.zero;

    private Vector3 ApplyMovementForceAt => transform.position + _rb.centerOfMass + _applyForceOffset;
    private bool IsGrounded => _suspensionContactNormals.Count > 1;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        _inputReader.MoveEvent += OnMove;
    }

    private void OnDisable()
    {
        _inputReader.MoveEvent -= OnMove;
    }

    private void OnMove(Vector2 moveVector)
    {
        _inputVector = moveVector;
    }

    private void FixedUpdate()
    {
        UpdateSuspension();

        if (IsGrounded)
        {
            UpdateProjectedDirection();

            UpdateTraction();

            UpdateSteering();

            UpdateMovement();
        }
    }

    private void UpdateSuspension()
    {
        _suspensionContactNormals.Clear();

        for (int i = 0; i < _springs.Length; i++)
        {
            Transform spring = _springs[i];
            if (Physics.Raycast(spring.position, -spring.up, out RaycastHit hit, _suspensionRestLength))
            {
                float compressionRatio = 1 - (hit.distance / _suspensionRestLength);
                float springOffsetForce = compressionRatio * _suspensionStrength;

                Vector3 springWorldVelocity = _rb.GetPointVelocity(spring.position);
                float springUpVelocity = Vector3.Dot(spring.up, springWorldVelocity);

                float suspensionForce = springOffsetForce - (springUpVelocity * _suspensionDamper);

                _rb.AddForceAtPosition(suspensionForce * spring.up, spring.position, ForceMode.Acceleration);

                _suspensionContactNormals.Add(hit.normal);
            }
        }
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
            //force = new Vector3(force.x, _rb.velocity.y, force.z);
            AccelerateTo(force, _acceleration);
        }
    }

    private void AccelerateTo(Vector3 targetVelocity, float maxAccel)
    {
        Vector3 deltaV = targetVelocity - _rb.velocity;
        Vector3 accel = deltaV / Time.deltaTime;

        if (accel.sqrMagnitude > maxAccel * maxAccel)
            accel = accel.normalized * maxAccel;

        _rb.AddForceAtPosition(accel, ApplyMovementForceAt, ForceMode.Acceleration);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // movement direction
        Debug.DrawRay(transform.position, _projectedDirection * 2f, Color.yellow);

        // springs
        for (int i = 0; i < _springs.Length; i++)
        {
            Transform spring = _springs[i];
            Debug.DrawRay(spring.position, -spring.up * _suspensionRestLength, Color.green);
        }
    }
#endif

}
