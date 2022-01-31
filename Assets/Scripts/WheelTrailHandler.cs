using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelTrailHandler : MonoBehaviour
{
    CarController carController;
    TrailRenderer trailRenderer;

    void Awake()
    {
        carController = GetComponentInParent<CarController>();
        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.emitting = false;
    }

    void Update()
    {
        if (carController.IsTireScreeching(out float lateralVelocity, out bool reverse))
            trailRenderer.emitting = true;
        else trailRenderer.emitting = false;
    }
}
