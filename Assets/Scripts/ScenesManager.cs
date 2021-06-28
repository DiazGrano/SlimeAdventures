using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    //public static ScenesManager sharedInstance;

    public SceneToLoad sceneToLoadName;
    public GameObject sceneTransition;

    private Animator anim;
    public string currentSceneName;
    public string sceneToLoad;
    public int animationDuration = 2;

    

    private void Awake()
    {

        if (sceneTransition)
        {
            if (sceneTransition.GetComponent<Animator>())
            {
                this.anim = sceneTransition.GetComponent<Animator>();
            }
            else if (sceneTransition.GetComponentInChildren<Animator>())
            {
                this.anim = sceneTransition.GetComponentInChildren<Animator>();
            }
            else
            {
                Debug.Log("No se ha encontrado animación de transición");
            }
        }
        else
        {
            Debug.Log("No se ha encontrado animación de transición");
        }

        this.currentSceneName = SceneManager.GetSceneAt(0).name;
        LoadScene();
    }

    public void LoadScene()
    {

        
        if (sceneToLoadName.name.Length > 0)
        {
            this.sceneToLoad = sceneToLoadName.name;
            StartCoroutine(LoadSceneCoroutine());
        }
        else
        {
            Debug.Log("Ninguna escena ha sido seleccionada");
        }

        
    }
    IEnumerator LoadSceneCoroutine()
    {
        if (this.anim != null)
        {
            this.anim.SetTrigger("Open");
        }
        
        yield return new WaitForSeconds(this.animationDuration);

        AsyncOperation auxUnloadCurrentScene = SceneManager.UnloadSceneAsync(currentSceneName);

        while (auxUnloadCurrentScene.progress < 1)
        {
            yield return new WaitForFixedUpdate();
        }
        this.currentSceneName = SceneManager.GetSceneAt(0).name;

        AsyncOperation auxScene = SceneManager.LoadSceneAsync(this.sceneToLoad, LoadSceneMode.Additive);

        while (!auxScene.isDone)
        {
            yield return new WaitForFixedUpdate();
        }


        if (this.anim != null)
        {
            this.anim.SetTrigger("Close");
        }
        
        yield return new WaitForSeconds(this.animationDuration);
        SceneManager.UnloadSceneAsync(currentSceneName);

        yield break;
    }

}
