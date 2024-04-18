using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EndZone : MonoBehaviour
{
    public static event Action OnEndZoneTriggered;

    private void OnTriggerEnter()
    {
        OnEndZoneTriggered?.Invoke();
    }
}
