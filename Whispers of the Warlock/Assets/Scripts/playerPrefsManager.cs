using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerPrefsManager : MonoBehaviour
{
    private static playerPrefsManager _instance;

    public static playerPrefsManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<playerPrefsManager>();
                if( _instance == null ) 
                {
                    GameObject go = new GameObject("playerPrefsManager");
                    _instance = go.AddComponent<playerPrefsManager>();
                }
            }
            return _instance;
        }
    }
    public enum GameAction
    {
        Jump,
        Sprint,
        Interact,
        Shoot,
        Shield,
        Blink,
        MoveUp,
        MoveDown,
        MoveRight,
        MoveLeft,
        None
    }
    private const string KeybindPrefix = "Keybind_";

    public KeyCode GetKeybind(GameAction action)
    {
        string key = KeybindPrefix + action.ToString();
        return (KeyCode)PlayerPrefs.GetInt(key, (int)DefaultKeybind(action));
    }

    public void SetKeybind(GameAction action, KeyCode key)
    {
        string keyName = KeybindPrefix + action.ToString();
        PlayerPrefs.SetInt(keyName, (int)key);
    }

    public static GameAction GameActionFromString(string action)
    {
        return Enum.TryParse(action, out GameAction result) ? result : GameAction.None;
    }

    public float GetAxisFromKeybind(GameAction action)
    {
        KeyCode key = GetKeybind(action);
        float axisValue = 0.0f;
        if (Input.GetKey(key))
        {
            axisValue = 1.0f;
        }
        return axisValue;
    }

    private static KeyCode DefaultKeybind(GameAction action)
    {
        switch (action)
        {
            case GameAction.Jump:
                return KeyCode.Space;       
            case GameAction.Sprint:
                return KeyCode.LeftShift;            
            case GameAction.Interact:
                return KeyCode.Q;            
            case GameAction.Shoot:
                return KeyCode.Mouse0;            
            case GameAction.Shield:
                return KeyCode.E;            
            case GameAction.Blink:
                return KeyCode.Mouse1;            
            case GameAction.MoveUp:
                return KeyCode.W;            
            case GameAction.MoveDown:
                return KeyCode.S;            
            case GameAction.MoveRight:
                return KeyCode.D;
            case GameAction.MoveLeft:
                return KeyCode.A;
            default:
                return KeyCode.None;
        }
    }
}
