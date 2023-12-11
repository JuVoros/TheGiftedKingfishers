using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class buttonFunctions : MonoBehaviour
{
    private KeybindMenu keyBindMenu;

    void Start()
    {
        keyBindMenu = GetComponent<KeybindMenu>();
    }

    public void Resume()
    {
        gameManager.instance.StateUnpause();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameManager.instance.StateUnpause();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void playerRespawn()
    {
        gameManager.instance.playerScript.spawnPlayer();
        gameManager.instance.StateUnpause();
    }

    public void RebindJump()
    {
        if (keyBindMenu != null)
        {
            keyBindMenu.StartRebind(keyBindMenu.jumpText, playerPrefsManager.GameAction.Jump);
        }
    }

    public void RebindSprint()
    {
        if (keyBindMenu != null)
        {
            keyBindMenu.StartRebind(keyBindMenu.sprintText, playerPrefsManager.GameAction.Sprint);
        }
    }

    public void RebindShoot()
    {
        if (keyBindMenu != null)
        {
            keyBindMenu.StartRebind(keyBindMenu.shootText, playerPrefsManager.GameAction.Shoot);
        }
    }

    public void RebindBlink()
    {
        if (keyBindMenu != null)
        {
            keyBindMenu.StartRebind(keyBindMenu.blinkText, playerPrefsManager.GameAction.Blink);
        }
    }

    public void RebindShield()
    {
        if (keyBindMenu != null)
        {
            keyBindMenu.StartRebind(keyBindMenu.shieldText, playerPrefsManager.GameAction.Shield);
        }
    }

    public void RebindInteract()
    {
        if (keyBindMenu != null)
        {
            keyBindMenu.StartRebind(keyBindMenu.interactText, playerPrefsManager.GameAction.Interact);
        }
    }

    public void RebindMoveUp()
    {
        if (keyBindMenu != null)
        {
            keyBindMenu.StartRebind(keyBindMenu.moveUpText, playerPrefsManager.GameAction.MoveUp);
        }
    }

    public void RebindMoveDown()
    {
        if (keyBindMenu != null)
        {
            keyBindMenu.StartRebind(keyBindMenu.moveDownText, playerPrefsManager.GameAction.MoveDown);
        }
    }

    public void RebindMoveLeft()
    {
        if (keyBindMenu != null)
        {
            keyBindMenu.StartRebind(keyBindMenu.moveLeftText, playerPrefsManager.GameAction.MoveLeft);
        }
    }

    public void RebindMoveRight()
    {
        if (keyBindMenu != null)
        {
            keyBindMenu.StartRebind(keyBindMenu.moveRightText, playerPrefsManager.GameAction.MoveRight);
        }
    }
}
