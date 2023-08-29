using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controlsMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject controls;

    public void Start()
    {
        mainMenu.SetActive(true);
        controls.SetActive(false); 
    }
    public void controlsButton()
    {
        mainMenu.SetActive(false);
        controls.SetActive(true); 
    }

}
