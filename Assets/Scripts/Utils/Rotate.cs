using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 0f;
    [SerializeField] private bool _rotateClockwise = true;

    private void Update()
    {
        int turnClockwise = _rotateClockwise ? 1 : -1;
        transform.Rotate(0, _rotationSpeed * Time.deltaTime * turnClockwise, 0);
    }
}
