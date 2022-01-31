using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErosionsManager : MonoBehaviour
{
    public event EventHandler<CarCheckpointsEventArgs> OnCarHitErosion; // If car hit an erosion

    [SerializeField] private List<ErosionRoad> erosionList;
    private CarCheckpointsEventArgs carArgs;

    private void Awake()
    {
        Transform erosionTransform = this.transform;
        erosionList = new List<ErosionRoad>();

        foreach (Transform erosion in erosionTransform)
        {
            ErosionRoad erosionRoad = erosion.GetComponent<ErosionRoad>();
            erosionRoad.SetErosionManager(this);
            erosionList.Add(erosionRoad);
        }

        carArgs = new CarCheckpointsEventArgs();
    }

    public void CarHitErosion(Transform carTransform)
    {
        carArgs.carTransform = carTransform;
        OnCarHitErosion?.Invoke(this, carArgs);
    }
}
