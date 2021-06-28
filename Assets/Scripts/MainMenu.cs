using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public SceneToLoad sceneToLoad;
    public string sceneToLoadName = "";

    public void OpenLoadScreen()
    {
        this.sceneToLoad.name = sceneToLoadName;
        SceneManager.LoadScene("LoadingScreen", LoadSceneMode.Additive);
        
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
