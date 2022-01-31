using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class CarDriverAgent : Agent
{
    public bool doSpawnRandom = true;

    [SerializeField] private TrackCheckpoints trackCheckpoints;
    [SerializeField] private ErosionsManager erosionsManager;
    [SerializeField] private MudsManager mudsManager;
    [SerializeField] private TrainSystem trainSystem;
    [SerializeField] private Transform respawn;

    private CarController carController;
    private float time = 0f;
    private bool _isTrainMove = false;
 

    private void Awake()
    {
        carController = GetComponent<CarController>();
    }

    private void Start()
    {
        trackCheckpoints.OnCarCorrectCheckpoint += CheckPoints_OnCarCorrectCheckpoint;
        trackCheckpoints.OnCarWrongCheckpoint += CheckPoints_OnCarWrongCheckpoint;
        trackCheckpoints.OnCarFinishLap += CheckPoints_OnCarFinishLap;

        erosionsManager.OnCarHitErosion += Erosion_OnCarHitErosion;

        mudsManager.OnCarHitMud += Mud_OnCarHitMud;

        trainSystem.OnGateClose += Train_OnGateClose;
        trainSystem.OnGateOpen += Train_OnGateOpen;
    }

    private void Update()
    {
        // Anti lazy
        // Punish car that waste time every seconds and reward if moving forward
         time += Time.deltaTime;

        if(time >= 1f)
        {
            time  = 0f;
            AddReward(-0.05f);

            if (carController.VelocityUp > 0f)
                AddReward(0.1f);
            else if (carController.VelocityUp <= 0f)
                AddReward(-0.1f);
        }

        
    }

    private void CheckPoints_OnCarCorrectCheckpoint(object sender, CarCheckpointsEventArgs e)
    {
        if (e.carTransform == transform)
        {
            Debug.Log("Yes this is correct checkpoint in THIS car");
            AddReward(1f);
        }
    }


    private void CheckPoints_OnCarWrongCheckpoint(object sender, CarCheckpointsEventArgs e)
    {
        if (e.carTransform == transform)
        {
            Debug.Log("Yes this is wrong checkpoint in THIS car");
            AddReward(-1f);

            // Respawn from the latest checkpoints, off it when training
            e.carTransform.GetComponent<CarController>().RespawnNearCheckpoint();
            
            //EndEpisode();
        }
    }

    private void CheckPoints_OnCarFinishLap(object sender, CarCheckpointsEventArgs e)
    {
        if (e.carTransform == transform)
        {
            Debug.Log("Yes this is lap in THIS car");
            AddReward(1f);
            //EndEpisode();
        }
    }

    private void Erosion_OnCarHitErosion(object sender, CarCheckpointsEventArgs e)
    {
        if (e.carTransform == transform)
        {
            Debug.Log("Yes this car hit erosion!");
            AddReward(-1f);
            //EndEpisode();
        }
    }

    private void Mud_OnCarHitMud(object sender, CarCheckpointsEventArgs e)
    {
        if (e.carTransform == transform)
        {
            Debug.Log("Yes this car hit mud!");
            AddReward(-1f);
            //EndEpisode();
        }
    }
    
    private void Train_OnGateClose(object sender, EventArgs e)
    {
        _isTrainMove = true;
    }

    private void Train_OnGateOpen(object sender, EventArgs e)
    {
        _isTrainMove = false;
    }

    public override void OnEpisodeBegin()
    {
        if(doSpawnRandom)
            SpawnRandom();
        trackCheckpoints.ResetCheckpoints(transform);
        carController.StopCar();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector2 checkpointForward = trackCheckpoints.GetNextCheckPoint(transform).transform.up;
        float directionDot = Vector3.Dot(transform.up, checkpointForward);
        
        sensor.AddObservation(directionDot);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector2 inputVector = Vector2.zero;
        bool inBrake = false;

        switch(actions.DiscreteActions[0])
        {
            case 0: inputVector.y = 0f; break;
            case 1: inputVector.y = +1f; break;
            case 2: inputVector.y = -1f; break;
        }

        switch(actions.DiscreteActions[1])
        {
            case 0: inputVector.x = 0f; break;
            case 1: inputVector.x = +1f; break;
            case 2: inputVector.x = -1f; break;
        }

        switch(actions.DiscreteActions[2])
        {
            case 0: inBrake = false; break;
            case 1: inBrake = true; break;
        }

        carController.SetInputVector(inputVector, inBrake);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        int accelerationAction = 0;
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) accelerationAction = 1;
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) accelerationAction = 2;

        int steeringAction = 0;
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) steeringAction = 1;
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) steeringAction = 2;

        int brakeAction = 0;
        if (Input.GetKey(KeyCode.Space)) brakeAction = 1;

        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = accelerationAction;
        discreteActions[1] = steeringAction;
        discreteActions[2] = brakeAction;
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.TryGetComponent<CarController>(out CarController car))
        {
            // Car hit & colliding with other car
            //AddReward(-1f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent<Wall>(out Wall wall))
        {
            // Car hit wall
            AddReward(-1f);
            //this.GetComponent<CarController>().RespawnNearCheckpoint();
            //EndEpisode();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent<Wall>(out Wall wall))
        {
            // Car hit & stay in wall
            AddReward(-0.01f);
        }
    }

    public void SpawnRandom()
    {
        transform.position = (Vector2)respawn.position + new Vector2(UnityEngine.Random.Range(-2f, 2f), UnityEngine.Random.Range(-2f, 2f));
        transform.up = respawn.up;
    }
}
