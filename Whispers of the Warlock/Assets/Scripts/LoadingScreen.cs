using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] GameObject loadingScreen;
    [SerializeField] Image loadBar;
public void loadScene(int sceneID)
    {
        StartCoroutine(loadSceneAsync(sceneID));
    }

    IEnumerator loadSceneAsync(int sceneID)
    {
        

        loadingScreen.SetActive(true);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneID);
        while (!operation.isDone)
        {
            float progressValue = Mathf.Clamp01(operation.progress*0.1f);
            loadBar.fillAmount = progressValue;
            yield return null;
        }
    }
    

    
}
