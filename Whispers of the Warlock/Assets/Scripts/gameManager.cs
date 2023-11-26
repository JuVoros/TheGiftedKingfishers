using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine.EventSystems;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject optionsMenu;
    [SerializeField] GameObject keybindsMenu; 
    [SerializeField] GameObject audioMenu;
    [SerializeField] GameObject pauseMenuButton;
    [SerializeField] GameObject optionsMenuButton;
    [SerializeField] GameObject audioMenuButton;
    [SerializeField] GameObject controlsMenuButton;
    [SerializeField] GameObject winMenuButton;
    [SerializeField] GameObject loseMenuButton;


    [SerializeField] GameObject jumpScareScreen;
    [SerializeField] GameObject playerDamageScreen;
    [SerializeField] GameObject playerHealthScreen;
    [SerializeField] GameObject bossDeathScreen;
    [SerializeField] GameObject reticle;

    [SerializeField] AudioClip jumpScareSound;
    [SerializeField] float jumpScareVol;

    [SerializeField] List<GameObject> weaponDrops;
    public List<GameObject> tempWeaponDrops;    
    [SerializeField] Transform[] weaponSpawmPos;
    [SerializeField] List<GameObject> potionDrops;    
    
    [SerializeField] TMP_Text enemyCountText;
    [SerializeField] TMP_Text weaponNameText;
    [Range(0,1)][SerializeField] float playerFlashTime;

    public Image playerHPBar;
    public Image playerManaBar;
    public GameObject playerSpawnPos;
    public GameObject player;
    public playerController playerScript;

    public bool isPaused;
    float timeScaleOrig;
    public int enemiesRemaining;
    int index;
    void Awake()
    {
        tempWeaponDrops = weaponDrops;
        timeScaleOrig = Time.timeScale;
        instance = this;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>(); 
        playerSpawnPos = GameObject.FindWithTag("Respawn");
        spawnItem();
    }

    void Update()
    {
        if(Input.GetButtonDown("Cancel") && menuActive == null)
        {
            StatePause();
            menuActive = menuPause;
            Main();
            menuActive.SetActive(isPaused);
        }
        
    }

    public void StatePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        reticle.SetActive(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void StateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScaleOrig;
        reticle.SetActive(true);
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

    public  void weaponNameUpdate()
    {
        weaponNameText.text = playerScript.getWeaponName();
    }

    public IEnumerator Winner()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(winMenuButton);
        yield return new WaitForSeconds(3);
        StatePause();
        menuActive = menuWin;
        menuActive.SetActive(true);
    }

    public void Lose()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(loseMenuButton);
        StatePause();
        menuActive = menuLose;
        menuActive.SetActive(true);

    }
    public void Options()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(optionsMenuButton);
        menuActive.SetActive(false);
        menuActive = optionsMenu;
        menuActive.SetActive(true);
        index = 0;
    }
    public void Audio()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(audioMenuButton);
        menuActive.SetActive(false);
        menuActive = audioMenu;
        menuActive.SetActive(true);
        index = 1;
    }
    public void keybinds()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(controlsMenuButton);
        menuActive.SetActive(false);
        menuActive = keybindsMenu;
        menuActive.SetActive(true);
        index = 1;
    }
    public void Main()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(pauseMenuButton);
        menuActive.SetActive(false);
        menuActive = menuPause;
        menuActive.SetActive(true);
        index = 0;

    }
    public void backButton()
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
    public void ScareJump()
    {
       StartCoroutine(playerScript.jumpScare(jumpScareScreen, jumpScareSound, jumpScareVol));

    }
    public List<GameObject> getWeaponDrops()
    {
        return tempWeaponDrops;
        
    }
    public List<GameObject> getPotionDrops()
    {
        return potionDrops;
    }
    
    public void spawnItem()
    {
        for (int i = 0; i < weaponSpawmPos.Length; i++)
        {            
            int weaponToSpawn = Random.Range(0, tempWeaponDrops.Count - 1);
            Instantiate(tempWeaponDrops[weaponToSpawn], weaponSpawmPos[i].position, weaponSpawmPos[i].rotation);
            tempWeaponDrops.RemoveAt(weaponToSpawn);
        }

    }
}
