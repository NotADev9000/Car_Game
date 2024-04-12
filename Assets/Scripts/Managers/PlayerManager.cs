using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private VehicleController Player;

    private void OnEnable()
    {
        VoidContact.OnVoidTriggered += OnVoidTriggered;
    }

    private void OnDisable()
    {
        VoidContact.OnVoidTriggered -= OnVoidTriggered;
    }

    private void OnVoidTriggered(Collider obj)
    {
        if (obj.gameObject == Player.gameObject)
        {
            TeleportPlayerToLastCheckpoint();
        }
    }

    private void TeleportPlayerToLastCheckpoint()
    {
        // TODO: get last checkpoint here
        Player.ResetAllMovement();
        Player.transform.rotation = Quaternion.identity;
        Player.transform.position = Vector3.zero + new Vector3(0, 2, 0);
    }
}
