using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainSystem : MonoBehaviour
{
    public event EventHandler OnGateClose; // When train gate closing
    public event EventHandler OnGateOpen; // When train gate opening

    [SerializeField] private int minTime = 5;
    [SerializeField] private int maxTime = 15;
    [SerializeField] private int launchTime = 5;
    [SerializeField] private float durationFence = 10f;
    [SerializeField] private float durationTrain = 10f;
    [SerializeField] private bool isOpen = true;
    [SerializeField] private List<FenceSingle> gatesList;
    private GameObject train;

    [SerializeField] private float _time = 0f;
    private bool _firstLaunch = false;

    private void Awake()
    {
        launchTime = UnityEngine.Random.Range(minTime, maxTime);
        Transform fencesTransform = transform.Find("Fences");
        train = GameObject.Find("Train");
        train.SetActive(false);
        gatesList = new List<FenceSingle>();
        
        foreach(Transform fence in fencesTransform)
        {
            FenceSingle f = fence.GetComponent<FenceSingle>();
            f.SetTrainSystem(this);
            gatesList.Add(f);
        }
    }

    private void Update()
    {
        _time += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.R))
            StartCoroutine(LaunchTrain());

        if (_time >= launchTime && !_firstLaunch)
        {
            StartCoroutine(LaunchTrain());
            _firstLaunch = true;
        }
    }

    public float GetDurationFence() { return durationFence; }

    public float GetDurationTrain() { return durationTrain; }

    public bool IsOpen() { return isOpen; }

    IEnumerator LaunchTrain()
    {
        isOpen = !isOpen;
        OnGateClose?.Invoke(this, EventArgs.Empty);

        foreach (FenceSingle fence in gatesList)
        {
            fence.LoadGate();
            Debug.Log("Close Gate!");
        }

        yield return new WaitForSeconds(durationFence);

        train.SetActive(true);

        yield return new WaitForSeconds(durationTrain+launchTime-durationFence);

        isOpen = !isOpen;
        OnGateOpen?.Invoke(this, EventArgs.Empty);

        foreach (FenceSingle fence in gatesList)
        {
            fence.LoadGate();
            Debug.Log("Open Gate");
        }
    }
}
