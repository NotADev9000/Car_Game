using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class VehicleController : MonoBehaviour
{
    [SerializeField] private InputReader _inputReader;

    [SerializeField] private Transform _centreOfMass;

    [SerializeField] private float _acceleration = 15f;  // m/s^2
    [SerializeField] private float _rotationSpeed = 60f; // deg/s

    [SerializeField] private float _maxSpeed = 25f; // m/s

    private Vector2 _inputVector;

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        _rb.centerOfMass = _centreOfMass.localPosition;
    }

    private void OnEnable()
    {
        _inputReader.MoveEvent += OnMove;
    }

    private void FixedUpdate()
    {
        Vector3 force = transform.forward * _inputVector.y * _maxSpeed;
        force = new Vector3(force.x, _rb.velocity.y, force.z);
        AccelerateTo(force, _acceleration);

        //_rb.AddForce(force, ForceMode.Acceleration);

        if (_inputVector.x != 0 && _inputVector.y != 0)
        {
            float rotateVelocity = _inputVector.x * _inputVector.y * _rotationSpeed * Time.fixedDeltaTime;
            Quaternion rotationChange = Quaternion.Euler(0f, rotateVelocity, 0f);
            _rb.MoveRotation(_rb.rotation * rotationChange);
        }

        //_rb.angularVelocity = Vector3.zero;

        if (_rb.velocity.magnitude > 0.1)
        {
            Debug.Log("velocity magnitude: " + _rb.velocity.magnitude.ToString());
            Debug.Log("velocity dot up magnitude: " + Vector3.Dot(transform.up, _rb.velocity).ToString());
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
