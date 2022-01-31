using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    public CarsManager carManager;
    public GameObject darkPanel;
    public GameObject pauseWindow;
    public GameObject finalWindow;
    public GameObject Laps;
    public GameObject lapTextPrefab;

    [SerializeField] private TrackCheckpoints trackCheckpoints;
    [SerializeField] private List<Transform> avalCarTransform;
    [SerializeField] private bool _isAllFinished = false;
    private bool _isFinalWindowOpen = false;

    private void Start()
    {
        trackCheckpoints.OnCarFinishLap += Checkpoints_CheckAllCarLap;

        var carTransform = trackCheckpoints.getCarTransformList();
        foreach(Transform car in carTransform)
        {
            if (car.gameObject.activeSelf)
                avalCarTransform.Add(car);
        }
    }

    private void Update()
    {
        if(_isAllFinished && !_isFinalWindowOpen && carManager.doLap)
        {
            ShowFinalWindow();
            _isFinalWindowOpen = true;
        }
    }
    
    private void Checkpoints_CheckAllCarLap(object sender, CarCheckpointsEventArgs e)
    {
        Debug.Log("Some car just finish a lap!");

        // Check all car if it's all finished if yes then show finalWindow
        StartCoroutine(checkAllCarifFinished());

    }

    IEnumerator checkAllCarifFinished()
    {
        // Wait to the corresponding car increase its lap
        yield return new WaitForSeconds(1f);

        foreach (Transform car in avalCarTransform)
        {
            var c = car.GetComponent<CarController>();

            if (!c.getFinished())
                yield break;
        }

        _isAllFinished = true;
    }

    public void OnPauseBtn_Clicked()
    {
        darkPanel.SetActive(true);
        pauseWindow.SetActive(true);
        Time.timeScale = 0;
    }

    public void OnResetBtn_Clicked()
    {
        Application.LoadLevel(Application.loadedLevel);
        Time.timeScale = 1;
    }

    public void OnMenuBtn_Clicked()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }

    public void OnBackBtn_Clicked()
    {
        darkPanel.SetActive(false);
        pauseWindow.SetActive(false);
        finalWindow.SetActive(false);
        Time.timeScale = 1;
    }

    public void ShowFinalWindow()
    {
        darkPanel.SetActive(true);

        for(int i = 0; i < avalCarTransform.Count; i++)
        {
            GameObject obj = Instantiate(lapTextPrefab, Laps.transform, false);
            obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y - 70*i, obj.transform.position.z);
            
            string laptime = "";
            for(int k = 0; k < carManager.maxLap; k++)
            {
                laptime += avalCarTransform[i].GetComponent<CarController>().getlapTime(k);

                if (k == carManager.maxLap - 1)
                    continue;
                else
                    laptime += ", ";

            }

            obj.GetComponentInChildren<Text>().text = avalCarTransform[i].name + " : " + laptime;
        }

        finalWindow.SetActive(true);

        Time.timeScale = 0;
    }
}
