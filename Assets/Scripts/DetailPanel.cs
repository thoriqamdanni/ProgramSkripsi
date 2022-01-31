using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DetailPanel : MonoBehaviour
{
    public GameObject player;
    public Text speedText;
    public Transform speedPointer;
    public float maxDegree = 81f;

    [SerializeField] private float _carSpeed = 0f;
    private float _pointerFactor = 0f;

    // Components
    CarController carController;

    private void Awake()
    {
        speedText.text = string.Format("Speed: {0}", _carSpeed);
        carController = player.GetComponent<CarController>();
        speedPointer.localRotation = Quaternion.Euler(0, 0, maxDegree);
        _pointerFactor = 2 * maxDegree / carController.maxSpeed;

        if(!carController)
        {
            Debug.Log("CarController inside gameObject Player doesn't exist");
        }
    }

    private void Update()
    {
        // Velocity up is the same as car's speed
        _carSpeed = carController.VelocityUp;
        speedText.text = string.Format("Speed: {0:0}", _carSpeed);

        // Rotate speed pointer
        if(speedPointer.localRotation.z >= -maxDegree && speedPointer.localRotation.z <= maxDegree)
        {
            speedPointer.localRotation = Quaternion.Euler(0, 0, maxDegree - _carSpeed * _pointerFactor);
        }
    }
}
