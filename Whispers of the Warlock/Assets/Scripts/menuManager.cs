using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class menuManager : MonoBehaviour
{
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject optionsMenu;
    [SerializeField] GameObject AudioMenu;
    [SerializeField] GameObject ControlsMenu;
    [SerializeField] GameObject menuActive;

   
    public void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        menuActive = mainMenu;
    }
    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void Options()
    {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
        menuActive = optionsMenu;
    }

    public void Audio()
    {
        optionsMenu.SetActive(false);
        AudioMenu.SetActive(true);
        menuActive = AudioMenu;
    }
    public void Controls()
    {
        
        optionsMenu.SetActive(false);
        ControlsMenu.SetActive(true);
        menuActive = ControlsMenu;
    }








    public void Back()
    {
        if(menuActive != optionsMenu)
        {
            menuActive.SetActive(false);
            menuActive = optionsMenu;
            menuActive.SetActive(true);
        }
        else
        {
            menuActive.SetActive(false);
            menuActive = mainMenu;
            menuActive.SetActive(true);
        }


    }
    public void QuitGame()
    {
        Application.Quit();

    }


}
