using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Car Basic Settings")]
    public float driftFactor = 0.95f;
    public float engineForce = 6000f;
    public float turnFactor = 3.5f;
    public float dragForce = 3f;
    public float minSpeedFactor = 8f;
    public float maxSpeed = 60f;
    public float reverseMaxFactor = 0.5f;

    [Header("Car Brake System")]
    public float brakeForce = 12f;
    public float inDriftFactor = 1;

    [Header("Car Detail")]
    [SerializeField] private bool isFinished = false;
    [SerializeField] private int lap = 0;
    [SerializeField] private List<double> timeLap = new List<double>();
    [SerializeField] private CarsManager carManager;
    [SerializeField] private TrackCheckpoints trackCheckpoints;

    public float VelocityUp { get; set; }
    public float RotationAngle { get; set; }

    private float accelerationInput = 0f;
    private float steeringInput = 0f;
    private float _driftFactor = 0f;
    private bool inBrake = false;
    private bool _initialState = true;

    // Components
    Rigidbody2D carRb;

    private void Awake()
    {
        carRb = GetComponent<Rigidbody2D>();

        if (MainMenuManager._playBy == "NPC")
        {
            Debug.Log("Played By NPC");
            CarInputHandler inputHandler = GetComponent<CarInputHandler>();
            CarDriverAgent driverAgent = GetComponent<CarDriverAgent>();
            inputHandler.enabled = false;
            driverAgent.enabled = true;
        }
    }

    private void Start()
    {
        trackCheckpoints.OnCarFinishLap += Checkpoints_OnCarFinishLap;
    }

    private void FixedUpdate()
    {
        ApplyEngineForce();
        ApplyBrake();
        KillOrthogonalVelocity();
        ApplySteering();
    }

    void ApplyEngineForce()
    {
        VelocityUp = Vector2.Dot(transform.up, carRb.velocity);
            
        // Limit max forward speed
        if (VelocityUp > maxSpeed && accelerationInput > 0)
            return;

        // Limit max reverse speed
        if (VelocityUp < -maxSpeed * reverseMaxFactor && accelerationInput < 0)
            return;

        // Apply drag if there's no input
        if (accelerationInput == 0)
            carRb.drag = Mathf.Lerp(carRb.drag, dragForce, Time.fixedDeltaTime * dragForce);
        else
            carRb.drag = 0;

        Vector2 carEngineForce = transform.up * accelerationInput * engineForce;

        carRb.AddForce(carEngineForce, ForceMode2D.Force);
    }

    void ApplySteering()
    {
        // Limit cars ability to turn when moving slowly
        float minSpeed = carRb.velocity.magnitude / minSpeedFactor;
        minSpeed = Mathf.Clamp01(minSpeed);

        if(_initialState)
        {
            carRb.velocity = Vector2.zero;
            carRb.angularVelocity = 0f;
            this.RotationAngle = -90f;
            _initialState = false;
        }

        RotationAngle -= steeringInput * turnFactor * minSpeed;

        carRb.MoveRotation(RotationAngle);
    }

    void KillOrthogonalVelocity()
    {
        Vector2 forwardVelocity = transform.up * Vector2.Dot(carRb.velocity, transform.up);
        Vector2 rightVelocity = transform.right * Vector2.Dot(carRb.velocity, transform.right);

        carRb.velocity = forwardVelocity + rightVelocity * _driftFactor;
    }

    void ApplyBrake()
    {
        // Apply brake if it is true
        if (inBrake)
        {
            carRb.drag = Mathf.Lerp(carRb.drag, brakeForce, Time.fixedDeltaTime * brakeForce);
            _driftFactor = inDriftFactor;
        }
        else _driftFactor = driftFactor;
    }

    public float GetLateralVelocity()
    {
        return Vector2.Dot(transform.right, carRb.velocity);
    }

    public bool IsTireScreeching(out float lateralVelocity, out bool reverse)
    {
        lateralVelocity = GetLateralVelocity();
        reverse = false;

        if(accelerationInput < 0 && VelocityUp > 0)
        {
            reverse = true;
            return true;
        }

        if (Mathf.Abs(lateralVelocity) > 4f)
        {
            return true;
        }

        return false;
    }

    public void SetInputVector(Vector2 inputVector, bool inBrake)
    {
        steeringInput = inputVector.x;
        accelerationInput = inputVector.y;
        this.inBrake = inBrake;
    }

    public void StopCar()
    {
        carRb.velocity = Vector2.zero;
        carRb.angularVelocity = 0f;
        this.RotationAngle = -90f;
    }

    public void RespawnNearCheckpoint()
    {
        var obj = trackCheckpoints.GetCurrentCheckPoint(transform);
        
        carRb.velocity = Vector2.zero;
        carRb.angularVelocity = 0f;

        if(obj == null)
        {
            // Respawn at start
            GetComponent<CarDriverAgent>().SpawnRandom();
            StopCar();
        }
        else
        {
            // Respawn near latest checkpoints
            Transform checkpoint = obj.transform;
            transform.up = checkpoint.up;
            transform.position = checkpoint.position;
            this.RotationAngle = checkpoint.eulerAngles.z;
        }
    }

    public void increaseLap()
    {
        if(!isFinished)
        {
            lap++;
            timeLap.Add(Math.Round(carManager.getTime(), 2));

            Debug.Log("Current Lap: " + lap);
        }
    }

    public void resetLap()
    {
        lap = 0;
    }

    public int getLap()
    {
        return lap;
    }

    public double getlapTime(int i)
    {
        return timeLap[i];
    }

    public void setFinished(bool finish)
    {
        this.isFinished = finish;
    }

    public bool getFinished()
    {
        return this.isFinished;
    }

    private void Checkpoints_OnCarFinishLap(object sender, CarCheckpointsEventArgs e)
    {
        if (e.carTransform == transform)
        {
            CarController careC = e.carTransform.GetComponent<CarController>();
            Debug.Log("Yes this car just finish a lap (Cars Controller)");
            careC.increaseLap();

            if (careC.getLap() >= carManager.maxLap)
            {
                Debug.Log("This car just finish all lap!");
                careC.setFinished(true);
            }
        }
    }
}
