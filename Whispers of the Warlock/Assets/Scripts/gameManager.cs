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
using UnityEngine.Audio;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;
    [SerializeField] GameObject menuActive;
    

    [Header("------ Menus ------")]    
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject optionsMenu;
    [SerializeField] GameObject keybindsMenu; 
    [SerializeField] GameObject audioMenu;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject inven;

    [Header("------Features ------")]
    [SerializeField] AudioClip jumpScareSound;
    [SerializeField] GameObject jumpScareScreen;
    [SerializeField] GameObject playerDamageScreen;
    [SerializeField] GameObject playerHealthScreen;
    [SerializeField] GameObject blinkScreen;
    [SerializeField] GameObject bossDeathScreen;
    [SerializeField] cameraController cameraController;
    [Range(0,1)][SerializeField] float playerFlashTime;

    [Header("------ Buttons ------")]
    [SerializeField] GameObject pauseMenuButton;
    [SerializeField] GameObject optionsMenuButton;
    [SerializeField] GameObject audioMenuButton;
    [SerializeField] GameObject controlsMenuButton;
    [SerializeField] GameObject winMenuButton;
    [SerializeField] GameObject loseMenuButton;

    [Header("------ Audio ------")]
    [SerializeField] AudioMixer mixer;
    [SerializeField] float jumpScareVol;

    [Header("------ Weapons ------")]
    [SerializeField] List<GameObject> weaponDrops;
    public List<GameObject> tempWeaponDrops;    
    [SerializeField] Transform[] weaponSpawmPos;
    [SerializeField] List<GameObject> potionDrops;

    [Header("------ UI ------")]
    [SerializeField] TMP_Text enemyCountText;
    [SerializeField] TMP_Text gameGoal;
    [SerializeField] TMP_Text weaponNameText;
    [SerializeField] TMP_Text hpPotionCount;
    [SerializeField] TMP_Text manaPotionCount;
    [SerializeField] GameObject reticle;
    [SerializeField] Slider sensitivitySlider;
    [SerializeField] Text sensitivityText;


    [Header("----- Potions -----")]
    [Range(1, 10)][SerializeField] int HpOnPickup;
    [Range(1, 10)][SerializeField] int ManaOnPickup;



    public Image playerHPBar;
    public Image playerManaBar;
    public Image teleportIcon;
    public GameObject playerSpawnPos;
    public GameObject player;
    public playerController playerScript;
    manaPotion manaScript;
    HealthPotion healthScript;
    public int manaPots;
    public int hpPots;
    public int enemiesRemaining;
    public bool isPaused;
    bool invenOpen;
    float timeScaleOrig;
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
        sensitivitySlider.value = cameraController.sensitivity;
        UpdateSensitivityText(cameraController.sensitivity);
    }

    public void UpdateSensitivity(float newSensitivity)
    {
        cameraController.UpdateSensitivity(newSensitivity);
        UpdateSensitivityText(newSensitivity);
    }
    private void UpdateSensitivityText(float sensitivityValue)
    {
        sensitivityText.text = $"Sensitivity: {sensitivityValue}";
    }

    void Update()
    {
        if(Input.GetButtonDown("Cancel") && menuActive == null && !invenOpen)
        {
            StatePause();
            menuActive = menuPause;
            Main();
            menuActive.SetActive(isPaused);
        }
        if (Input.GetButtonDown("Inventory") && menuActive == null && !invenOpen)
        {

            Inventory();

        }
        else if (Input.GetButtonDown("Inventory") && invenOpen)
        {

            inven.SetActive(false);
            invenOpen = false;
            
        }
        hpPotionCount.text = hpPots.ToString("0");
        manaPotionCount.text = manaPots.ToString("0");
        if (Input.GetButtonDown("Health Pot") && hpPots > 0 && playerScript.PlayerHPOrig != playerScript.HP)
        {
            consumeHP();
        }
        if (Input.GetButtonDown("Mana Pot") && manaPots > 0 && playerScript.manaMax != playerScript.manaCur)
        {
            consumeMP();

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
        enemyCountText.text = enemiesRemaining.ToString("0");

            

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
        mixer.SetFloat("Master", 0);
        menuActive.SetActive(true);
        index = 0;
    }
    public void Audio()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(audioMenuButton);
        menuActive.SetActive(false);
        menuActive = audioMenu;
        mixer.SetFloat("Master", -80);
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

    public IEnumerator playerFlashBlink()
    {
        blinkScreen.SetActive(true);
        yield return new WaitForSeconds(playerFlashTime);
        blinkScreen.SetActive(false);
    }

    public void playerBlinkFOVup()
    {
        cameraController.blinkFOVup();
        StartCoroutine(playerFlashBlink());

    }

    public void playerBlinkFOVdown()
    {
        cameraController.blinkFOVdown();
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
    void Inventory()
    {
        inven.SetActive(true);
        invenOpen = true;
        
    }
    void consumeHP()
    {
        hpPots -= 1;
        playerScript.addHealth(HpOnPickup);
    }
    void consumeMP()
    {
        manaPots -= 1;
       playerScript.addMana(ManaOnPickup);
    }





}
