using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void StartButton()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void ControlsButton()
    {
        SceneManager.LoadScene("Controls");
    }

    public void QuitButton()
    {
        Application.Quit();
        Debug.Log("Game is Exiting"); 
    }
}
