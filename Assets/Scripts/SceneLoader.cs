using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    #region Public methods

    public void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void LoadNextScene()
    {
        var currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        var maxSceneIndex = SceneManager.sceneCountInBuildSettings - 1;
        
        if (currentSceneIndex == maxSceneIndex)
        {
            Debug.LogError("Невозможно загрузить следущую сцену. Эта последняя.");
            return;
        }
        
        if (currentSceneIndex < maxSceneIndex)
        {
            SceneManager.LoadScene(currentSceneIndex + 1);
        }
        

    }

    public void ExitGame()
    {
        Application.Quit();
    }

    #endregion
}