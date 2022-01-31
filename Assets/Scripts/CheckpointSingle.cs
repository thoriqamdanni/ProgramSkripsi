using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSingle : MonoBehaviour
{
    private TrackCheckpoints trackCheckpoints;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        UnCheck();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<CarController>(out CarController car))
        {
            trackCheckpoints.CarPassCheckpoint(this, other.transform);
        }
    }

    public void SetTrackCheckpoints(TrackCheckpoints trackCheckpoints)
    {
        this.trackCheckpoints = trackCheckpoints;
    }

    public void UnCheck()
    {
        //spriteRenderer.color = new Color(0f, 0f, 225f, 0.2f);
    }

    public void Checked()
    {
        //spriteRenderer.color = new Color(0f, 225f, 0f, 0.2f);
    }

    public void NextCheck()
    {
        //spriteRenderer.color = new Color(225f, 0f, 0f, 0.2f);
    }
}
