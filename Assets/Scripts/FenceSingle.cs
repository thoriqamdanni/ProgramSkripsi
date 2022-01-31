using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Orientation { Horizontal, Vertical }

public class FenceSingle : MonoBehaviour
{
    [SerializeField] private Color color1 = Color.red;
    [SerializeField] private Color color2 = Color.blue;
    [SerializeField] private float delaySec = 1f;
    [SerializeField] private Orientation orientation = Orientation.Horizontal;

    GameObject fencePivot;

    SpriteRenderer lampTop;
    SpriteRenderer lampBott;

    private IEnumerator lampAnim = null;

    private TrainSystem trainSystem;

    private void Awake()
    {
        fencePivot = transform.Find("Pivot").gameObject;
        lampTop = transform.Find("Train Lamp/headlights").GetComponent<SpriteRenderer>();
        lampBott = transform.Find("Train Lamp/headlights (1)").GetComponent<SpriteRenderer>();

        if (fencePivot == null)
            Debug.Log(gameObject.name + ": No fence's pivot has been initialized!");

        if (lampTop == null || lampBott == null)
            Debug.Log(gameObject.name + ": No lamp has been initialized!");
    }

    public void LoadGate()
    {
        if(!trainSystem.IsOpen())
        {
            CloseFence();
            TurnOnLamp();
        }
        else
        {
            OpenFence();
            TurnOffLamp();
        }
    }

    public void SetTrainSystem(TrainSystem trainSystem)
    {
        this.trainSystem = trainSystem;
    }

    private void OpenFence()
    {
        StartCoroutine(AnimationFence(new Vector3(0, 0, 0 + (int)orientation*90), new Vector3(0, 0, 100 + (int)orientation*90), trainSystem.GetDurationFence(), fencePivot));
    }

    private void CloseFence()
    {
        StartCoroutine(AnimationFence(fencePivot.transform.eulerAngles, new Vector3(0, 0, 0 + (int)orientation*90), trainSystem.GetDurationFence(), fencePivot));
    }

    private void TurnOnLamp()
    {
        lampAnim = LampAnimation(Color.red, Color.blue, 1f, lampTop, lampBott);
        StartCoroutine(lampAnim);
    }

    private void TurnOffLamp()
    {
        lampTop.color = Color.white;
        lampBott.color = Color.white;

        if(lampAnim != null)
            StopCoroutine(lampAnim);
    }

    IEnumerator AnimationFence(Vector3 startAngle, Vector3 endAngle, float duration, GameObject fence)
    {
        for(float time = 0f; time < duration; time += Time.deltaTime)
        {
            fence.transform.eulerAngles = Vector3.Lerp(startAngle, endAngle, time / duration);
            yield return null;
        }
    }

    IEnumerator LampAnimation(Color c1, Color c2, float changeSecond, SpriteRenderer lamp1, SpriteRenderer lamp2)
    {
        while(true)
        {
            (c1, c2) = (c2, c1);
            lamp1.color = c1;
            lamp2.color = c2;
            yield return new WaitForSeconds(changeSecond);
        }
    }
}
