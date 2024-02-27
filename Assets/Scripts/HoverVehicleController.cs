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

    [SerializeField] private Transform[] _wheels = Array.Empty<Transform>();
    [SerializeField] private float _suspensionLength = 0.6f;
    [SerializeField] private float _suspensionForce = 5f;

    [SerializeField] private float _maxSpeed = 25f; // m/s
    [SerializeField] private float _acceleration = 15f;  // m/s^2
    //[SerializeField] private float _rotationSpeed = 60f; // deg/s

    private Rigidbody _rb;

    private Vector2 _inputVector;

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
        // SUSPENSION
        Color debugColor;
        RaycastHit hit;
        foreach (Transform wheel in _wheels)
        {
            if (Physics.Raycast(wheel.position, Vector3.down, out hit, _suspensionLength))
            {
                Debug.Log("touching: " + wheel.gameObject.name.ToString());
                float compressionRatio = 1 - (hit.distance/_suspensionLength);
                _rb.AddForceAtPosition(compressionRatio * Vector3.up * _suspensionForce, wheel.position);
                debugColor = Color.green;
            } 
            else
            {
                debugColor = Color.red;
            }

            Debug.DrawRay(wheel.position, Vector3.down * _suspensionLength, debugColor);
        }

        // MOVEMENT
        Vector3 force = transform.forward * _inputVector.y * _maxSpeed;
        force = new Vector3(force.x, _rb.velocity.y, force.z);
        AccelerateTo(force, _acceleration);
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
