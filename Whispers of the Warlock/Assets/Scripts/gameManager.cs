using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject reloadPromp;

    [SerializeField] GameObject playerDamageScreen;
    [SerializeField] GameObject playerHealthScreen;
    [SerializeField] GameObject bossDeathScreen;
    
    [SerializeField] TMP_Text enemyCountText;
   [Range(0,1)][SerializeField] float playerFlashTime;

    public Image playerHPBar;
    public Image playerManaBar;
    public GameObject playerSpawnPos;
    public GameObject player;
    public playerController playerScript;

    public bool isPaused;
    float timeScaleOrig;
    public int enemiesRemaining;
    void Awake()
    {
        timeScaleOrig = Time.timeScale;
        instance = this;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>(); 
        playerSpawnPos = GameObject.FindWithTag("Respawn");
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

        StartCoroutine(BossDeathFlash());
        enemyCountText.text = enemiesRemaining.ToString("0");

            
        if (enemiesRemaining <= 0)
        {
            StartCoroutine(Winner());
        }
    }
    public IEnumerator Winner()
    {
        yield return new WaitForSeconds(3);
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

    public IEnumerator playerFlashDamage()
    {
        playerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(playerFlashTime);
        playerDamageScreen.SetActive(false);
    }

    public IEnumerator playerFlashHeals()
    {
        playerHealthScreen.SetActive(true);
        yield return new WaitForSeconds(playerFlashTime);
        playerHealthScreen.SetActive(false);
    }

    public IEnumerator BossDeathFlash()
    {
        
        bossDeathScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        bossDeathScreen.SetActive(false);
    }
    public void ReloadText(bool set)
    {

       reloadPromp.SetActive(set);

    }
   
}
