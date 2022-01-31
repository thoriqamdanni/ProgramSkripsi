using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarInputHandler : MonoBehaviour
{
    CarController carController;

    private Vector2 inputVector = Vector2.zero;
    private bool inBrake = false;

    private void Awake()
    {
        carController = GetComponent<CarController>();
    }

    private void Update()
    {
        inputVector = Vector2.zero;
        inBrake = false;
        inputVector.x = Input.GetAxis("Horizontal");
        inputVector.y = Input.GetAxis("Vertical");

        if (Input.GetKey(KeyCode.Space))
            inBrake = true;

        carController.SetInputVector(inputVector, inBrake);
    }
}
