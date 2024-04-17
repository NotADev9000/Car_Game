using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(VehicleController))]
public class VehicleView : MonoBehaviour
{
    [Header("Suspension Settings")]
    [SerializeField] private Transform[] _visualWheels;
    [SerializeField] [Tooltip("How fast should wheels move to raycast position")] private float _snapWheelsToRaySpeed = 10f;

    private VehicleController _controller;

    private void Awake()
    {
        _controller = GetComponent<VehicleController>();
    }

    private void Update()
    {
        UpdateWheelVisuals();
    }

    private void UpdateWheelVisuals()
    {
        for (int i = 0; i < _controller.Suspensions.Length; i++)
        {
            float targetYPos = _controller.Suspensions[i].Transform.localPosition.y - _controller.Suspensions[i].CurrentLength;
            float changeYPos = targetYPos - _visualWheels[i].localPosition.y;

            _visualWheels[i].localPosition += new Vector3(0, changeYPos * Time.deltaTime * _snapWheelsToRaySpeed, 0);

            _visualWheels[i].localPosition = new Vector3(_visualWheels[i].localPosition.x,
                                                     _visualWheels[i].localPosition.y,
                                                     _visualWheels[i].localPosition.z);
        }
    }
}
