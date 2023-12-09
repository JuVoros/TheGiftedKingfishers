
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Unity.VisualScripting.Member;

public class menuManager : MonoBehaviour
{
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject MainMenu;
    [SerializeField] GameObject OptionsMenu;
    [SerializeField] GameObject AudioMenu;
    [SerializeField] GameObject ControlsMenu;    
    [SerializeField] GameObject CreditMenu;
    [SerializeField] AudioMixer mixer;
    [SerializeField] LoadingScreen loadingScreen;

    [SerializeField] GameObject firstMainButton;
    [SerializeField] GameObject firstOptionsButton;
    [SerializeField] GameObject firstAudioButton;
    [SerializeField] GameObject firstControlsButton;
    [SerializeField] GameObject firstCreditsButton;

    int index;
   
    public void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        menuActive = MainMenu;
        Main();
    }  
    public void StartGame()
    {
        loadingScreen.loadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void Options()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstOptionsButton);
        menuActive.SetActive(false);       
        menuActive = OptionsMenu;
        mixer.SetFloat("Master", 0);
        menuActive.SetActive(true);
        index = 0;
    }
    public void Audio()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstAudioButton);
        menuActive.SetActive(false);
        menuActive = AudioMenu;
        //mixer.SetFloat("Master", -80);
        menuActive.SetActive(true);
        index = 1;
    }
    public void Controls()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstControlsButton);
        menuActive.SetActive(false);
        menuActive = ControlsMenu;
        menuActive.SetActive(true);
        index = 1;
    }
    public void Credits()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstCreditsButton);
        menuActive.SetActive(false);
        menuActive = CreditMenu;
        menuActive.SetActive(true);
        index = 0;

    }
    public void Back()
    {
        switch (index)
        {

            case 0:
                Main();
                break;

            case 1:
                Options();
                break;
        }     

    }
    public void Main()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstMainButton);
        menuActive.SetActive(false);
        menuActive = MainMenu;
        menuActive.SetActive(true);
    }
    public void QuitGame()
    {
        Application.Quit();

    }
   
}
