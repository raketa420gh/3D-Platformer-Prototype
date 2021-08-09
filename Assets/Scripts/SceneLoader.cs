using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    #region Variables

    private int currentSceneIndex;
    private int lastSceneIndex;

    #endregion
    
    
    #region Events

    public static event Action OnSceneLoaded;
    
    #endregion


    #region Unity lifecycle

    private void Start()
    {
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        lastSceneIndex = SceneManager.sceneCountInBuildSettings - 1;
    }

    #endregion


    #region Public methods

    public void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
        OnSceneLoaded?.Invoke();
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(currentSceneIndex);
        OnSceneLoaded?.Invoke();
    }

    public void LoadNextScene()
    {
        if (currentSceneIndex == lastSceneIndex)
        {
            Debug.LogError("Невозможно загрузить следущую сцену. Эта последняя.");
            return;
        }
        
        if (currentSceneIndex < lastSceneIndex)
        {
            SceneManager.LoadScene(currentSceneIndex + 1);
        }
        
        OnSceneLoaded?.Invoke();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    #endregion
}