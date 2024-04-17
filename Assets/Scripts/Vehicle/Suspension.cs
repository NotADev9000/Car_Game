using System;
using UnityEngine;

[System.Serializable]
public class Suspension
{
    [SerializeField] private Transform _transform;
    public Transform Transform => _transform;

    public bool IsGrounded { get; private set; }
    public float CurrentLength { get; private set; }
    public Vector3 HitNormal { get; private set; }

    public void CastSpring(float restLength)
    {
        if (Physics.Raycast(Transform.position, -Transform.up, out RaycastHit hit, restLength, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            IsGrounded = true;
            CurrentLength = hit.distance;
            HitNormal = hit.normal;
        }
        else
        {
            IsGrounded = false;
            CurrentLength = restLength;
        }
    }

    public float CalculateSpringForces(float suspensionRestLength, float springStrength, Vector3 suspensionWorldVelocity, float springDamper)
    {
        float springOffset = suspensionRestLength - CurrentLength;
        float springOffsetForce = springOffset * springStrength;

        float suspensionUpVelocity = Vector3.Dot(Transform.up, suspensionWorldVelocity);

        return springOffsetForce - (suspensionUpVelocity * springDamper);
    }
}