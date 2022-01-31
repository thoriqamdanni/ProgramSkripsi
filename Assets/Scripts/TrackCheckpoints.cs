using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackCheckpoints : MonoBehaviour
{
    public event EventHandler<CarCheckpointsEventArgs> OnCarCorrectCheckpoint; // If car passes on the correct checkpoint
    public event EventHandler<CarCheckpointsEventArgs> OnCarWrongCheckpoint; // If car not pass on the correct checkpoint
    public event EventHandler<CarCheckpointsEventArgs> OnCarFinishLap; // If car finish a lap

    [SerializeField] private List<Transform> carTransformList;
    private List<CheckpointSingle> checkpointsList;
    private List<int> nextCheckpointIndexIndexList;
    private CarCheckpointsEventArgs carArgs;

    private void Awake()
    {
        Transform checkpointsTransform = transform.Find("Checkpoints");
        checkpointsList = new List<CheckpointSingle>();

        foreach(Transform checkpoint in checkpointsTransform)
        {
            CheckpointSingle checkpointSingle = checkpoint.GetComponent<CheckpointSingle>();
            checkpointSingle.SetTrackCheckpoints(this);
            checkpointsList.Add(checkpointSingle);
        }

        // Initialize next checkpoint index as 0 in every cars
        nextCheckpointIndexIndexList = new List<int>();
        foreach(Transform carTransform in carTransformList)
        {
            nextCheckpointIndexIndexList.Add(0);
        }

        carArgs = new CarCheckpointsEventArgs();
    }

    public void CarPassCheckpoint(CheckpointSingle checkpoint, Transform carTransform)
    {
        // Get checkpoint index for corresponding car
        int nextCheckpointIndex = nextCheckpointIndexIndexList[carTransformList.IndexOf(carTransform)];
        carArgs.carTransform = carTransform;
        if (checkpointsList.IndexOf(checkpoint) == nextCheckpointIndex)
        {
            // Correct checkpoint
            Debug.Log("Correct");
            CheckpointSingle correctCheckpoint = checkpointsList[nextCheckpointIndex];
            correctCheckpoint.Checked();

            if(nextCheckpointIndex == checkpointsList.Count - 1)
            {
                // Reset checkpoint index to 0 after finish a lap
                nextCheckpointIndexIndexList[carTransformList.IndexOf(carTransform)] = 0;
                OnCarFinishLap?.Invoke(this, carArgs);

                // Uncheck all checkpoints to blue color
                foreach(CheckpointSingle checkpointSingle in checkpointsList)
                {
                    checkpointSingle.UnCheck();
                }
            }
            else
            {
                nextCheckpointIndexIndexList[carTransformList.IndexOf(carTransform)]++;
            }

            OnCarCorrectCheckpoint?.Invoke(this, carArgs);
        } 
        else
        {
            // Wrong checkpoint
            Debug.Log("Wrong");
            OnCarWrongCheckpoint?.Invoke(this, carArgs);

            CheckpointSingle wrongCheckpoint = checkpointsList[nextCheckpointIndex];
            wrongCheckpoint.NextCheck();
        }
    }

    public GameObject GetNextCheckPoint(Transform carTransform)
    {
        int nextCheckpointIndex = nextCheckpointIndexIndexList[carTransformList.IndexOf(carTransform)];
        CheckpointSingle correctCheckpoint = checkpointsList[nextCheckpointIndex];
        return correctCheckpoint.gameObject;
    }

    public GameObject GetCurrentCheckPoint(Transform carTransform)
    {
        int nextCheckpointIndex = nextCheckpointIndexIndexList[carTransformList.IndexOf(carTransform)];

        if (nextCheckpointIndex == 0)
            return null;

        CheckpointSingle currentCheckpoint = checkpointsList[nextCheckpointIndex - 1];
        return currentCheckpoint.gameObject;
    }

    public void ResetCheckpoints(Transform carTransform)
    {
        nextCheckpointIndexIndexList[carTransformList.IndexOf(carTransform)] = 0;

        if (carTransform.TryGetComponent<CarController>(out CarController car))
        {
            car.resetLap();
        }
    }

    public List<Transform> getCarTransformList()
    {
        return carTransformList;
    }
}
