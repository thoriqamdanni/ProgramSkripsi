using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject darkPanel;
    public GameObject optWindow;

    public ToggleGroup npcGroup;
    public ToggleGroup playByGroup;
    public ToggleGroup obstaclesGroup;
    public ToggleGroup difficultyGroup;

    static public int _npcPlay = 0;
    static public string _playBy = "PLAYER";
    static public bool _playWithObstacles = true;
    static public bool _playWithAIObs = false;

    public void OnPlayBtn_Click()
    {
        SceneManager.LoadScene(1);
    }

    public void OnOptBtn_Click()
    {
        darkPanel.SetActive(true);
        optWindow.SetActive(true);
    }

    public void OnExitBtn_Click()
    {
        Application.Quit();
    }

    public void OnBackBtn_Click()
    {
        darkPanel.SetActive(false);
        optWindow.SetActive(false);
    }

    public void OnOptionsListNPCPlay_Clicked()
    {
        Toggle toggle = npcGroup.ActiveToggles().FirstOrDefault();
        _npcPlay = int.Parse(toggle.GetComponentInChildren<Text>().text);
        Debug.Log("NPC Playing is " + _npcPlay);
    }

    public void OnOptionsListPlayedBy_Clicked()
    {
        Toggle toggle = playByGroup.ActiveToggles().FirstOrDefault();
        _playBy = toggle.GetComponentInChildren<Text>().text;
        Debug.Log("Played by " + _playBy);
    }
    
    public void OnOptionsListObstacles_Clicked()
    {
        Toggle toggle = obstaclesGroup.ActiveToggles().FirstOrDefault();
        string temp = toggle.GetComponentInChildren<Text>().text;

        if (temp == "ON")
            _playWithObstacles = true;
        else if (temp == "OFF")
            _playWithObstacles = false;
        Debug.Log("Play with obstacle is " + temp);
    }

    public void OnOptionsListDifficulty_Clicked()
    {
        Toggle toggle = difficultyGroup.ActiveToggles().FirstOrDefault();
        string temp = toggle.GetComponentInChildren<Text>().text;

        if (temp == "AI")
        {
            _playWithAIObs = false;
            Debug.Log("Play with AI Mode");

        }
        else if (temp == "AI+Obs")
        {
            _playWithAIObs = true;
            Debug.Log("Play with AI+Obs Mode");
        }
    }
}
