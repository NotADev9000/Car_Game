using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    //[SerializeField] private float _rotationSpeed = 60f; // deg/s

    private Rigidbody _rb;

    private Vector2 _inputVector;

    private List<Vector3> _suspensionContactNormals = new List<Vector3>();

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        //_rb.centerOfMass = _centreOfMass.localPosition;
    }

    private void OnEnable()
    {
        _inputReader.MoveEvent += OnMove;
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
                Debug.Log(force);

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

        // MOVEMENT
        Vector3 suspensionContactAverage = Vector3.zero;
        for (int i = 0; i < _suspensionContactNormals.Count; i++)
        {
            suspensionContactAverage += _suspensionContactNormals[i];
        }
        if (_suspensionContactNormals.Count > 0) suspensionContactAverage /= _suspensionContactNormals.Count;

        if (_inputVector.y != 0)
        {
            Vector3 force = _inputVector.y * _maxSpeed * transform.forward;
            //force = new Vector3(force.x, _rb.velocity.y, force.z);
            AccelerateTo(force, _acceleration);
        }
        else
        {
            Debug.Log(_inputVector.y);
        }
    }

    private void AccelerateTo(Vector3 targetVelocity, float maxAccel)
    {
        Vector3 deltaV = targetVelocity - _rb.velocity;
        Vector3 accel = deltaV / Time.fixedDeltaTime;

        if (accel.sqrMagnitude > maxAccel * maxAccel)
            accel = accel.normalized * maxAccel;

        _rb.AddForce(accel, ForceMode.Acceleration);
    }

    private void OnMove(Vector2 moveVector)
    {
        _inputVector = moveVector;
    }
}
