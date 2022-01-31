using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Barracuda;


public class CarsManager : MonoBehaviour
{
    public GameObject player;
    public List<GameObject> cars;
    public GameObject obstacles;
    public bool doLap = true;
    public int maxLap = 1;

    [Header("AI Brains")]
    public NNModel AI;
    public NNModel AIObs;

    [SerializeField] private int _npcPlay = 0;
    [SerializeField] private bool _playWithObstacles = false;
    [SerializeField] private bool _playWithAIObs = false;
    [SerializeField] private double currentTime = 0f;

    private void Awake()
    {
        currentTime = 0f;
        _playWithAIObs = MainMenuManager._playWithAIObs;
        _playWithObstacles = MainMenuManager._playWithObstacles;

        if (_playWithObstacles)
            obstacles.SetActive(true);
        else
            obstacles.SetActive(false);

        if (_playWithAIObs)
        {
            player.GetComponent<Unity.MLAgents.Policies.BehaviorParameters>().Model = AIObs;

            foreach (var car in cars)
            {
                car.GetComponent<Unity.MLAgents.Policies.BehaviorParameters>().Model = AIObs;
            }
        }
        else
        {
            player.GetComponent<Unity.MLAgents.Policies.BehaviorParameters>().Model = AI;

            foreach (var car in cars)
            {
                car.GetComponent<Unity.MLAgents.Policies.BehaviorParameters>().Model = AI;
            }
        }
    }

    private void Start()
    {
        _npcPlay = MainMenuManager._npcPlay;

        if(_npcPlay != 0)
        {
            Debug.Log("Played by " + _npcPlay + " NPC");
            for (int i = 0; i < _npcPlay; i++)
            {
                cars[i].SetActive(true);
            }
        } else
        {
            Debug.Log("Player play alone");
        }


        // For training agent w/o obstacles
        //_playWithObstacles = true;
    }

    private void Update()
    {
        currentTime += Time.deltaTime;
    }

    public double getTime()
    {
        return currentTime;
    }
}
