using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private bool _timeStart = false;
    private float _time = 0f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        UnCheck();
    }

    private void Update()
    {
        if(_timeStart)
            _time += Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<CarController>(out CarController car))
        {
            Debug.Log("Hit Wall!");
            Checked();
            _timeStart = true;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.TryGetComponent<CarController>(out CarController car))
        {
            // Car hit & stay in wall
            if (_time >= 3f)
            {
                _timeStart = false;
                _time = 0f;
                car.RespawnNearCheckpoint();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<CarController>(out CarController car))
        {
            Debug.Log("Exit Wall!");
            UnCheck();
        }
    }

    public void UnCheck()
    {
        //spriteRenderer.color = new Color(255f, 150f, 0f, 0.2f);
    }

    public void Checked()
    {
        //spriteRenderer.color = new Color(180f, 180f, 180f, 0.2f);
    }
}
