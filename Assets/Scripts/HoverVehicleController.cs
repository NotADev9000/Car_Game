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

    [Header("Suspension Settings")]
    [SerializeField] private Transform[] _wheels = Array.Empty<Transform>();
    [SerializeField] private float _suspensionLength = 0.6f;
    [SerializeField] private float _springStrength = 5f;
    [SerializeField] private float _springDamper = 2f;

    [Header("Vehicle Settings")]
    [SerializeField] private float _maxSpeed = 25f; // m/s
    [SerializeField] private float _acceleration = 15f;  // m/s^2
    [SerializeField] private float _rotationSpeed = 60f; // deg/s
    [SerializeField][Range(0, 1)][Tooltip("0 = no grip")] private float _gripFactor = 0.5f;

    [Header("Force Settings")]
    [SerializeField] private float _applyForceForward = 0f;
    [SerializeField] private float _applyForceDown = 0f;

    private Rigidbody _rb;

    private Vector2 _inputVector;

    private List<Vector3> _suspensionContactNormals = new List<Vector3>();
    private bool _isGrounded = false;
    Vector3 applyForceAt;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        //_rb.centerOfMass = _centreOfMass.localPosition;
    }

    private void OnEnable()
    {
        _inputReader.MoveEvent += OnMove;
    }

    private void Update()
    {
        applyForceAt = transform.position + _rb.centerOfMass + (transform.forward * _applyForceForward) + (-transform.up * _applyForceDown);
    }

    private void FixedUpdate()
    {
        _suspensionContactNormals.Clear();

        // SUSPENSION
        Color debugColor;
        RaycastHit hit;
        foreach (Transform wheel in _wheels)
        {
            if (Physics.Raycast(wheel.position, -wheel.up, out hit, _suspensionLength))
            {
                float compressionRatio = (float)Math.Round(1 - (hit.distance/_suspensionLength), 2);
                float springOffsetForce = compressionRatio * _springStrength;
                
                Vector3 wheelWorldVelocity = _rb.GetPointVelocity(wheel.position); 
                float springVelocity = Vector3.Dot(wheel.up, wheelWorldVelocity);

                float force = springOffsetForce - (springVelocity * _springDamper);

                _rb.AddForceAtPosition(force * wheel.up, wheel.position, ForceMode.Acceleration);

                _suspensionContactNormals.Add(hit.normal);

                debugColor = Color.green;
            } 
            else
            {
                debugColor = Color.red;
            }

            Debug.DrawRay(wheel.position, -wheel.up * _suspensionLength, debugColor);
        }

        // grounded if at least 2 wheels are on ground
        _isGrounded = _suspensionContactNormals.Count > 1;

        // calculate the direction to accelerate the vehicle
        // this is done to prevent the vehicle moving towards the sky/ground too much when tilted
        // this is an average of all the surfaces the wheels touch
        Vector3 suspensionContactAverage = Vector3.zero;
        for (int i = 0; i < _suspensionContactNormals.Count; i++)
        {
            suspensionContactAverage += _suspensionContactNormals[i];
        }
        if (_suspensionContactNormals.Count > 0) 
            suspensionContactAverage = (suspensionContactAverage / _suspensionContactNormals.Count).normalized;
        Vector3 projectedDirection = Vector3.ProjectOnPlane(transform.forward, suspensionContactAverage);

        Debug.DrawRay(transform.position, projectedDirection * 3f, Color.green);

        if (_isGrounded)
        {
            // TRACTION
            float sidewaysVelocity = Vector3.Dot(transform.right, _rb.velocity);
            float oppositeTractionVelocity = -sidewaysVelocity * _gripFactor;
            Vector3 tractionForce = transform.right * (oppositeTractionVelocity / Time.fixedDeltaTime);
            _rb.AddForce(tractionForce, ForceMode.Acceleration);

            // STEERING
            if (_rb.velocity.magnitude > 0.0f)
            {
                Vector3 steerForce = _inputVector.x * _rotationSpeed * Vector3.up;
                _rb.AddTorque(steerForce, ForceMode.Acceleration);
            }

            // MOVEMENT
            if (_inputVector.y != 0)
            {
                Vector3 force = _inputVector.y * _maxSpeed * projectedDirection;
                force = new Vector3(force.x, _rb.velocity.y, force.z);
                AccelerateTo(force, _acceleration);
            }
        }

        Debug.DrawRay(applyForceAt, Vector3.down, Color.magenta);
    }

    private void AccelerateTo(Vector3 targetVelocity, float maxAccel)
    {
        Vector3 deltaV = targetVelocity - _rb.velocity;
        Vector3 accel = deltaV / Time.deltaTime;

        if (accel.sqrMagnitude > maxAccel * maxAccel)
            accel = accel.normalized * maxAccel;

        //_rb.AddForce(accel, ForceMode.Acceleration);

        _rb.AddForceAtPosition(accel, applyForceAt, ForceMode.Acceleration);
    }

    private void OnMove(Vector2 moveVector)
    {
        _inputVector = moveVector;
    }
}
