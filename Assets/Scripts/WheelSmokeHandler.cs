using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelSmokeHandler : MonoBehaviour
{
    [SerializeField] float particleEmissionRate = 0f;

    CarController carController;
    ParticleSystem particleSystem;
    ParticleSystem.EmissionModule particleSystemEmissionModule;

    void Awake()
    {
        carController = GetComponentInParent<CarController>();
        particleSystem = GetComponent<ParticleSystem>();
        particleSystemEmissionModule = particleSystem.emission;
        particleSystemEmissionModule.rateOverTime = 0;
    }

    void Update()
    {
        particleEmissionRate = Mathf.Lerp(particleEmissionRate, 0, Time.deltaTime * 5f);
        particleSystemEmissionModule.rateOverTime = particleEmissionRate;

        if(carController.IsTireScreeching(out float lateralVelocity, out bool reverse))
        {
            if (reverse)
                particleEmissionRate = 30f;
            else particleEmissionRate = Mathf.Abs(lateralVelocity) * 2f;
        }
    }
}
