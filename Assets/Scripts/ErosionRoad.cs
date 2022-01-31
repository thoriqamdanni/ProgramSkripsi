using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErosionRoad : MonoBehaviour
{
    private ErosionsManager erosionManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<CarController>(out CarController car))
        {
            car.RespawnNearCheckpoint();
            erosionManager.CarHitErosion(other.transform);
        }
    }

    public void SetErosionManager(ErosionsManager erosionManager)
    {
        this.erosionManager = erosionManager;
    }
}
