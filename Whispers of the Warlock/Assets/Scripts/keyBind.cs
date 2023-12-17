using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class KeyBind : MonoBehaviour
{
    private playerPrefsManager prefsManager;

    private void Start()
    {
        prefsManager = playerPrefsManager.Instance;

        if (prefsManager == null)
        {
        }
        if (prefsManager != null)
        {
        prefsManager = GetComponent<playerPrefsManager>();

        }
        
    }
    void Update()
    {
    }

    public void UpdateKeybindingTexts(Text moveUpText, Text moveDownText, Text moveRightText, Text moveLeftText,
        Text jumpText, Text sprintText, Text interactText, Text shootText, Text shieldText, Text blinkText)
    {
        moveUpText.text = $"Move Up: {prefsManager.GetKeybind(playerPrefsManager.GameAction.MoveUp)}";
        moveDownText.text = $"Move Down: {prefsManager.GetKeybind(playerPrefsManager.GameAction.MoveDown)}";
        moveRightText.text = $"Move Right: {prefsManager.GetKeybind(playerPrefsManager.GameAction.MoveRight)}";
        moveLeftText.text = $"Move Left: {prefsManager.GetKeybind(playerPrefsManager.GameAction.MoveLeft)}";
        jumpText.text = $"Jump: {prefsManager.GetKeybind(playerPrefsManager.GameAction.Jump)}";
        sprintText.text = $"Sprint: {prefsManager.GetKeybind(playerPrefsManager.GameAction.Sprint)}";
        interactText.text = $"Interact: {prefsManager.GetKeybind(playerPrefsManager.GameAction.Interact)}";
        shootText.text = $"Shoot: {prefsManager.GetKeybind(playerPrefsManager.GameAction.Shoot)}";
        shieldText.text = $"Shield: {prefsManager.GetKeybind(playerPrefsManager.GameAction.Shield)}";
        blinkText.text = $"Blink: {prefsManager.GetKeybind(playerPrefsManager.GameAction.Blink)}";
    }
}