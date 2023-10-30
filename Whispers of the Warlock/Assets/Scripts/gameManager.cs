using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;

    public GameObject player;
    public GameObject enemy;

    public bool isPaused;
    float timeScaleOrig;
    int enemiesRemaining;

    private void Awake()
    {
        timeScaleOrig = Time.timeScale;
        instance = this;
        player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        if(Input.GetButtonDown("Cancel") && menuActive == null)
        {
            StatePause();
            menuActive = menuPause;
            menuActive.SetActive(isPaused);
        }   
    }

    public void StatePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void StateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }
    public void updateGoal(int amount)
    {
        enemiesRemaining += amount;
        if (enemiesRemaining <= 0)
        {
            Winner();
        }
    }
    public void Winner()
    {
        StatePause();
        menuActive = menuWin;
        menuActive.SetActive(true);
    }

    public void Lose()
    {
        StatePause();
        menuActive = menuLose;
        menuActive.SetActive(true);

    }



}
