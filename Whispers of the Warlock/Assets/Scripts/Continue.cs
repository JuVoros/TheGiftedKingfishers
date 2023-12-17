using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Continue : MonoBehaviour
{
    [SerializeField] LoadingScreen loadingScreen;
    public void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        KeyCode continueKey = KeyCode.Space;
        if (Input.GetKeyDown(continueKey))
        {
            loadingScreen.loadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
