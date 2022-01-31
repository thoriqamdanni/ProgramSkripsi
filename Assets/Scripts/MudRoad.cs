using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MudRoad : MonoBehaviour
{
    private MudsManager mudsManager;
    public float kineticConstant = 0.6f;
    public float decreaseMaxSpeedFactor = 0.3f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<CarController>(out CarController car))
        {
            car.engineForce *= kineticConstant;
            car.maxSpeed *= decreaseMaxSpeedFactor;
            mudsManager.CarHitMud(other.transform);
        }

        // Pull force to the car from the mud
        if (other.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.velocity *= kineticConstant;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<CarController>(out CarController car))
        {
            car.engineForce /= kineticConstant;
            car.maxSpeed /= decreaseMaxSpeedFactor;
            mudsManager.CarExitMud(other.transform);
        }
    }

    public void SetMudManager(MudsManager mudsManager)
    {
        this.mudsManager = mudsManager;
    }
}
