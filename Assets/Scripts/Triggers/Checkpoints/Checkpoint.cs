using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Vector3 _respawnPosition;

    public static event Action<Vector3> OnCheckpointTriggered;

    private void OnTriggerEnter()
    {
        OnCheckpointTriggered?.Invoke(_respawnPosition);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(_respawnPosition, 1f);
    }
#endif
}
