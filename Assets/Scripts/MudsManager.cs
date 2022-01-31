using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MudsManager : MonoBehaviour
{
    public event EventHandler<CarCheckpointsEventArgs> OnCarHitMud; // If car hit a mud
    public event EventHandler<CarCheckpointsEventArgs> OnCarExitMud; // If car exit a mud

    [SerializeField] private List<MudRoad> mudList;
    private CarCheckpointsEventArgs carArgs;

    private void Awake()
    {
        Transform mudTransform = this.transform;
        mudList = new List<MudRoad>();

        foreach (Transform mud in mudTransform)
        {
            MudRoad mudRoad = mud.GetComponent<MudRoad>();
            mudRoad.SetMudManager(this);
            mudList.Add(mudRoad);
        }

        carArgs = new CarCheckpointsEventArgs();
    }

    public void CarHitMud(Transform carTransform)
    {
        carArgs.carTransform = carTransform;
        OnCarHitMud?.Invoke(this, carArgs);
    }

    public void CarExitMud(Transform carTransform)
    {
        carArgs.carTransform = carTransform;
        OnCarExitMud?.Invoke(this, carArgs);
    }
}
