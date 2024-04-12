using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class VoidContact : MonoBehaviour
{
    public static event Action<Collider> OnVoidTriggered;

    private void OnTriggerEnter(Collider other)
    {
        OnVoidTriggered?.Invoke(other);
    }
}
