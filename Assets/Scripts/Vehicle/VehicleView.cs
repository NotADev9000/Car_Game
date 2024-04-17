using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(VehicleController))]
public class VehicleView : MonoBehaviour
{
    [Header("Suspension Settings")]
    [SerializeField] private Transform[] _visualWheels;
    [SerializeField] [Tooltip("How fast should wheels move to raycast position")] private float _moveWheelsToPositionSpeed = 10f;
    [SerializeField] [Tooltip("How fast should wheels rotate upon input")] private float _moveWheelsToRotationSpeed = 1f;
    [SerializeField] private float _wheelMaxTurnAngle = 25f;

    private VehicleController _controller;

    private void Awake()
    {
        _controller = GetComponent<VehicleController>();
    }

    private void Update()
    {
        UpdateWheelPosition();
        UpdateWheelRotation();
    }

    private void UpdateWheelPosition()
    {
        for (int i = 0; i < _controller.Suspensions.Length; i++)
        {
            float targetYPos = _controller.Suspensions[i].Transform.localPosition.y - _controller.Suspensions[i].CurrentLength;
            float changeYPos = targetYPos - _visualWheels[i].localPosition.y;

            _visualWheels[i].localPosition += new Vector3(0, changeYPos * Time.deltaTime * _moveWheelsToPositionSpeed, 0);

            _visualWheels[i].localPosition = new Vector3(_visualWheels[i].localPosition.x,
                                                     _visualWheels[i].localPosition.y,
                                                     _visualWheels[i].localPosition.z);
        }
    }

    private void UpdateWheelRotation()
    {
        for (int i = 0; i < Mathf.Min(_controller.Suspensions.Length, 2); i++)
        {
            Vector3 targetRotation = new Vector3(0f, _wheelMaxTurnAngle * _controller.InputVector.x, 0f);
            _visualWheels[i].transform.localRotation = Quaternion.RotateTowards(_visualWheels[i].transform.localRotation, 
                                                                                Quaternion.Euler(targetRotation), 
                                                                                _moveWheelsToRotationSpeed);
        }
    }
}
