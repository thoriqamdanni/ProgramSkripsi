using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Train : MonoBehaviour
{
    public Transform point1;
    public Transform point2;

    private float _time;
    private float _trainLength;

    private TrainSystem trainSystem;

    private void Awake()
    {
        trainSystem = transform.GetComponentInParent<TrainSystem>();
        Transform tailCargo = transform.GetChild(transform.childCount - 1);
        _trainLength = transform.position.x - tailCargo.position.x;
    }

    private void OnEnable()
    {
        _time = 0f;
        transform.position = point1.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.TryGetComponent<CarController>(out CarController car))
        {
            Debug.Log("Train Hit!");
        }
    }

    private void Update()
    {
        _time += Time.deltaTime;
        transform.position = Vector3.Lerp(
            point1.position, 
            new Vector3(point2.position.x + _trainLength, point2.position.y, point2.position.z), 
            _time / trainSystem.GetDurationTrain());

        if(transform.position == point2.position)
        {
            gameObject.SetActive(false);
        }
    }
}
