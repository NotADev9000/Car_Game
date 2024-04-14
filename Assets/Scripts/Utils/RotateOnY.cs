using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateOnY : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 0f;
    [SerializeField] private bool _rotateClockwise = true;

    private void Update()
    {
        int turnDirection = _rotateClockwise ? 1 : -1;
        transform.Rotate(0, _rotationSpeed * Time.deltaTime * turnDirection, 0);
    }
}
