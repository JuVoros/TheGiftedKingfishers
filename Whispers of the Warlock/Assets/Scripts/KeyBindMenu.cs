using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class KeybindMenu : MonoBehaviour
{
    public playerPrefsManager prefsManager;
    public KeyBind keyBindScript;

    public Text moveUpText;
    public Text moveDownText;
    public Text moveRightText;
    public Text moveLeftText;
    public Text jumpText;
    public Text sprintText;
    public Text interactText;
    public Text shootText;
    public Text shieldText;
    public Text blinkText;

    private bool rebinding = false;
    private Text currentRebindText;

    void Start()
    {

        moveUpText = GameObject.Find("MoveUpText").GetComponent<Text>();
        moveDownText = GameObject.Find("MoveDownText").GetComponent<Text>();
        moveLeftText = GameObject.Find("MoveLeftText").GetComponent<Text>();
        moveRightText = GameObject.Find("MoveRightText").GetComponent<Text>();
        blinkText = GameObject.Find("BlinkText").GetComponent<Text>();
        sprintText = GameObject.Find("SprintText").GetComponent<Text>();
        jumpText = GameObject.Find("JumpText").GetComponent<Text>();
        shieldText = GameObject.Find("ShieldText").GetComponent<Text>();
        shootText = GameObject.Find("ShootText").GetComponent<Text>();
        interactText = GameObject.Find("InteractText").GetComponent<Text>();
        
        if (keyBindScript != null)
        {

            keyBindScript.UpdateKeybindingTexts(
        moveUpText, moveDownText, moveRightText, moveLeftText,
        jumpText, sprintText, interactText, shootText, shieldText, blinkText);
        }
    }


    public void StartRebind(Text uiText, playerPrefsManager.GameAction action)
    {
        if (!rebinding)
        {
            currentRebindText = uiText;
            StartCoroutine(RebindCoroutine(action));
        }
    }

    IEnumerator RebindCoroutine(playerPrefsManager.GameAction action)
    {
        rebinding = true;
        currentRebindText.text = "Press a key to bind...";

        while (!Input.anyKeyDown)
        {
            yield return null;
        }

        KeyCode newKey = KeyCode.None;

        foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(key))
            {
                newKey = key;
                break;
            }
        }

        prefsManager.SetKeybind(action, newKey);
        currentRebindText.text = $"{action}: {newKey}";
        keyBindScript.UpdateKeybindingTexts(
            moveUpText, moveDownText, moveRightText, moveLeftText,
            jumpText, sprintText, interactText, shootText, shieldText, blinkText);

        rebinding = false;
    }
}